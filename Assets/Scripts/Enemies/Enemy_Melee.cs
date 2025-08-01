using System;
using System.Collections;
using AI;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    public class Enemy_Melee : Enemy_Base
    {
        [Header("Melee")]
        [SerializeField] protected float attackSpd = 3f;
        [SerializeField] protected MeleeWeapon weapon;
        [SerializeField] protected AnimationClip attackAnim;
        [SerializeField] protected PatrolRoute patrolRoute;
        
        [Header("Audio")]
        [SerializeField] protected AudioClip onAttack;
        
        private const float DISTANCE_TO_ATTACK = 1.5f;
        
        private Animator _anim;
        private NavMeshAgent _agent;
        private float _attackCooldown = 0f;

        protected override void Start()
        {
            base.Start();
            _anim = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
            
            weapon.Init(damage);
        }

        private void FixedUpdate()
        {
            if (hasTarget)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance > DISTANCE_TO_ATTACK)
                {
                    _agent.isStopped = false;
                    _agent.SetDestination((player.transform.position));
                    _runCooldown();
                }
                else
                    _attackPlayer();
            }
            else
            {
                _agent.isStopped = true;
                patrolRoute.Patrol(_agent);
                // TODO: Return to patrol route

                _runCooldown();
            }
        }

        protected override void lookAt(Transform target)
        {
            // NavMeshAgent does this for us, yay ^_^
        }

        private void _attackPlayer()
        {
            if (_attackCooldown == 0)
            {
                _attackCooldown = attackSpd;
                StartCoroutine(_swingWeapon());
                
                if(onAttack != null)
                    audioSource.PlayOneShot(onAttack);
            }
            else
            {
                _runCooldown();
            }
        }

        private IEnumerator _swingWeapon()
        {
            weapon.transform.parent.gameObject.SetActive(true); // TODO: Update with real model
            _anim.Play(attackAnim.name);
            yield return new WaitForSeconds(attackAnim.length);
            weapon.transform.parent.gameObject.SetActive(false);
        }

        private void _runCooldown()
        {
            if (_attackCooldown > 0)
            {
                _attackCooldown -= Time.deltaTime;
                if (_attackCooldown < 0)
                    _attackCooldown = 0;
            }
        }
    }
}