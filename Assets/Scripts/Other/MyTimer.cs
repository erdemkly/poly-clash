using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Other
{
    public class MyTimer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtTimer;
        private float _time;
        [SerializeField]private float countDown;
        public UnityEvent onTimeOver;
        private bool _isEnded;

        private void FixedUpdate()
        {
            if (_isEnded) return;
            if (countDown <= 0)
            {
                _isEnded = true;
                onTimeOver?.Invoke();
                return;
            }
            if (_time <= 0)
            {
                countDown--;
                var minutes = Mathf.FloorToInt(countDown / 60);
                var seconds = Mathf.FloorToInt(countDown % 60);
                txtTimer.text = $"{minutes:00}:{seconds:00}";
                _time = 1;
            }
            else
            {
                _time -= Time.fixedDeltaTime;
            }
            
        }

    }
}
