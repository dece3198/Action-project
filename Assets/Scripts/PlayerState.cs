using System.Collections;
using UnityEngine;

public enum State
{
    Idle, Walk, Run, Jump, Attack, Hit
}

public abstract class BaseState<T>
{
    public abstract void Enter(T player);
    public abstract void Update(T player);
    public abstract void Exit(T player);
}

public class PlayerIdle : BaseState<PlayerState>
{
    private float time;
    public override void Enter(PlayerState player)
    {
        player.animator.SetBool("Walk", false);
        player.animator.SetBool("Run", false);
        time = 0;
    }

    public override void Exit(PlayerState player)
    {
    }

    public override void Update(PlayerState player)
    {
        if(player.controller.isMove)
        {
            player.ChangeState(State.Walk);
        }

        if (player.katanaposA.transform.childCount > 0)
        {
            time += Time.deltaTime;

            if(time > 8f)
            {
                player.animator.SetTrigger("Off");
                time = 0;
            }
        }
    }
}

public class PlayerWalk : BaseState<PlayerState>
{
    public override void Enter(PlayerState player)
    {
        player.controller.moveSpeed = 3;
        player.animator.SetBool("Walk", true);
        player.animator.SetBool("Run", false);
    }

    public override void Exit(PlayerState player)
    {
    }

    public override void Update(PlayerState player)
    {
        if (!player.controller.isMove)
        {
            player.ChangeState(State.Idle);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            player.ChangeState(State.Run);
        }
    }
}

public class PlayerRun : BaseState<PlayerState>
{
    public override void Enter(PlayerState player)
    {
        player.controller.moveSpeed = 5;
        player.animator.SetBool("Run", true);
        player.runParticle.gameObject.SetActive(true);
        player.runParticle.Play();
    }

    public override void Exit(PlayerState player)
    {
        player.runParticle.gameObject.SetActive(false);
    }

    public override void Update(PlayerState player)
    {
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            player.ChangeState(State.Walk);
        }

        if(!player.controller.isMove)
        {
            player.ChangeState(State.Idle);
        }
    }
}

public class PlayerJump : BaseState<PlayerState>
{
    private bool isJumpCool = true;

    public override void Enter(PlayerState player)
    {
        if (isJumpCool)
        { 
            player.jumpParticle.gameObject.SetActive(true);
            player.jumpParticle.Play();
            player.animator.Play("Running Jump");
            player.controller.moveY = player.controller.jumpSpeed;
            player.StartCoroutine(JumpCo(player));
        }
    }

    public override void Exit(PlayerState player)
    {
    }

    public override void Update(PlayerState player)
    {
        if (player.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            player.ChangeState(State.Idle);
        }
        else if(!player.animator.GetCurrentAnimatorStateInfo(0).IsName("Running Jump"))
        {
            player.ChangeState(State.Run);
        }

        if(!player.controller.isMove)
        {
            player.ChangeState(State.Idle);
        }
    }

    private IEnumerator JumpCo(PlayerState player)
    {
        isJumpCool = false;
        yield return new WaitForSeconds(2f);
        isJumpCool = true;
    }
}

public class PlayerAttack : BaseState<PlayerState>
{
    public override void Enter(PlayerState player)
    {
        player.controller.moveSpeed = 1;
        player.animator.Play("SlashA");
    }

    public override void Exit(PlayerState player)
    {
        player.isComboA = false;
        player.isComboB = false;
    }

    public override void Update(PlayerState player)
    {

        if(player.isComboA)
        {
            if(Input.GetMouseButtonDown(0))
            {
                player.animator.SetBool("ComboA", true);
            }
        }

        if(player.isComboB)
        {
            if (Input.GetMouseButtonDown(0))
            {
                player.animator.SetBool("ComboB", true);
            }
        }

        if(player.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            player.ChangeState(State.Idle);
            player.animator.SetBool("ComboB", false);
            player.animator.SetBool("ComboA", false);
        }

        if(player.animator.GetCurrentAnimatorStateInfo(0).IsName("SlashB"))
        {
            player.animator.SetBool("ComboA", false);
        }
        if (player.animator.GetCurrentAnimatorStateInfo(0).IsName("SlashC"))
        {
            player.animator.SetBool("ComboB", false);
        }

    }
}

public class PlayerHit : BaseState<PlayerState>
{
    public override void Enter(PlayerState player)
    {
        player.controller.moveSpeed = 0;
        player.animator.SetBool("ComboB", false);
        player.animator.SetBool("ComboA", false);
        player.animator.Play("Hit");
    }

    public override void Exit(PlayerState player)
    {

    }

    public override void Update(PlayerState player)
    {
        if(player.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            player.ChangeState(State.Idle);
        }
    }
}

public class PlayerState : MonoBehaviour,IInteractable
{
    [SerializeField] private float hp;
    public float Hp 
    { 
        get { return hp; }
        set { hp = value; }
    }
    public float damage = 0;

