using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Utilities;

public class WallSegment : MonoBehaviour, IDamageable
{
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private Material _builtWall;
    [SerializeField] private Material _hitMat;

    private const float DAMAGE_MODIFIER = 0.75f;
    
    private UnityAction<int> _onHit;
    private bool _isHittable = false;

    public void SetOnHitCallback(UnityAction<int> callback)
    {
        _onHit = callback;
    }

    public void TakeDamage(int damage)
    {
        int modifiedDamage = Convert.ToInt32(damage * DAMAGE_MODIFIER);
        
        if(_isHittable)
            _onHit.Invoke(modifiedDamage);
        
        StartCoroutine(_flashOnHit());
    }

    public void BuildAfterDelay()
    {
        _renderer.material = _builtWall;
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<NavMeshObstacle>().enabled = true;
        
        _isHittable = true;
    }

    private IEnumerator _flashOnHit()
    {
        _renderer.material = _hitMat;
        
        yield return new WaitForSeconds(0.1f);
        
        _renderer.material = _builtWall;
    }
}
