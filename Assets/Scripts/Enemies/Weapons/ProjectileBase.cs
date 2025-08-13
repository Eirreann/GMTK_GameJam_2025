using System;
using System.Collections;
using Player;
using UnityEngine;
using Utilities;

namespace Enemies.Weapons
{
    public class ProjectileBase : MonoBehaviour
    {
        [SerializeField] protected float _projectileSpeed = 5f;

        protected ProjectilePool pool;
        protected PlayerController target;
        protected int damage;

        private bool _isFired = false;
        private float _despawnTimer = 3f;
        
        private const float DESPAWN_TIME = 3f;

        public void Init(ProjectilePool pool)
        {
            this.pool = pool;
        }

        public virtual void FireAtTarget(PlayerController target, int damage)
        {
            this.target = target;
            this.damage = damage;
            _despawnTimer = DESPAWN_TIME;
            _isFired = true;
        }

        public virtual void ReturnToPool()
        {
            if (_isFired)
            {
                _isFired = false;
                pool.ReturnToPool(this);
            }
        }

        private void FixedUpdate()
        {
            if (_isFired)
            {
                transform.position += (transform.forward * _projectileSpeed) * Time.deltaTime;
                _despawnTimer -= Time.deltaTime;
                
                if(_despawnTimer <= 0)
                    ReturnToPool();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var hit = other.GetComponent(typeof(IDamageable));
            if (hit != null)
            {
                (hit as IDamageable).TakeDamage(damage);
                ReturnToPool();
            }
        }
    }
}