    private AudioSource audioSource;
    public AudioClip[] audioClip;

    public PlayerController controller;
    public Animator animator;
    public State state;
    public StateMachine<State, PlayerState> stateMachine = new StateMachine<State, PlayerState>();
    public GameObject katanaposA;
    public GameObject katanaposB;
    public GameObject katana;
    public ParticleSystem jumpParticle;
    public ParticleSystem runParticle;
    public ParticleSystem atkParticleA;
    public ParticleSystem atkParticleB;
    public ViewDetector viewDetector;
    [SerializeField] private float pushPower;

    public bool isAtk = false;
    public bool isComboA = false;
    public bool isComboB = false;
    private bool isAtkCool = true;
    private bool isBlockCool = true;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        controller = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        viewDetector = GetComponent<ViewDetector>();
        stateMachine.Reset(this);
        stateMachine.AddState(State.Idle, new PlayerIdle());
        stateMachine.AddState(State.Walk, new PlayerWalk());
        stateMachine.AddState(State.Run, new PlayerRun());
        stateMachine.AddState(State.Jump, new PlayerJump());
        stateMachine.AddState(State.Attack, new PlayerAttack());
        stateMachine.AddState(State.Hit, new PlayerHit());
        ChangeState(State.Idle);
    }

    private void Update()
    {
        stateMachine.Update();
        if (state != State.Jump)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (isAtk)
                {
                    if (state != State.Attack)
                    {
                        if (isAtkCool)
                        {
                            StartCoroutine(AttackCo());
                            ChangeState(State.Attack);
                        }
                    }
                }
                else
                {
                    controller.moveSpeed = 0;
                    animator.Play("On");
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (state != State.Hit)
                ChangeState(State.Jump);
        }

        viewDetector.FindAttackTarget();
        if(isBlockCool)
        {
            if (Input.GetMouseButtonDown(1))
            {
                animator.Play("Block");
                StartCoroutine(BlockCO());
                if (viewDetector.AtkTarget != null)
                {
                    if (viewDetector.AtkTarget.GetComponent<EnemyController>().isAtk)
                    {
                        viewDetector.AtkTarget.GetComponent<EnemyController>().ChangeState(EnemyState.Groggy);
                    }
                }
            }
        }
    }

    public void ChangeState(State _state)
    {
        state = _state;
        stateMachine.ChangeState(_state);
    }

    public void KatanaOn()
    {
        audioSource.PlayOneShot(audioClip[0]);
        katana.transform.parent = katanaposA.transform;
        katana.transform.position = katanaposA.transform.position;
        katana.transform.rotation = katanaposA.transform.rotation;
        ChangeState(state);
        isAtk = true;
    }

    public void KatanaOff()
    {
        katana.transform.parent = katanaposB.transform;
        katana.transform.position = katanaposB.transform.position;
        katana.transform.rotation = katanaposB.transform.rotation;
        isAtk = false;
    }

    public void ComboA()
    {
        isComboA = !isComboA;
    }

    public void ComboB()
    {
        isComboB = !isComboB;
    }

    public void AtkParticleA()
    {
        atkParticleA.gameObject.SetActive(true);
        viewDetector.FindTarget();
        atkParticleA.Play();
        if (viewDetector.Target != null)
        {
            damage = 10;
            viewDetector.Target.GetComponent<IInteractable>()?.TakeHit(damage);
            pushPower = 2f;
            if(!viewDetector.Target.GetComponent<Monster>().isSkill)
            {
                viewDetector.Target.GetComponent<Rigidbody>().AddForce((transform.forward + transform.up) * pushPower, ForceMode.Impulse);
            }
        }
    }

    public void AtkParticleB()
    {
        
        atkParticleB.gameObject.SetActive(true);
        atkParticleB.Play();
        viewDetector.FindTarget();
        if (viewDetector.Target != null)
        {
            damage = 30;
            pushPower = 4f;
            viewDetector.FindRangeTarget(damage);
            if (!viewDetector.Target.GetComponent<Monster>().isSkill)
            {
                viewDetector.Target.GetComponent<Rigidbody>().AddForce((transform.forward + transform.up) * pushPower, ForceMode.Impulse);
            }
        }
    }

    public void TakeHit(float damage)
    {
        Hp -= damage;
        Debug.Log(Hp);
        controller.moveSpeed = 0;
        if(state != State.Hit)
        ChangeState(State.Hit);
    }

    public void Die()
    {
    }

    private IEnumerator AttackCo()
    {
        isAtkCool = false;
        yield return new WaitForSeconds(1f);
        isAtkCool = true;
    }

    private IEnumerator BlockCO()
    {
        float speed = controller.moveSpeed;
        isBlockCool = false;
        controller.moveSpeed = 0;
        yield return new WaitForSeconds(1f);
        isBlockCool = true;
        controller.moveSpeed = speed;
    }
}
