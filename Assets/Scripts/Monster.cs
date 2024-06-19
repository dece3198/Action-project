using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MonsterType
{
    Mob, MiddleBoss, Boss
}

public class Monster : MonoBehaviour
{
    public float maxHp;
    public float damage;
    public float pushPower;
    public float speed;

    public MonsterType type;

    public Animator animator;
    public ViewDetector viewDetector;
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    [SerializeField] protected GameObject canvas;
    [SerializeField] protected GameObject damageText;
    [SerializeField] protected Stack<GameObject> textStack = new Stack<GameObject>();
    public Slider slider;
    public SkinnedMeshRenderer skinnedMesh;
    public GameObject weapon;
    public GameObject weaponPosA;
    public GameObject weaponPosB;
    public Projector projectorA;
    public Projector projectorB;
    public Projector projectorC;

    public IEnumerator Skill;

    public bool isSkill;
    public bool isSword = false;
    public bool isAtk = false;
    public bool isAtkCool = true;
    public bool isDrive = true;
    public bool isGroggy = true;

    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject damage = Instantiate(damageText, canvas.transform);
            textStack.Push(damage);
            damage.SetActive(false);
            damage.transform.position = canvas.transform.position;
            damage.GetComponent<DamageText>().monster = this;
        }
    }

    public void ExitPool(float _damage)
    {
        GameObject damage = textStack.Pop();
        damage.transform.GetComponent<TextMeshProUGUI>().text = _damage.ToString();
        damage.SetActive(true);
    }

    public void EnterPool(GameObject _damageText)
    {
        textStack.Push(_damageText);
        _damageText.SetActive(false);
        _damageText.transform.position = canvas.transform.position;
    }
}
