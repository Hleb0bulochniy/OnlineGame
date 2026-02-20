using Photon.Pun;
using UnityEngine;

public class PlayerControllerPun : MonoBehaviourPun
{
    [Header("References")]
    public Camera playerCamera;
    public Transform cameraPivot;
    public AudioListener audioListener;

    [Header("Move")]
    public float walkSpeed = 5f;
    public float acceleration = 12f;

    [Header("Look")]
    public float mouseSensitivity = 2f;
    public float maxPitch = 80f;

    [Header("Misc")]
    public bool lockCursor = true;

    private Rigidbody rb;
    private float pitch;
    private Vector3 wishDir;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // ����� �� �����������
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
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
            // ��� ����� ������� ��������� ������ � ����
            EnableLocalStuff(false);
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        Look();
        ReadInput();
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        MovePhysics();
    }

    private void OnEnable() => GetComponent<PlayerMass>().OnMassChangeAction += ChangeSpeed;
    private void OnDisable()
    {
        var pm = GetComponent<PlayerMass>();
        if (pm != null)
            pm.OnMassChangeAction -= ChangeSpeed;
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

    private void ReadInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(x, 0f, z);
        input = Vector3.ClampMagnitude(input, 1f);

        wishDir = (transform.right * input.x + transform.forward * input.z).normalized;
    }

    private void MovePhysics()
    {
        Vector3 currentVel = rb.linearVelocity;
        Vector3 targetVel = wishDir * walkSpeed;


        float control = 1f;

        Vector3 velChange = targetVel - new Vector3(currentVel.x, 0f, currentVel.z);
        velChange = Vector3.ClampMagnitude(velChange, acceleration * control);

        rb.AddForce(new Vector3(velChange.x, 0f, velChange.z), ForceMode.VelocityChange);
    }

    private void ChangeSpeed()
    {
        walkSpeed = 7f + (transform.localScale.x * walkSpeed) / 10f;
    }
}
