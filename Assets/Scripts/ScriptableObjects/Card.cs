using Managers;
using UnityEngine;
namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Card",menuName = "Card Game/New Card")]
    public class Card : ScriptableObject
    {
        public string id;
        public Sprite sprite;
        [Range(1,CardManager.MAXStamina)]public int stamina;
        public int damage;
        public GameObject prefab;
        [Range(0,500)]public int health;
    }
}
