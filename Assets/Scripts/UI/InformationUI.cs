using Helper;
using Other;
using UnityEngine;

namespace UI
{
    public class InformationUI : MonoBehaviour
    {
        [SerializeField] private GameObject sliderPrefab;

        public MySlider CreateSlider(Transform t,bool isPlayer)
        {
            var tr = transform;
            var slider =  Instantiate(sliderPrefab, t.position.SetY(t.position.y+2), tr.rotation, tr).GetComponent<MySlider>();
            slider.fillImage.color = isPlayer ? Color.green : Color.red;
            return slider;
        }
    }
}
