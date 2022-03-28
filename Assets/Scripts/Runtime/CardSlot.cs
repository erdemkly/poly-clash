using System;
using DG.Tweening;
using Managers;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Runtime
{
    public class CardSlot : CardUI,IPointerDownHandler,IDragHandler, IPointerUpHandler
    {
        [SerializeField] private GameObject disablePanel;
        private void OnEnable()
        {
            if (CardManager.Instance == null) return;
            CardManager.Instance.SubscribeCardSlot(this);
        }
        private void OnDisable()
        {
            if (CardManager.Instance == null) return;
            CardManager.Instance.UnSubscribeSlot(this);
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (InputManager.Instance.CurrentStateName != "CardState") return;
            if (OwnCard == null) return;
            if (CardManager.Instance.Stamina < OwnCard.stamina) return;
            DOTween.Rewind("card");
            cardObj.transform.DOLocalMoveY(20f, 0.1f).SetLoops(2,LoopType.Yoyo).SetId("card");
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (InputManager.Instance.CurrentStateName != "CardState") return;
            if (OwnCard == null) return;
            if (cardObj.activeSelf)
            {
                CardManager.Instance.selectedCard.SetCard(OwnCard, this);
                cardObj.SetActive(false);
            }
            else
            {
                if (CardManager.Instance.selectedCard.ownCardSlot == this)
                {
                    CardManager.Instance.selectedCard.ControlPlace();
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (InputManager.Instance.CurrentStateName != "CardState") return;
            if (CameraManager.Instance.ScreenToWorldRay(eventData.position,1<<8,out var hit) && hit.collider.CompareTag("PlayerArea"))
            {
                CardManager.Instance.selectedCard.PlaceCard();
            }
            else
            {
                cardObj.SetActive(true);
            }
            CardManager.Instance.selectedCard.SetCard(null,null);
        }
        private void Update()
        {
            if (OwnCard == null) return;
            disablePanel.SetActive(CardManager.Instance.Stamina < OwnCard.stamina);
        }
    }
}
