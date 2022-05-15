using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Other
{
    public class MySlider : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI txtSlider;
        public Image fillImage;
        public Slider Slider => slider;

        public void SetSliderText()
        {
            txtSlider.text = $"{slider.value}";
        }
        
    }
}
