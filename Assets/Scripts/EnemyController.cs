using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyState
{
    Idle, Walk, Run, Attack, Hit, Groggy, SkillA, SkillB, SkillC    
}

public class EnemyIdle : BaseState<EnemyController>
{
    float time;
    
    public override void Enter(EnemyController player)
    {
        player.isSkillB = false;
        player.animator.SetBool("Walk", false);
        time = 0;

        if(!player.isSword)
        {
            player.animator.Play("On");
            player.isSword = true;
        }
    }

    public override void Exit(EnemyController player)
    {
    }

    public override void Update(EnemyController player)
    {
        player.viewDetector.FindTarget();

        if(player.katanaPosA.transform.childCount < 0)
        {
            time += Time.deltaTime;

            if (time > 8f)
            {
                player.animator.Play("Off");
                player.isSword = false;
                time = 0;
            }
        }


        if (player.viewDetector.Target != null)
        {
            player.ChangeState(EnemyState.Walk);
        }
    }
}

public class EnemyWalk : BaseState<EnemyController>
{
    public override void Enter(EnemyController player)
    {
        player.speed = 1;
        player.animator.SetBool("Walk", true);
    }

    public override void Exit(EnemyController player)
    {
    }

    public override void Update(EnemyController player)
    {
        player.viewDetector.FindTarget();
        if (player.viewDetector.Target != null)
        {
            Vector3 dir = player.viewDetector.Target.transform.position - player.transform.position;
            dir.Normalize();
            player.transform.position = Vector3.MoveTowards(player.transform.position, player.viewDetector.Target.transform.position, player.speed * Time.deltaTime);
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 6);
        }
        else
        {
            player.ChangeState(EnemyState.Idle);
        }
    }
}

public class EnemyRun : BaseState<EnemyController>
{
    float time;
    public override void Enter(EnemyController player)
    {
        time = 0.5f;
        player.speed = 30;
        player.StartCoroutine(RunAttack1(player));
    }

    public override void Exit(EnemyController player)
    {
    }

    public override void Update(EnemyController player)
    {
        player.IdleOn();
    }

    private IEnumerator RunAttack1(EnemyController player)
    {
        float runningTime = time;
        player.skillB.gameObject.SetActive(true);
        player.skillB.Play();
        while (runningTime > 0f)
        {
            if(player.isDrive)
            {
                runningTime -= Time.deltaTime;
                player.transform.Translate(Vector3.forward * player.speed * Time.deltaTime);
                yield return null;
            }
            else
            {
                player.skillB.gameObject.SetActive(false);
                player.animator.SetBool("SkillB", false);
                yield return null ;
            }
        }
        player.skillB.gameObject.SetActive(false);
        player.animator.SetBool("SkillB", false);
    }

}

public class EnemyAttack : BaseState<EnemyController>
{
    public override void Enter(EnemyController player)
    {
        player.animator.Play("Slash");
    }

    public override void Exit(EnemyController player)
    {
    }

    public override void Update(EnemyController player)
    {
        player.IdleOn();
    }
}

public class EnemyHit : BaseState<EnemyController>
{
    public override void Enter(EnemyController player)
    {
        player.StartCoroutine(HitCo(player));
        if(player.animator.GetCurrentAnimatorStateInfo(0).IsName("On"))
        {
            return;
        }
        player.animator.Play("Hit");
        
    }

    public override void Exit(EnemyController player)
    {
    }

    public override void Update(EnemyController player)
    {
        player.IdleOn();
    }

    private IEnumerator HitCo(EnemyController player)
    {
        player.mat.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        player.mat.color = Color.white;
    }
}

public class EnemyGroggy : BaseState<EnemyController>
{
    public override void Enter(EnemyController player)
    {


    }

    public override void Exit(EnemyController player)
    {
    }

    public override void Update(EnemyController player)
    {
        player.IdleOn();
    }
}

public class EnemySkillA : BaseState<EnemyController>
{
    float time;
    public override void Enter(EnemyController player)
    {
        time = 1f;
        player.isSkill = true;
        player.damage = 10f;
        player.viewDetector.rangeAngle = 44;
        player.StartCoroutine(RunAttack1(player));
        player.animator.SetBool("Walk", false);
        player.animator.Play("SkillA");
    }

    public override void Exit(EnemyController player)
    {
    }

    public override void Update(EnemyController player)
    {
        player.viewDetector.FindPlayerTarget();
    }

