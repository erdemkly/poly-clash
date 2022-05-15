using System;
using System.Collections.Generic;
using System.Linq;
using Abstracts;
using Other;
using Runtime;
using Runtime.Castle;
using Runtime.Fighter;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public bool gameIsEnded;
        public UnityEvent onStartEvent,onWinEvent,onLoseEvent;
        [SerializeField]private GameObject playerAreaParticle;
        public Castle playerCastle, opponentCastle;
        public List<Fighter> playerFighters, opponentFighters;

        public void SetPlayerArea(bool active)
        {
            playerAreaParticle.SetActive(active);
        }
        private void Start()
        {
            Application.targetFrameRate = 60;
            StartEventHandle();
            playerFighters = new List<Fighter>();
            opponentFighters = new List<Fighter>();
        }

        public void EndGame()
        {
            gameIsEnded = true;

            var allFighters = playerFighters.Concat(opponentFighters).ToList();
            foreach (var fighter in allFighters)
            {
                fighter.Stop();
            }
        }
        public void StartEventHandle()
        {
            gameIsEnded = false;
            onStartEvent.Invoke();
        }
        public void WinEventHandle()
        {
            if (gameIsEnded) return;
            EndGame();
            onWinEvent.Invoke();
        }
        public void LoseEventHandle()
        {
            if (gameIsEnded) return;
            EndGame();
            onLoseEvent.Invoke();
        }

        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
