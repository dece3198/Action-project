using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum WarriorState
{
    Idle, Walk, Run, Hit,Attack, SkillA,SkillB,SkillC, SkillD, Die
}

public class WarriorIdle : BaseState<WarriorController>
{
    float time;
    bool isSkillOn = true;
    public override void Enter(WarriorController player)
    {
        time = 0;
        player.animator.SetBool("Walk", false);
    }

    public override void Exit(WarriorController player)
    {
    }

    public override void Update(WarriorController player)
    {
        player.viewDetector.FindTarget();

        if(player.weaponPosA.transform.childCount < 0)
        {
            time += Time.deltaTime;

            if(time > 8f)
            {
                player.animator.Play("Off");
                time = 0;
            }
        }

        if(player.viewDetector.Target != null)
        {
            if(!player.isSword)
            {
                player.animator.Play("On");
            }
            else
            {
                if(isSkillOn)
                {
                    player.Skill = player.SkillCo();
                    player.StartCoroutine(player.Skill);
                    isSkillOn = false;
                }
            }
            player.ChangeState(WarriorState.Walk);
        }
    }
}

public class WarriorWalk : BaseState<WarriorController>
{
    public override void Enter(WarriorController player)
    {
        player.speed = 2;
        player.animator.SetBool("Walk", true);
    }

    public override void Exit(WarriorController player)
    {
    }

    public override void Update(WarriorController player)
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
            player.ChangeState(WarriorState.Idle);
        }
    }
}

public class WarriorRun : BaseState<WarriorController>
{
    float time;

    public override void Enter(WarriorController player)
    {
        time = 3f;
        player.speed = 10f;
        player.animator.Play("SkillD");
        player.animator.SetBool("Walk", true);
        player.StartCoroutine(RunCo(player));
        player.isSkillD = true;
    }

    public override void Exit(WarriorController player)
    {
    }

    public override void Update(WarriorController player)
    {
    }

    private IEnumerator RunCo(WarriorController warrior)
    {
        float runningTime = time;
        while (runningTime > 0)
        {
            runningTime -= Time.deltaTime;
            if(warrior.isDrive)
            {
                warrior.transform.Translate(Vector3.forward * warrior.speed * Time.deltaTime);
                yield return null;
            }
        }

        warrior.isSkillD = false;
        warrior.isDrive = true;
        warrior.animator.SetBool("Walk", false);
        warrior.ChangeState(WarriorState.Idle);
    }
}

public class WarriorHit : BaseState<WarriorController>
{
    Color color;
    public override void Enter(WarriorController player)
    {
        player.animator.Play("Hit");
        player.StartCoroutine(HitCo(player));
    }

    public override void Exit(WarriorController player)
    {
    }

    public override void Update(WarriorController player)
    {
        player.IdleOn();
    }

    private IEnumerator HitCo(WarriorController warrior)
    {
        color = warrior.skinnedMesh.materials[3].color;
        warrior.skinnedMesh.materials[3].color = Color.red;
        warrior.gameObject.layer = 0;
        yield return new WaitForSeconds(0.2f);
        warrior.gameObject.layer = 7;
        warrior.skinnedMesh.materials[3].color = color;
    }
}

public class WarriorAttack : BaseState<WarriorController>
{
    public override void Enter(WarriorController player)
    {
        player.animator.Play("Attack");
        player.animator.SetBool("Walk", false);
    }

    public override void Exit(WarriorController player)
    {
    }

    public override void Update(WarriorController player)
    {
    }
}

public class WarriorSkillA : BaseState<WarriorController>
{
    float time;
    Vector3 targetPos;
    public override void Enter(WarriorController player)
    {
        time = 2f;
        player.damage = 20;
        player.isSkill = true;
        player.animator.Play("JumpAttack");
        player.StartCoroutine(SkillACo(player));
    }

    public override void Exit(WarriorController player)
    {
    }

    public override void Update(WarriorController player)
    {
        player.viewDetector.FindTarget();
    }

