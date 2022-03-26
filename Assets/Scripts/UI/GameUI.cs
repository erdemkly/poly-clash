using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private Slider staminaSlider;
        [SerializeField] private TextMeshProUGUI txtStamina;
        public void SetStaminaBar(int value)
        {
            value = (int)Mathf.Clamp(value, 0, staminaSlider.maxValue);
            staminaSlider.DOValue(value, 0.1f);
            txtStamina.text = $"{value}";
        }
    }
}
