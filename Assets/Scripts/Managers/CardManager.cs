using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Other;
using Runtime;
using ScriptableObjects;
using UnityEngine;
namespace Managers
{
    public class CardManager : MonoSingleton<CardManager>
    {
        public List<Card> allCards;
        
        #if UNITY_EDITOR
        [Button()]
        public void FetchCards()
        {
            allCards = Resources.LoadAll<Card>("Cards").ToList();
        }
        #endif

        public List<CardSlot> allSlots;
        public SelectedCard selectedCard;

        public const int MAXStamina=10;
        private int _stamina;
        public int Stamina
        {
            get => _stamina;
            set
            {
                _stamina = Mathf.Clamp(value,0,MAXStamina);
                UIManager.Instance.GameUI.SetStaminaBar(_stamina);
            }
        }

        public void SubscribeCardSlot(CardSlot slot)
        {
            allSlots ??= new List<CardSlot>();
            if (slot == null) return;
            if (allSlots.Contains(slot)) return;
            allSlots.Add(slot);
        }
        public void UnSubscribeSlot(CardSlot slot)
        {
            if (allSlots == null) return;
            if (!allSlots.Contains(slot)) return;
            allSlots.Remove(slot);
        }

        private void Start()
        {
            Stamina = 0;
            for (var index = 0; index < allSlots.Count; index++)
            {
                var slot = allSlots[index];
                slot.SetCard(allCards[index]);
            }
        }

        private float _time;
        [SerializeField] private float coolDown;
        
        private void Update()
        {
            selectedCard.gameObject.transform.position = Input.mousePosition;

            if (Stamina < MAXStamina)
            {
                if (_time >= coolDown)
                {
                    Stamina += 1;
                    _time = 0;
                }
                else
                {
                    _time += Time.deltaTime;
                }
            }
        }
    }   
}
