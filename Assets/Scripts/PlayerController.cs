using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;

    public bool isMove;
    public bool isJumpCool = true;
    private float moveY;

    [SerializeField] private Camera cam;
    [SerializeField] private Transform cameraPoint;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private GroundChecker groundChecker;

    private Vector3 moveVec;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        groundChecker = GetComponent<GroundChecker>();
        animator = GetComponent<Animator>();
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        PlayerMove();
    }

    private void Update()
    {
        Gravity();

        if(isMove)
        {
            animator.SetBool("Walk", true);
            if (Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetBool("Run", true);
            }
            else
            {
                animator.SetBool("Run", false);
            }
        }
        else
        {
            animator.SetBool("Run", false);
            animator.SetBool("Walk", false);
        }
    }

    private void PlayerMove()
    {
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if(moveInput.magnitude > 0)
        {
            isMove = true;
        }
        else
        {
            isMove = false;
        }

        if(!isMove)
        {
            return;
        }

        Vector3 forwarVec = new Vector3(cam.transform.forward.x, 0f, cam.transform.forward.z);
        Vector3 RightVec = new Vector3(cam.transform.right.x, 0f, cam.transform.right.z);

        moveVec = moveInput.x * RightVec + moveInput.z * forwarVec;
        transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveVec), Time.deltaTime * rotateSpeed);
        characterController.Move(moveVec * moveSpeed * Time.deltaTime);
    }

    private void Gravity()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(isJumpCool)
            {
                moveY = jumpSpeed * Time.deltaTime;
                animator.Play("Running Jump");
                StartCoroutine(JumpCo());
            }
        }

        if(groundChecker.IsGrounded)
        {
            moveY = 0;
        }
        else
        {
            moveY += Physics.gravity.y * Time.deltaTime;
        }

        characterController.Move(Vector3.up * moveY * Time.deltaTime);
    }

    private IEnumerator JumpCo()
    {
        isJumpCool = false;
        yield return new WaitForSeconds(1f);
        isJumpCool = true;
    }
}
