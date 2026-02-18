using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerPun : MonoBehaviourPun
{
    [Header("References")]
    public Camera playerCamera;
    public Transform cameraPivot;
    public AudioListener audioListener;

    [Header("Move")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float acceleration = 12f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 1.2f;
    public float gravity = -25f;
    public float groundedStick = -2f;

    [Header("Look")]
    public float mouseSensitivity = 2f;
    public float maxPitch = 80f;

    [Header("Misc")]
    public bool lockCursor = true;

    private CharacterController cc;
    private Vector3 moveVelocity;
    private float verticalVelocity;
    private float pitch;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            EnableLocalStuff(true);

            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        else
        {
            var cc = GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            EnableLocalStuff(false);
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        Look();
        Move();
    }

    private void EnableLocalStuff(bool enable)
    {
        if (playerCamera != null) playerCamera.enabled = enable;
        if (audioListener != null) audioListener.enabled = enable;

        if (audioListener == null && playerCamera != null)
        {
            var al = playerCamera.GetComponent<AudioListener>();
            if (al != null) al.enabled = enable;
        }
    }

    private void Look()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(x, 0f, z);
        input = Vector3.ClampMagnitude(input, 1f);

        bool sprint = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = sprint ? sprintSpeed : walkSpeed;

        Vector3 targetVel = (transform.right * input.x + transform.forward * input.z) * targetSpeed;

        moveVelocity = Vector3.MoveTowards(moveVelocity, targetVel, acceleration * Time.deltaTime);

        bool grounded = cc.isGrounded;
        if (grounded && verticalVelocity < 0f)
            verticalVelocity = groundedStick;

        if (grounded && Input.GetButtonDown("Jump"))
        {
            // v = sqrt(h * -2g)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 motion = new Vector3(moveVelocity.x, verticalVelocity, moveVelocity.z);
        cc.Move(motion * Time.deltaTime);
    }
}
