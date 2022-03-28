using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NaughtyAttributes;
using Other;
using Runtime.States;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Managers
{
    public class InputManager : MonoSingleton<InputManager>,IDragHandler,IPointerDownHandler,IPointerUpHandler,IEndDragHandler
    {
        public State currentState;
        public string CurrentStateName => StateToString(currentState);
    
        private Dictionary<string, State> allStates;
        public Dictionary<string, State> AllStates
        {
            get => allStates;
            set => allStates = value;
        }

        [SerializeField]
        private StateChanger stateChanger;
        public void SetState(string stateName)
        {
            var state = StringToState(stateName);
            if (state == null)
            {
                Debug.LogError($"{stateName} - State Not Exist!");
                return;
            }
            currentState?.OnExit();
            if (currentState == state) return;
            currentState = state;
            currentState.OnStart();
        }
        public void SetState(string stateName,bool forceStart)
        {
            var state = StringToState(stateName);
            if (state == null)
            {
                Debug.LogError($"{stateName} - State Not Exist!");
                return;
            }
            currentState?.OnExit();
            if (currentState == state && !forceStart) return;
            currentState = state;
            currentState.OnStart();
        
        }

        private State StringToState(string stateName)
        {
            foreach (var state in allStates)
            {
                if (state.Key.ToLower() == stateName.ToLower())
                {
                    return state.Value;
                }
            }
            return null;
        }
        private string StateToString(State state)
        {
            if (state == null) return null;
            foreach (var s in allStates)
            {
                if (s.Value == state)
                {
                    return s.Key;
                }
            }
            return null;
        }
        private void Awake()
        {
            InitStates();
        }

        private void InitStates()
        {
            if(allStates == null) allStates = new Dictionary<string, State>();
            allStates.Add("EmptyState", new EmptyState());
            allStates.Add("RunState", new RunState());
		allStates.Add("CardState", new CardState());
		//[NEW_STATE]
        }

        private void Update()
        {
            currentState?.OnUpdate();
        }

        public void OnDrag(PointerEventData eventData)
        {
            currentState?.OnDrag(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            currentState?.OnPointerDown(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            currentState?.OnPointerUp(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            currentState?.OnEndDrag(eventData);
        }
    
    }

    public class StateChanger : MonoBehaviour
    {
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(StateChanger)),Serializable]
    public class StateChangerDrawer : PropertyDrawer
    {
        private InputManager inputManager;
        private int selectedState = 0;
    
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
        
            if (!Application.isPlaying)
            {
                GUILayout.Label($"Waiting to play..",EditorStyles.helpBox);
                return;
            }
            inputManager = InputManager.Instance;
            if(inputManager.currentState != null) GUILayout.Label($"<color=\"White\">Selected state <b>{inputManager.CurrentStateName}</b></color>",GUIStyle.none);
            var allStateNames = inputManager.AllStates.Keys.ToArray();
            selectedState = EditorGUILayout.Popup("States", selectedState, allStateNames);
            if (GUILayout.Button("Select State"))
            {
                inputManager.SetState(allStateNames[selectedState]);
            }
            //base.OnGUI(position, property, label);

        }
    }
#endif
}
