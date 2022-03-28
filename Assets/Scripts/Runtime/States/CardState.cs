using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Runtime.States
{
    public class CardState : State
    {
        public override void OnPointerDown(PointerEventData eventData)
        {
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            
        }
        public override void OnDrag(PointerEventData eventData)
        {
        }
        public override void OnEndDrag(PointerEventData eventData)
        {
        }
        public override void OnStart()
        {
            Debug.Log("Enter CardState");
        }
        public override void OnUpdate()
        {
        }
        public override void OnExit()
        {
        }
    }
}
