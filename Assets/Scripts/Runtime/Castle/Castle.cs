using System;
using Abstracts;
using DG.Tweening;
using Managers;
using Other;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Runtime.Castle
{
    public class Castle : DamageableObject
    {
    [SerializeField] private UnityEvent onDeadEvent;

    [SerializeField] private bool isPlayer;
        private void OnEnable()
        {
            HealthBar = UIManager.Instance.InformationUI.CreateSlider(transform,isPlayer);
            HealthBar.GetComponent<TransformFollower>().target = transform;
            MaxHealth = 1000;
            Health = MaxHealth;
        }
        public override void GetDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                OnDead();
            }
        }
        public override void OnDead()
        {
            transform.DOScale(0, 0.2f);
            onDeadEvent?.Invoke();
            Destroy(HealthBar.gameObject);
        }
    }
}
