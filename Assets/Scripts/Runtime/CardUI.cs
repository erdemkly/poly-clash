using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime
{
    public class CardUI : MonoBehaviour
    {
        private Card _ownCard;
        public Card OwnCard
        {
            get => _ownCard;
            set
            {
                _ownCard = value;
                cardObj.SetActive(OwnCard!=null);
            }
        }
        [SerializeField] protected GameObject cardObj;
        [SerializeField] protected Image cardImage;
        [SerializeField] protected TextMeshProUGUI txtStamina;
        
        
        public void SetCard(Card card)
        {
            OwnCard = card;
            if (card == null) return;
            cardImage.sprite = card.sprite;
            txtStamina.text = $"{card.stamina}";
        }
    }
}
