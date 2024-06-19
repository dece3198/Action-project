using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;
    [SerializeField] private CinemachineFreeLook cinemachineFree;

    Camera cam;
    Vector3 originPos;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        cam = Camera.main;
        cinemachineFree = FindObjectOfType<CinemachineFreeLook>();
    }

    public void Shake()
    {
        StartCoroutine(ShakeCo(0.3f, 0.2f));
    }

    public IEnumerator ShakeCo(float duration, float magnitude)
    {
        float timer = 0;
        originPos = cam.transform.position;

        while (timer <= duration)
        {
            cinemachineFree.enabled = false;
            cam.transform.localPosition = Random.insideUnitSphere * magnitude + originPos;

            timer += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originPos;
        cinemachineFree.enabled = true;


    }
}
