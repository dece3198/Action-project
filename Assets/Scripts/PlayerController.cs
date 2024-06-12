using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;

    public bool isMove;
    public float moveY;

    [SerializeField] private Camera cam;
    [SerializeField] private Transform cameraPoint;

    public float moveSpeed;
    public float jumpSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private GroundChecker groundChecker;

    private Vector3 moveVec;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        groundChecker = GetComponent<GroundChecker>();
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
        characterController.Move(Vector3.up * moveY * Time.deltaTime);

        if (groundChecker.IsGrounded)
        {
            if(moveY < 0)
            {
                moveY = 0;
            }
        }
        else
        {
            moveY += Physics.gravity.y * Time.deltaTime;
        }
    }
}
