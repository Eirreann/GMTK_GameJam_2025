using System;
using UnityEngine;
using Utilities;

namespace Enemies
{
    public class MeleeWeapon : MonoBehaviour
    {
        private int _damage;
        
        private const float DAMAGE_COOLDOWN = 0.5f;
        
        private float _damageCooldownTimer = 0f;

        public void Init(int damage)
        {
            _damage = damage;
        }

        private void Update()
        {
            if (_damageCooldownTimer > 0)
            {
                _damageCooldownTimer -= Time.deltaTime;
                if(_damageCooldownTimer < 0)
                    _damageCooldownTimer = 0;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var hit = other.GetComponent(typeof(IDamageable));
            if (hit != null && _damageCooldownTimer <= 0)
            {
                (hit as IDamageable).TakeDamage(_damage);
                _damageCooldownTimer = DAMAGE_COOLDOWN;
            }
        }
    }
}