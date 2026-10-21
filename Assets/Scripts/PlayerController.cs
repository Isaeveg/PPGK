using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float crouchSpeed = 1.2f;
    public float gravity = -20f;
    public float turnSpeed = 10f;
    public float standHeight = 1.8f;
    public float crouchHeight = 1f;
    public float mouseSensitivity = 0.15f;
    public float jumpDuration = 1f;
    public Animator animator;

    [HideInInspector] public bool topDownMode;
    public bool HasTarget { get; private set; }

    CharacterController cc;
    Vector3 target;
    float verticalVelocity;
    float jumpTimer;
    float rootMotionY;
    bool isCrouching;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        SetCrouch(false);
    }

    public void AddRootMotion(Vector3 delta) => rootMotionY += delta.y;

    public void MoveTo(Vector3 point)
    {
        target = point;
        HasTarget = true;
    }

    public void StopMoving() => HasTarget = false;

    void Update()
    {
        var kb = Keyboard.current;
        bool jumping = jumpTimer > 0f;
        Vector3 move = Vector3.zero;
        float speed = walkSpeed;

        if (!topDownMode)
        {
            HasTarget = false;

            if (Cursor.lockState == CursorLockMode.Locked)
                transform.Rotate(0f, Mouse.current.delta.ReadValue().x * mouseSensitivity, 0f);

            float x = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
            float z = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);
            move = Vector3.ClampMagnitude(transform.right * x + transform.forward * z, 1f);

            if (!jumping && kb.leftCtrlKey.isPressed != isCrouching) SetCrouch(!isCrouching);

            if (isCrouching) speed = crouchSpeed;
            else if (kb.leftShiftKey.isPressed) speed = runSpeed;

            if (kb.spaceKey.wasPressedThisFrame && cc.isGrounded && !jumping && !isCrouching)
            {
                jumpTimer = jumpDuration;
                animator.SetTrigger("Jump");
            }
        }
        else
        {
            if (isCrouching) SetCrouch(false);

            if (HasTarget)
            {
                Vector3 toTarget = target - transform.position;
                toTarget.y = 0f;
                if (toTarget.magnitude < 0.15f) HasTarget = false;
                else
                {
                    move = toTarget.normalized;
                    speed = runSpeed;
                    transform.rotation = Quaternion.Slerp(transform.rotation,
                        Quaternion.LookRotation(move), turnSpeed * Time.deltaTime);
                }
            }
        }

        Vector3 motion = move * speed * Time.deltaTime;

        if (jumpTimer > 0f)
        {
            jumpTimer -= Time.deltaTime;
            motion.y = rootMotionY;
            verticalVelocity = 0f;
        }
        else
        {
            if (cc.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
            verticalVelocity += gravity * Time.deltaTime;
            motion.y = verticalVelocity * Time.deltaTime;
        }
        rootMotionY = 0f;

        cc.Move(motion);

        Vector3 velocity = cc.velocity;
        velocity.y = 0f;

        if (HasTarget && move != Vector3.zero && velocity.magnitude < speed * 0.1f) HasTarget = false;

        animator.SetFloat("Speed", velocity.magnitude, 0.1f, Time.deltaTime);
        animator.SetBool("IsCrouching", isCrouching);
    }

    void SetCrouch(bool crouch)
    {
        isCrouching = crouch;
        cc.height = crouch ? crouchHeight : standHeight;
        cc.center = Vector3.up * cc.height / 2f;
    }
}
