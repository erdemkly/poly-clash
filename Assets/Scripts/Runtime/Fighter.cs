using Abstracts;
using Managers;
using Other;
using UnityEngine;
using UnityEngine.Animations;

namespace Runtime
{
    public class Fighter : MonoBehaviour, IDamageable
    {
        private MySlider _healthBar;
        private int _maxHealth;
        private int _health;
        public float Damage;

        public void InitializeFighter(int maxHealth, float damage)
        {
            _healthBar = UIManager.Instance.InformationUI.CreateSlider(transform);
            _healthBar.GetComponent<TransformFollower>().target = transform;
            MaxHealth = maxHealth;
            Damage = damage;
            Health = maxHealth;
        }

        public MySlider HealthBar
        {
            get => _healthBar;
            set => _healthBar = value;
        }

        public int MaxHealth
        {
            get => _maxHealth;
            set
            {
                _healthBar.Slider.maxValue = value;
                _maxHealth = value;
            }
        }

        public int Health
        {
            get => _health;
            set
            {
                _healthBar.Slider.value = value;
                _health = value;
            }
        }


        private void OnEnable()
        {
        }


        public void GetDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                OnDead();
            }
        }

        public void OnDead()
        {
        }
    }
}