    private IEnumerator SkillACo(WarriorController player)
    {
        float runningTime = time;

        targetPos = player.viewDetector.Target.transform.position;
        player.projectorC.transform.SetParent(null);
        player.projectorC.transform.position = targetPos;
        player.projectorC.gameObject.SetActive(true);
        player.projectorC.orthographicSize = 0.01f;
        player.GetComponent<Rigidbody>().AddForce(Vector3.up * 15, ForceMode.Impulse);
        while (runningTime > 0f)
        {
            runningTime -= Time.deltaTime;
            player.projectorC.orthographicSize += 5f * Time.deltaTime;
            player.transform.position = Vector3.Lerp(player.transform.position, targetPos, 0.02f);
            yield return null;
        }

        player.skillAParticle.gameObject.SetActive(true);
        player.skillAParticle.Play();
        player.viewDetector.FindPlayerTarget();
        if(player.viewDetector.RangeTarget != null)
        {
            player.viewDetector.RangeTarget.GetComponent<IInteractable>()?.TakeHit(player.damage);
        }
        player.projectorC.gameObject.SetActive(false);
        player.projectorC.transform.parent = player.transform;
        player.projectorC.transform.position = player.transform.position;
        player.isSkill = false;
        player.ChangeState(WarriorState.Idle);
    }
}

public class WarriorSkillB : BaseState<WarriorController>
{
    float time;
    public override void Enter(WarriorController player)
    {
        time = 2;
        player.isSkill = true;
        player.damage = 10;
        player.StartCoroutine(SkillBCo(player));
    }

    public override void Exit(WarriorController player)
    {
    }

    public override void Update(WarriorController player)
    {
    }

    private IEnumerator SkillBCo(WarriorController player)
    {
        float runningTime = time;
        player.animator.Play("SpinA");
        player.projectorC.gameObject.SetActive(true);
        player.projectorC.orthographicSize = 0.01f;

        while (runningTime > 0f)
        {
            runningTime -= Time.deltaTime;
            player.projectorC.orthographicSize += 5f * Time.deltaTime;
            yield return null;
        }
        player.weapon.transform.position = player.weaponPosC.transform.position;
        player.weapon.transform.rotation = player.weaponPosC.transform.rotation;
        player.projectorC.gameObject.SetActive(false);
        player.skillBParticle.gameObject.SetActive(true);
        player.skillBParticle.Play();
        for(int i = 0; i < 5; i++)
        {
            player.viewDetector.FindPlayerTarget();
            if (player.viewDetector.RangeTarget != null)
            {
                player.viewDetector.RangeTarget.GetComponent<IInteractable>()?.TakeHit(player.damage);
            }
            yield return new WaitForSeconds(0.2f);
        }
        player.weapon.transform.position = player.weaponPosA.transform.position;
        player.weapon.transform.rotation = player.weaponPosA.transform.rotation;
        player.skillBParticle.gameObject.SetActive(false);
        player.isSkill = false;
        player.ChangeState(WarriorState.Idle);
    }
}

public class WarriorSkillC : BaseState<WarriorController>
{
    public override void Enter(WarriorController player)
    {
        player.isSkill = true;
        player.animator.Play("Roar");
        player.StartCoroutine(SkillCCo(player));
    }

    public override void Exit(WarriorController player)
    {
        player.isSkill = false;
    }

    public override void Update(WarriorController player)
    {
        player.IdleOn();
    }

    private IEnumerator SkillCCo(WarriorController warrior)
    {
        warrior.skillCParticle.gameObject.SetActive(true);
        warrior.skillCParticle.Play();
        warrior.def = 999;
        yield return new WaitForSeconds(10f);
        warrior.skillCParticle.gameObject.SetActive(false);
        warrior.def = 0;
    }
}

public class WarriorSkillD : BaseState<WarriorController>
{
    float time;
    public override void Enter(WarriorController player)
    {
        time = 1;
        player.damage = 30;
        player.isSkill = true;
        player.animator.Play("Block");
        player.StartCoroutine(SkillDCo(player));
    }

    public override void Exit(WarriorController player)
    {
    }

    public override void Update(WarriorController player)
    {
    }

    private IEnumerator SkillDCo(WarriorController player)
    {
        float runningTime = time;
        player.projectorB.orthographicSize = 0.01f;
        player.projectorB.aspectRatio = 5f;
        player.projectorB.gameObject.SetActive(true);

        while (runningTime > 0f)
        {
            runningTime -= Time.deltaTime;
            player.projectorB.orthographicSize += 4f * Time.deltaTime;
            player.projectorB.aspectRatio += 4f * Time.deltaTime;
            yield return null;
        }

        player.projectorB.gameObject.SetActive(false);
        player.isSkill = false;
        player.ChangeState(WarriorState.Run);
    }
}

