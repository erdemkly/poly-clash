using System;
using System.Collections;
using Helper;
using Other;
using Runtime.Fighter;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class OpponentManager : MonoSingleton<OpponentManager>
    {
        [SerializeField] private BoxCollider boxCollider;
        private void Start()
        {
            GameManager.Instance.onStartEvent.AddListener(() =>
            {
                StartCoroutine(CardPlaceSequence());
            });
        }

        private void PlaceCard()
        {
            var randomCard = RandomItemGeneric<Card>.GetRandom(CardManager.Instance.allCards.ToArray());
            var fighter = Instantiate(randomCard.prefab, GetRandomPoint(), Quaternion.identity).GetComponent<Fighter>();
            fighter.InitializeFighter(randomCard,false);
        }

        private Vector3 GetRandomPoint()
        {
            var bounds = boxCollider.bounds;
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }
        private IEnumerator CardPlaceSequence()
        {
            while (!GameManager.Instance.gameIsEnded)
            {
                yield return new WaitForSeconds(Random.Range(3, 7));
                PlaceCard();
            }
        }
    }
}
