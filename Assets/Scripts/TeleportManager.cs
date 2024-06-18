using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    public static TeleportManager instance;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] targetPos;
    [SerializeField] private GameObject stepText;
    private int step = 1;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        stepText.SetActive(true);
    }

    public IEnumerator Teleport()
    {
        yield return new WaitForSeconds(5f);
        player.transform.position = targetPos[step - 1].transform.position;
        yield return new WaitForSeconds(1);
        player.GetComponent<PlayerState>().enabled = true;
        player.GetComponent<PlayerController>().enabled = true;
    }
}
