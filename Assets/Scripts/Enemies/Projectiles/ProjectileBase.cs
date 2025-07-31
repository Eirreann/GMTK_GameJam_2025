using System;
using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class ProjectileBase : MonoBehaviour
    {
        [SerializeField] protected float _projectileSpeed = 5f;
        [SerializeField] protected float _impactDistance = 0.5f;

        protected ProjectilePool pool;
        protected PlayerController target;
        protected int damage;

        private bool _isFired = false;
        private float _despawnTimer = 3f;
        
        private const string TAG_PLAYER = "Player";

        public void Init(ProjectilePool pool)
        {
            this.pool = pool;
        }

        public virtual void FireAtTarget(PlayerController target, int damage)
        {
            this.target = target;
            this.damage = damage;
            _isFired = true;
            
            //_fireCoroutine = StartCoroutine(_moveTowardTarget());
        }

        protected virtual void Update()
        {
            // if(_target == null && _isFired)
            // {
            //     StopAllCoroutines();
            //     _isFired = false;
            //     _pool.ReturnToPool(this);
            // }

            if (_isFired)
            {
                transform.position += (transform.forward * _projectileSpeed) * Time.deltaTime;
                _despawnTimer -= Time.deltaTime;
                
                if(_despawnTimer <= 0)
                    _returnToPool();
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TAG_PLAYER))
            {
                var player = other.GetComponent<PlayerController>();
                player.TakeDamage(damage);
                _returnToPool();
            }
        }

        protected virtual void _returnToPool()
        {
            _isFired = false;
            pool.ReturnToPool(this);
        }

        protected virtual IEnumerator _moveTowardTarget()
        {
            while(Vector3.Distance(transform.position, target.transform.position) > _impactDistance)
            {
                //Debug.Log(Vector3.Distance(transform.position, _target.position));
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, _projectileSpeed * Time.deltaTime);
                transform.LookAt(target.transform);

                yield return null;
            }

            target.TakeDamage(damage);
            _returnToPool();
        }
    }
}