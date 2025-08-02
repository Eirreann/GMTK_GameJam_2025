using System;
using Game;
using UnityEngine;
using Utilities;

namespace Enemies
{
    public class MeleeWeapon : MonoBehaviour
    {
        [SerializeField] private bool _hasKnockback = false;
        [SerializeField] private float _knockbackForce = 30f;
        
        private const float DAMAGE_COOLDOWN = 0.5f;
        
        private int _damage;
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
                if (other.CompareTag("Player") && _hasKnockback)
                {
                    var dir = transform.parent.forward;
                    var force = dir * _knockbackForce;
                    force.y = 0;
                    GameManager.Instance.Player.playerMovement.SetKnockback(force);
                }
                
                _damageCooldownTimer = DAMAGE_COOLDOWN;
            }
        }
    }
}