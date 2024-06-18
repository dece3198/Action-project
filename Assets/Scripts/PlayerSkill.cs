using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkill : MonoBehaviour
{
    private float speed;

    [SerializeField] private ViewDetector viewDetector;
    [SerializeField] private PlayerController controller;
    [SerializeField] private PlayerState state;

    [SerializeField] private float qSkillCool;
    private float qSkillMax;
    [SerializeField] private Image qSkillImage;
    [SerializeField] private GameObject qSkillPos;
    [SerializeField] private GameObject qSkillObj;
    [SerializeField] private ParticleSystem qSkillParticle;

    [SerializeField] private float eSkillCool;
    private float eSkillMax;
    [SerializeField] private Image eSkillImage;
    [SerializeField] private ParticleSystem eSkillParticle;

    private bool isQSkill = true;
    private bool isESkill = true;   

    private void Awake()
    {
        viewDetector = GetComponent<ViewDetector>();
        controller = GetComponent<PlayerController>();
        state = GetComponent<PlayerState>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(isQSkill)
            {
                if (state.isAtk)
                {
                    if (controller.moveSpeed > 0)
                    {
                        speed = controller.moveSpeed;
                        controller.moveSpeed = 0;
                        state.animator.Play("QSkill");
                        StartCoroutine(QSkillCo());
                    }
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            if(isESkill)
            {
                if (state.isAtk)
                {
                    if(controller.moveSpeed > 0)
                    {
                        speed = controller.moveSpeed;
                        controller.moveSpeed = 0;
                        StartCoroutine(ESkillCo());
                        StartCoroutine(ESkill());
                    }
                }
            }
        }
    }


    public IEnumerator QSkill()
    {
        state.damage = 40;
        state.pushPower = 3;
        qSkillParticle.gameObject.SetActive(true);
        qSkillParticle.Play();
        qSkillParticle.transform.position = qSkillPos.transform.position;
        qSkillParticle.transform.rotation = qSkillPos.transform.rotation;
        float time = 0.7f;
        while (time > 0)
        {
            time -= Time.deltaTime;
            qSkillObj.GetComponent<ViewDetector>().FindRangeTarget(state.damage,state.pushPower);
            qSkillObj.transform.Translate(Vector3.forward * 15 * Time.deltaTime);
            yield return null;
        }
        controller.moveSpeed = speed;
        qSkillObj.transform.position = qSkillPos.transform.position;

    }

    private IEnumerator QSkillCo()
    {
        qSkillMax = qSkillCool;
        isQSkill = false;
        while (qSkillCool > 0)
        {
            qSkillCool -= Time.deltaTime;
            qSkillImage.fillAmount = qSkillCool / qSkillMax;
            yield return null;
        }
        qSkillCool = qSkillMax;
        isQSkill = true;
    }

    public IEnumerator ESkill()
    {
        state.animator.Play("ESkill");
        state.damage = 10;
        state.pushPower = 0;
        eSkillParticle.gameObject.SetActive(true);
        eSkillParticle.Play();
        for(int i = 0; i < 10; i++)
        {
            viewDetector.FindRangeTarget(state.damage,state.pushPower);
            yield return new WaitForSeconds(0.2f);
        }
        controller.moveSpeed = speed;
        eSkillParticle.gameObject.SetActive(false);
    }

    private IEnumerator ESkillCo()
    {
        eSkillMax = eSkillCool;
        isESkill = false;
        while (eSkillCool > 0)
        {
            eSkillCool -= Time.deltaTime;
            eSkillImage.fillAmount = eSkillCool / eSkillMax;
            yield return null;
        }
        eSkillCool = eSkillMax;
        isESkill = true;
    }
}