public class WarriorDie : BaseState<WarriorController>
{
    public override void Enter(WarriorController player)
    {
    }

    public override void Exit(WarriorController player)
    {
    }

    public override void Update(WarriorController player)
    {
    }
}

public class WarriorController : Monster, IInteractable
{
    [SerializeField] private float hp;
    public float Hp 
    {
        get { return hp; } 
        set 
        { 
            hp = value; 

        }
    }

    public float def;

    public WarriorState warriorState;
    public StateMachine<WarriorState, WarriorController> stateMachine = new StateMachine<WarriorState, WarriorController>();
    [SerializeField] private Dictionary<int, WarriorState> skillDic = new Dictionary<int, WarriorState>();
    public GameObject weaponPosC;

    public ParticleSystem skillAParticle;
    public ParticleSystem skillBParticle;
    public ParticleSystem skillCParticle;

    public bool isSkillD = false;

    private void Awake()
    {
        viewDetector = GetComponent<ViewDetector>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        stateMachine.Reset(this);
        stateMachine.AddState(WarriorState.Idle, new WarriorIdle());
        stateMachine.AddState(WarriorState.Walk, new WarriorWalk());
        stateMachine.AddState(WarriorState.Run, new WarriorRun());
        stateMachine.AddState(WarriorState.Hit, new WarriorHit());
        stateMachine.AddState(WarriorState.Attack, new WarriorAttack());
        stateMachine.AddState(WarriorState.SkillA, new WarriorSkillA());
        stateMachine.AddState(WarriorState.SkillB, new WarriorSkillB());
        stateMachine.AddState(WarriorState.SkillC, new WarriorSkillC());
        stateMachine.AddState(WarriorState.SkillD, new WarriorSkillD());
        stateMachine.AddState(WarriorState.Die, new WarriorDie());
        ChangeState(WarriorState.Idle);

        skillDic.Add(0, WarriorState.SkillA);
        skillDic.Add(1, WarriorState.SkillB);
        skillDic.Add(2, WarriorState.SkillC);
        skillDic.Add(3, WarriorState.SkillD);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.GetComponent<PlayerState>() != null)
        {
            if(isSkillD)
            {
                collision.transform.GetComponent<IInteractable>()?.TakeHit(damage);
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
                    if(isSword)
                    {
                        if(warriorState != WarriorState.Hit)
                        {
                            if(warriorState != WarriorState.Run)
                            {
                                StartCoroutine(AttackCo());
                                ChangeState(WarriorState.Attack);
                            }
                        }
                    }
                }
            }
        }

        viewDetector.FindTarget();
        if(slider != null)
        {
            if(viewDetector.Target != null)
            {
                slider.gameObject.SetActive(true);
            }
            slider.value = Hp / maxHp;
        }
    }

    public void TakeHit(float damage)
    {
        if(def >= damage)
        {
            damage = 0;
            Hp -= damage;
        }
        else
        {
            Hp -= damage;
        }
        ExitPool(damage);
        if(!isSkill)
        {
            ChangeState(WarriorState.Hit);
        }
    }

    public void Die()
    {

    }

    public void IdleOn()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            ChangeState(WarriorState.Idle);
        }
    }

    public void Attack()
    {
        damage = 10;
        viewDetector.FindAttackTarget();
        if (viewDetector.AtkTarget != null)
        {
            viewDetector.AtkTarget.GetComponent<IInteractable>()?.TakeHit(damage);
        }
    }

    public void AxeOn()
    {
        weapon.transform.parent = weaponPosA.transform;
        weapon.transform.position = weaponPosA.transform.position;
        weapon.transform.rotation = weaponPosA.transform.rotation;
        isSword = true;
        ChangeState(WarriorState.Idle);
    }

    public void AxeOff()
    {
        weapon.transform.parent = weaponPosB.transform;
        weapon.transform.position = weaponPosB.transform.position;
        weapon.transform.rotation = weaponPosB.transform.rotation;
        ChangeState(warriorState);
        isSword = false;
    }

    public void ChangeState(WarriorState state)
    {
        warriorState = state;
        stateMachine.ChangeState(state);
    }

    public IEnumerator SkillCo()
    {
        while (true)
        {
            int rand = Random.Range(0, skillDic.Count);
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
