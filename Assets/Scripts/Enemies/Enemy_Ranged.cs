using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class Enemy_Ranged : Enemy_Base
{
    [Header("Ranged")]
    [SerializeField] protected float fireRate;
    [SerializeField] protected Transform gunRotation;
    [SerializeField] protected List<Transform> bulletSpawnPos;
    [SerializeField] protected AnimationClip idleAnim;
    
    [Header("Audio")]
    [SerializeField] protected AudioClip onFire;
    
    protected bool isFiring = false;
    
    private Animator _anim;
    private Quaternion _startGunRot;
    private ProjectilePool _pool;
    private float _fireCooldown = 0;
    
    private void Start()
    {
        _anim = GetComponent<Animator>();
        _pool = GetComponent<ProjectilePool>();
        
        _startGunRot = gunRotation.localRotation;
        _pool.Setup(bulletSpawnPos);
    }
    
    protected override void Update()
    {
        base.Update();
        if (hasTarget)
            _fireWeapon();
        else
        {
            if (_fireCooldown > 0)
            {
                _fireCooldown -= Time.deltaTime;
                if (_fireCooldown < 0)
                    _fireCooldown = 0;
            }
        }
    }

    public override void ResetEnemy()
    {
        base.ResetEnemy();
        _pool.ResetPool();
    }

    protected override void lookAt(Transform target)
    {
        if (target != null)
        {
            _anim.enabled = false;
            // Rotate turret base
            rotationOrigin.LookAt(target.transform);
            rotationOrigin.localRotation = Quaternion.Euler(startRot.eulerAngles.z, rotationOrigin.localRotation.eulerAngles.y, startRot.eulerAngles.z);

            // Rotate gun
            gunRotation.LookAt(target.transform);
            gunRotation.localRotation = Quaternion.Euler(gunRotation.localRotation.eulerAngles.x, _startGunRot.eulerAngles.y, _startGunRot.eulerAngles.z);
        }
        else
        {
            rotationOrigin.localRotation = startRot;
            _anim.enabled = true;
            _anim.Play(idleAnim.name);
        }
    }

    private void _fireWeapon()
    {
        if (_fireCooldown == 0)
        {
            _fireCooldown = fireRate;
            _spawnBullet(damage);
        }
        else
        {
            _fireCooldown -= Time.deltaTime;
            if (_fireCooldown < 0)
                _fireCooldown = 0;
        }
    }

    private void _spawnBullet(int dmg)
    {
        ProjectileBase bullet = _pool.GetProjectile();
        bullet.FireAtTarget(player, damage);
        
        if(onFire != null)
            audioSource.PlayOneShot(onFire);
    }
}
