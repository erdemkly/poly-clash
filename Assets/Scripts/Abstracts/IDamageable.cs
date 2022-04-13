using Other;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Abstracts
{
    public interface IDamageable
    {
        public MySlider HealthBar { get; set; }
        public int MaxHealth { get; set; }
        public int Health { get; set; }
        public void GetDamage(int damage);
        public void OnDead();
    }
}
