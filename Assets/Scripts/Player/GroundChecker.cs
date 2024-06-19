using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] private Transform point;
    [SerializeField, Range(0, 1)] private float distance;
    public bool IsGrounded { get; private set; }

    private void Update()
    {
        IsGrounded = Physics.Raycast(point.position, Vector3.down, distance, layerMask);
    }
}
