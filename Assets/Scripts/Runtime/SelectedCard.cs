using System;
using Helper;
using Managers;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime
{
    public class SelectedCard : CardUI
    {
        public CardSlot ownCardSlot;
        private Vector3 _placePosition;
        
        public void PlaceCard()
        {
            if (OwnCard == null) return;
            if (ownCardSlot == null) return;

            Instantiate(OwnCard.prefab, _placePosition, Quaternion.identity);
            CardManager.Instance.Stamina-=OwnCard.stamina;
            ownCardSlot.SetCard(null);
            OwnCard = null;
            CardManager.Instance.SetNextCard();
        }
        public void SetCard(Card card,CardSlot slot)
        {
            ownCardSlot = slot;
            SetCard(card);
        }
        public void ControlPlace()
        {
            if (OwnCard == null) return;
            if (CameraManager.Instance.ScreenToWorldRay(Input.mousePosition,1<<8, out var hit) && hit.collider.CompareTag("PlayerArea"))
            {
                cardImage.color = Color.green;
                _placePosition = hit.point.SetY(0);
            }
            else
            {
                cardImage.color = Color.red;
            }
        }
    }
}
