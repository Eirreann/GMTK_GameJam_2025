using System;
using UnityEngine;

public class Enemy_Base : MonoBehaviour
{
    [Header("Base Enemy")]
    [SerializeField] protected Transform rotationOrigin;
    [SerializeField] protected int damage;
    [SerializeField] protected int detectionRange;

    protected AudioSource audioSource;
    protected Quaternion startRot;
    protected PlayerController player;
    protected bool hasTarget = false;
    
    private const string TAG_PLAYER = "Player";

    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
        startRot = rotationOrigin.localRotation;
        GetComponent<SphereCollider>().radius = detectionRange;
    }

    protected virtual void Update()
    {
        hasTarget = player != null;
        lookAt(hasTarget ? player.transform : null);
    }

    protected virtual void lookAt(Transform target)
    {
        // Logic applied in child classes
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(TAG_PLAYER))
            player = other.GetComponent<PlayerController>();
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag(TAG_PLAYER))
            player = null;
    }
}
