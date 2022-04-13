using System;
using Abstracts;
using Managers;
using Other;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Castle
{
    public class Castle : MonoBehaviour, IDamageable
    {
        private int health;
        [SerializeField]private int maxHealth;
        private MySlider healthBar;

        public MySlider HealthBar
        {
            get => healthBar;
            set => healthBar = value;
        }
        public int MaxHealth
        {
            get => maxHealth;
            set
            {
                healthBar.Slider.maxValue = MaxHealth;
                maxHealth = value;
            }
        }
        public int Health
        {
            get => health;
            set
            {
                healthBar.Slider.value = value;
                health = value;
            }
        }
        private void OnEnable()
        {
            healthBar = UIManager.Instance.InformationUI.CreateSlider(transform);
            MaxHealth = maxHealth;
            Health = MaxHealth;
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
            Debug.Log("Aaaaa");
        }
    }
}
