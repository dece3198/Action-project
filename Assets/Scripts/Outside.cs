using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outside : MonoBehaviour
{
    [SerializeField] private GameObject returnPos;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Monster>() != null)
        {
            other.transform.position = returnPos.transform.position;
        }
    }
}
