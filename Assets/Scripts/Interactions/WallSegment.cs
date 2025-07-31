using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WallSegment : MonoBehaviour
{
    [SerializeField] private Material _builtWall;

    private void Start()
    {
        StartCoroutine(_buildAfterDelay());
    }

    private IEnumerator _buildAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<NavMeshObstacle>().enabled = true;
        GetComponent<MeshRenderer>().material = _builtWall;
    }
}