    private IEnumerator RunAttack1(EnemyController player)
    {
        float runningTime = time;

        player.projectorA.gameObject.SetActive(true);
        player.projectorA.orthographicSize = 1.0f;

        while (runningTime > 0f)
        {
            runningTime -= Time.deltaTime;
            player.projectorA.orthographicSize += 10f * Time.deltaTime;
            yield return null;
        }

        player.skillA.gameObject.SetActive(true);
        player.skillA.Play();

        if (player.viewDetector.RangeTarget != null)
        {
            player.viewDetector.FindPlayerTarget();
            player.viewDetector.RangeTarget.GetComponent<IInteractable>()?.TakeHit(player.damage);
        }

        player.projectorA.gameObject.SetActive(false);
        player.isSkill = false;
        player.ChangeState(EnemyState.Idle);
    }
}

public class EnemySkillB : BaseState<EnemyController>
{
    float time;

    public override void Enter(EnemyController player)
    {
        time = 1f;
        player.damage = 20f;
        player.isSkill = true;
        player.isSkillB = true;
        player.StartCoroutine(RunAttack1(player));
        player.animator.SetBool("Walk", false);
        player.animator.Play("SkillB");
        player.animator.SetBool("SkillB",true);
    }

    public override void Exit(EnemyController player)
    {
    }

    public override void Update(EnemyController player)
    {
        player.viewDetector.FindPlayerTarget();
    }

    private IEnumerator RunAttack1(EnemyController player)
    {
        float runningTime = time;

        player.projectorB.gameObject.SetActive(true);
        player.projectorB.orthographicSize = 0.01f;
        player.projectorB.aspectRatio = 5f;

        while (runningTime > .0f)
        {
            runningTime -= Time.deltaTime;
            player.projectorB.orthographicSize += 3f * Time.deltaTime;
            player.projectorB.aspectRatio += 3f * Time.deltaTime;
            yield return null;
        }

        player.projectorB.gameObject.SetActive(false);
        player.isSkill = false;
        player.ChangeState(EnemyState.Run);
    }
}

public class EnemySkillC : BaseState<EnemyController>
{
    float time;

    public override void Enter(EnemyController player)
    {
        time = 2f;
        player.damage = 4f;
        player.isSkill = true;
        player.viewDetector.rangeAngle = 360;
        player.StartCoroutine(RunAttack1(player));
        player.animator.SetBool("Walk", false);
        player.animator.Play("SkillC");
    }

    public override void Exit(EnemyController player)
    {
    }

    public override void Update(EnemyController player)
    {
        player.viewDetector.FindPlayerTarget();
    }

    private IEnumerator RunAttack1(EnemyController player)
    {
        float runningTime = time;

        player.projectorC.gameObject.SetActive(true);
        player.projectorC.orthographicSize = 0.01f;
        player.startSkillC.gameObject.SetActive(true);
        player.startSkillC.Play();

        while (runningTime > .0f)
        {
            runningTime -= Time.deltaTime;
            player.projectorC.orthographicSize += 7.5f * Time.deltaTime;
            yield return null;
        }

        player.endSkillC.gameObject.SetActive(true);
        player.endSkillC.Play();

        for(int i = 0; i < 10; i++)
        {
            if (player.viewDetector.RangeTarget != null)
            {
                player.viewDetector.FindPlayerTarget();
                player.viewDetector.RangeTarget.GetComponent<IInteractable>()?.TakeHit(player.damage);
            }
                yield return new WaitForSeconds(0.2f);
        }
        player.projectorC.gameObject.SetActive(false);
        player.isSkill = false;
        player.ChangeState(EnemyState.Idle);
    }
}


public class EnemyController : MonoBehaviour, IInteractable
{
    private float hp;
    public float HP 
    {
        get { return hp; } 
        set 
        { 
            hp = value; 
        }
    }
    public float damage;
    public float pushPower;
    public float speed;
    public EnemyState enemyState;

    public StateMachine<EnemyState, EnemyController> stateMachine = new StateMachine<EnemyState, EnemyController>();
    public Animator animator;
    public ViewDetector viewDetector;
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    public Material mat;
    [SerializeField] private SkinnedMeshRenderer skinnedMesh;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject damageText;
    [SerializeField] private Slider slider;
    [SerializeField] private Stack<GameObject> textStack = new Stack<GameObject>();
    [SerializeField] private Dictionary<int,EnemyState> skillDic = new Dictionary<int,EnemyState>();
    [SerializeField] private GameObject katana;
    public GameObject katanaPosA;
    [SerializeField] private GameObject katanaPosB;
    public ParticleSystem skillA;
    public ParticleSystem skillB;
    public ParticleSystem startSkillC;
    public ParticleSystem endSkillC;
    public Projector projectorA;
    public Projector projectorB;
    public Projector projectorC;


