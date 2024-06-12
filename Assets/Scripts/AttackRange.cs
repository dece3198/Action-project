using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    [SerializeField] private Projector projector;
    [SerializeField] private Projector projectorB;
    [SerializeField] private float time;

    private void Start()
    {
        StartCoroutine(RunAttack2());
    }

    private IEnumerator RunAttack1()
    {
        float runningTime = time;

        projector.gameObject.SetActive(true);
        projector.orthographicSize = 1.0f;

        while (runningTime > .0f)
        {
            runningTime -= Time.deltaTime;
            projector.orthographicSize += 3.2f * Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator RunAttack2()
    {
        float runningTime = time;

        projectorB.gameObject.SetActive(true);
        projectorB.orthographicSize = 0.01f;
        projectorB.aspectRatio = 5;

        while (runningTime > .0f)
        {
            runningTime -= Time.deltaTime;
            projectorB.orthographicSize += 2.0f * Time.deltaTime;
            projectorB.aspectRatio += 1f * Time.deltaTime;
            yield return null;
        }
    }
}