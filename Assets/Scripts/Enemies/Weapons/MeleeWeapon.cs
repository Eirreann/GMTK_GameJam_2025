using System;
using System.Collections.Generic;
using Game;
using UnityEngine;
using Utilities;

namespace Enemies.Weapons
{
    public class MeleeWeapon : MonoBehaviour
    {
        [SerializeField] private bool _hasKnockback = false;
        [SerializeField] private float _knockbackForce = 30f;
        
        private const float DAMAGE_COOLDOWN = 0.5f;
        
        private int _damage;
        private List<IDamageable> _hitList = new List<IDamageable>();
        
        public void Init(int damage)
        {
            _damage = damage;
        }

        private void OnTriggerEnter(Collider other)
        {
            var hit = other.GetComponent(typeof(IDamageable));
            if (hit != null)
            {
                var target = hit as IDamageable;
                if (!_hitList.Contains(target))
                {
                    target.TakeDamage(_damage);
                    _hitList.Add(hit as IDamageable);
                    
                    if (other.CompareTag("Player") && _hasKnockback)
                    {
                        var dir = transform.forward;
                        var force = dir * _knockbackForce;
                        force.y = 0;
                        GameManager.Instance.Player.playerMovement.SetKnockback(force);
                    }
                    else if (other.CompareTag("PlayerWall"))
                    {
                        
                    }
                }
            }
        }

        private void OnDisable()
        {
            _hitList.Clear();
        }
    }
}