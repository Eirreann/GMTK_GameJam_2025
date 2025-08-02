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
                _anim.SetBool("isWalking", true);
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance > DISTANCE_TO_ATTACK)
                {
                    if (IsTrapped)
                    {
                        if (Physics.Raycast(transform.position, player.transform.position - transform.position,
                                out RaycastHit hit, distance) && hit.collider.CompareTag("PlayerWall"))
                        {
                            float distanceToWall = Vector3.Distance(transform.position, hit.transform.position);
                            if (distanceToWall > (DISTANCE_TO_ATTACK + 0.5f))
                            {
                                //Debug.DrawRay(transform.position + Vector3.up, player.transform.position - hit.transform.position, Color.red);
                                _agent.isStopped = false;
                                _agent.SetDestination((hit.transform.position));
                            }
                            else
                                _attack();
                        }
                    }
                    else
                    {
                        _agent.isStopped = false;
                        _agent.SetDestination((player.transform.position));
                        _runCooldown();
                    }
                }
                else
                    _attack();
            }
            else
            {
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

        protected override void lookAt(Transform target)
        {
            // NavMeshAgent does this for us, yay ^_^
        }

        private void _attack()
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