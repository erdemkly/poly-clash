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
    public class CardSlot : CardUI,IPointerDownHandler,IPointerUpHandler,IDragHandler
    {
        [SerializeField] private GameObject disablePanel;
        private bool isSelected;
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
            if (OwnCard == null) return;
            if (isSelected) return;
            if (CardManager.Instance.Stamina < OwnCard.stamina) return;
            isSelected = true;
            cardObj.transform.DOLocalMoveY(20f, 0.1f);
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            cardObj.transform.localPosition = Vector3.zero;
            if (OwnCard == null) return;
            if (CardManager.Instance.selectedCard.OwnCard != OwnCard) return;
            CardManager.Instance.Stamina-=OwnCard.stamina;
            CardManager.Instance.selectedCard.SetCard(null);
            cardObj.SetActive(true);
            isSelected = false;
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (!isSelected) return;
            if (OwnCard == null) return;
            CardManager.Instance.selectedCard.SetCard(OwnCard);
            cardObj.SetActive(false);
        }

        private void Update()
        {
            disablePanel.SetActive(CardManager.Instance.Stamina < OwnCard.stamina);
        }
    }
}
