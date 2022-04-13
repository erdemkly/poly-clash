using System;
using Other;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public UnityEvent onStartEvent;
        [SerializeField]private GameObject playerAreaParticle;

        public void SetPlayerArea(bool active)
        {
            playerAreaParticle.SetActive(active);
        }
        private void Start()
        {
            Application.targetFrameRate = 60;
            StartEventHandle();
        }
        public void StartEventHandle()
        {
            onStartEvent.Invoke();
        }
    }
}
