using System.Collections;
using TMPro;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    public static TeleportManager instance;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] targetPos;
    [SerializeField] private TextMeshProUGUI stepText;
    private int step = 1;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        stepText.gameObject.SetActive(true);
        stepText.text = step.ToString() + "Step";
    }

    public IEnumerator Teleport()
    {
        yield return new WaitForSeconds(5f);
        player.transform.position = targetPos[step - 1].transform.position;
        step++;
        stepText.gameObject.SetActive(true);
        stepText.text = step.ToString() + "Step";
        yield return new WaitForSeconds(1);
        player.GetComponent<PlayerState>().Hp = player.GetComponent<PlayerState>().maxHp;
        player.GetComponent<PlayerState>().enabled = true;
        player.GetComponent<PlayerController>().enabled = true;
    }
}