    public bool isSkill;
    public bool isSword = false;
    public bool isAtk = false;
    public bool isAtkCool = true;
    public bool isSkillB;
    public bool isDrive = true;



    private void Awake()
    {
        HP = 1000;
        animator = GetComponent<Animator>();
        viewDetector = GetComponent<ViewDetector>();
        audioSource = GetComponent<AudioSource>();
        mat = skinnedMesh.material;
        stateMachine.Reset(this);
        stateMachine.AddState(EnemyState.Idle, new EnemyIdle());
        stateMachine.AddState(EnemyState.Walk, new EnemyWalk());
        stateMachine.AddState(EnemyState.Run, new EnemyRun());
        stateMachine.AddState(EnemyState.Attack, new EnemyAttack());
        stateMachine.AddState(EnemyState.Hit, new EnemyHit());
        stateMachine.AddState(EnemyState.Groggy, new EnemyGroggy());
        stateMachine.AddState(EnemyState.SkillA, new EnemySkillA());
        stateMachine.AddState(EnemyState.SkillB, new EnemySkillB());
        stateMachine.AddState(EnemyState.SkillC, new EnemySkillC());
        ChangeState(EnemyState.Idle);

        skillDic.Add(0, EnemyState.SkillA);
        skillDic.Add(1, EnemyState.SkillB);
        skillDic.Add(2, EnemyState.SkillC);
    }

    private void Start()
    {
        for(int i = 0; i < 5; i++)
        {
            GameObject damage = Instantiate(damageText, canvas.transform);
            textStack.Push(damage);
            damage.SetActive(false);
            damage.transform.position = canvas.transform.position;
            damage.GetComponent<DamageText>().enemyController = this;
        }

        StartCoroutine(SkillCo());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.GetComponent<PlayerState>() != null)
        {
            if(isSkillB)
            {
                collision.transform.GetComponent<IInteractable>()?.TakeHit(damage);
                isSkillB = false;
            }
        }

        if(collision.transform.CompareTag("Wall"))
        {
            isDrive = false;
        }
    }

    private void Update()
    {
        stateMachine.Update();
        if(!isSkill)
        {
            viewDetector.FindAttackTarget();
            if(viewDetector.AtkTarget != null)
            {
                if(isAtkCool)
                {
                    StartCoroutine(AttackCo());
                    ChangeState(EnemyState.Attack);
                }
            }
        }

        if(isAtk)
        {
            if(Input.GetMouseButtonDown(2))
            {
                ChangeState(EnemyState.Groggy);
            }
        }
    }

    private void ExitPool(float _damage)
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

    public void Die()
    {
    }

    public void ChangeState(EnemyState state)
    {
        enemyState = state;
        stateMachine.ChangeState(state);
    }

    public void TakeHit(float damage)
    {
        HP -= damage;
        ExitPool(damage);
        if(!isSkill)
        {
            ChangeState(EnemyState.Hit);
        }
    }

    public void IdleOn()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            ChangeState(EnemyState.Idle);
        }
    }

    public void Attack()
    {
        damage = 5;
        if(isAtk)
        {
            viewDetector.FindAttackTarget();
            if (viewDetector.AtkTarget != null)
            {
                viewDetector.AtkTarget.GetComponent<IInteractable>()?.TakeHit(damage);
            }
        }
    }

    public void GroggyTiming()
    {
        isAtk = !isAtk;
    }

    public void KatanaOn()
    {
        audioSource.PlayOneShot(audioClips[0]);
        katana.transform.parent = katanaPosA.transform;
        katana.transform.position = katanaPosA.transform.position;
        katana.transform.rotation = katanaPosA.transform.rotation;
        ChangeState(enemyState);
    }

    public void KatanaOff()
    {
        katana.transform.parent = katanaPosB.transform;
        katana.transform.position = katanaPosB.transform.position;
        katana.transform.rotation = katanaPosB.transform.rotation;
        isSword = false;
        isSkillB = false;
    }

    private IEnumerator SkillCo()
    {
        while(true)
        {
            int rand = Random.Range(0, 3);
            int randCool = Random.Range(8, 15);
            yield return new WaitForSeconds(randCool);
            ChangeState(skillDic[rand]);
        }
    }

    private IEnumerator AttackCo()
    {
        isAtkCool = false;
        yield return new WaitForSeconds(2f);
        isAtkCool = true;
    }
}