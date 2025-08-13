using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using Enemies.Weapons;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    public class Enemy_Melee : Enemy_Base
    {
        [Header("Melee")]
        [SerializeField] protected float attackSpd = 3f;
        [SerializeField] protected float distToAttack = 1.5f;
        [SerializeField] protected MeleeWeapon weapon;
        [SerializeField] protected AnimationClip attackAnim;
        [SerializeField] protected PatrolRoute patrolRoute;

        [Header("Audio")]
        [SerializeField] protected AudioClip onDetect;
        [SerializeField] protected AudioClip onAttack;
        [SerializeField] protected List<AudioClip> footsteps;
        
        private Animator _anim;
        private NavMeshAgent _agent;
        private float _attackCooldown = 0f;
        private bool _isPursuing = false;
        private int _footstepIndex = 0;

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
                if (!_isPursuing)
                {
                    if(onDetect != null)
                        audioSource.PlayOneShot(onDetect);
                    _isPursuing = true;
                }
                
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance > distToAttack)
                {
                    if (IsTrapped)
                    {
                        if (Physics.Raycast(transform.position, player.transform.position - transform.position,
                                out RaycastHit hit, distance) && hit.collider.CompareTag("PlayerWall"))
                        {
                            float distanceToWall = Vector3.Distance(transform.position, hit.transform.position);
                            if (distanceToWall > (distToAttack + 0.5))
                            {
                                //Debug.DrawRay(transform.position + Vector3.up, player.transform.position - hit.transform.position, Color.red);
                                _agent.isStopped = false;
                                _anim.SetBool("isWalking", true);
                                _agent.SetDestination((hit.transform.position));
                            }
                            else
                                _attack();
                        }
                    }
                    else
                    {
                        _agent.isStopped = false;
                        _anim.SetBool("isWalking", true);
                        _agent.SetDestination((player.transform.position));
                        _runCooldown();
                    }
                }
                else
                    _attack();
            }
            else
            {
                _isPursuing = false;
                if(IsTrapped)
                    _agent.isStopped = true;
                else
                    patrolRoute.Patrol(_agent, _anim);
                _runCooldown();
            }
        }

        public override void ResetEnemy()
        {
            base.ResetEnemy();
            _agent.isStopped = true;
            transform.position = startPos;
        }

        public void PlayFootstep()
        {
            audioSource.PlayOneShot(footsteps[_footstepIndex]);
            _footstepIndex++;
            if(_footstepIndex >= footsteps.Count)
                _footstepIndex = 0;
        }

        protected override void lookAt(Transform target)
        {
            // NavMeshAgent does this for us, yay ^_^
        }

        private void _attack()
        {
            if (_attackCooldown == 0)
            {
                _attackCooldown = attackSpd;
                
                _anim.SetBool("isWalking", false);
                
                _anim.Play(attackAnim.name);
                
                if(onAttack != null)
                    audioSource.PlayOneShot(onAttack);
            }
            else
            {
                _runCooldown();
            }
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