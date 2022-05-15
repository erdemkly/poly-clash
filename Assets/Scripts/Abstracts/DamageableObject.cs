using Other;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Abstracts
{
    public abstract class DamageableObject : MonoBehaviour
    {
        public MySlider HealthBar { get; set; }

        private int _maxHealth;
        public int MaxHealth { get => _maxHealth;
            set
            {
                HealthBar.Slider.maxValue = value;
                _maxHealth = value;
            } 
        }

        private int _health;
        public int Health { get => _health;
            set
            {
                HealthBar.Slider.value = value;
                _health = value;
            } }
        public abstract void GetDamage(int damage);
        public abstract void OnDead();
    }
}
