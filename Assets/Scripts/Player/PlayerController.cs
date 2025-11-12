using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpHeight = 3f;
    public float gravity = -9.81f;
    public float jumpResetDelay = 0.2f;
    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    [Header("Wall Jump Settings")]
    public LayerMask doubleJumpLayer;
    private Vector3 moveInput;
    private Vector3 bufferedMoveInput = Vector3.zero;
    private Vector3 latestMoveInput = Vector3.zero;
    private Vector3 velocity;
    private CharacterController controller;
    private Animator animator;
    private bool isJumping = false;
    private bool isGrounded = true;
    private bool wallJump = false;
    private bool isDashing = false;
    private bool canDash = true;
    private bool hasWallJumped = false;
    private bool isTouchingWall = false;
    public bool isRootMotionActive = false;
    private Sword sword;
    private bool isJumpHeld = false;
    private bool isDashHeld = false;
    private bool isMoveHeld = false;
    private bool useUnscaledTime = false;
    private Vector2 look = Vector2.zero;
    [SerializeField] float worldBottomBounndary = -100f;
    (Vector3, Quaternion) initialPositionAndRotation;
    public void UseUnscaledTime(bool value)
    {
        useUnscaledTime = value;
    }
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        sword = GetComponentInChildren<Sword>();
        initialPositionAndRotation = (transform.position, transform.rotation);
    }

    void Update()
    {
        float delta = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            hasWallJumped = false;
        }

        if (isRootMotionActive)
        {
            latestMoveInput = Vector3.zero;
            bufferedMoveInput = Vector3.zero;
        }
        else
        {
            Vector3 move = new Vector3(moveInput.x, 0, moveInput.z);

            if (move.magnitude > 0)
            {
                latestMoveInput = move;
            }

            if (move.magnitude > 0.1f)
            {
                float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            }

            controller.Move(move * moveSpeed * delta);
        }

        if (isJumping && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = false;
        }

        velocity.y += gravity * delta;
        controller.Move(velocity * delta);

        CheckBounds();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (isRootMotionActive) return;
        Vector2 input = context.ReadValue<Vector2>();
        moveInput = new Vector3(input.x, 0, input.y);
        isMoveHeld = context.performed;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isJumpHeld = true;
            if (isGrounded || (isTouchingWall && !hasWallJumped))
            {
                isJumping = true;
                if (isTouchingWall)
                {
                    hasWallJumped = true;
                }
            }
        }
        else if (context.canceled)
        {
            isJumpHeld = false;
        }
    }

    private IEnumerator JumpResetDelay()
    {
        yield return new WaitForSeconds(jumpResetDelay);
        isJumping = false;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash && !isDashing)
        {
            isDashHeld = true;
            StartCoroutine(Dash(useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime));
        }
        else if (context.canceled)
        {
            isDashHeld = false;
        }
    }

    private IEnumerator Dash(float delta)
    {
        isDashing = true;
        canDash = false;
        float startTime = Time.time;
        Vector3 dashDirection = latestMoveInput.normalized;
        if (dashDirection == Vector3.zero)
        {
            dashDirection = transform.forward;
        }

        while (Time.time < startTime + dashDuration)
        {
            controller.Move(dashDirection * dashSpeed * delta);
            yield return null;
        }

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (((1 << hit.gameObject.layer) & doubleJumpLayer) != 0)
        {
            isTouchingWall = true;
        }
        else
        {
            isTouchingWall = false;
        }
    }

    public void JumpingSlash()
    {
        isRootMotionActive = true;
    }

    public void JumpingSlashFinished()
    {
        isRootMotionActive = false;
    }

    public bool IsDashing()
    {
        return isDashing;
    }

    public Vector3 GetVelocity()
    {
        return controller.velocity;
    }

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        controller.enabled = false;
        transform.SetPositionAndRotation(position, rotation);
        controller.enabled = true;
        velocity = Vector3.zero;
    }

    void CheckBounds()
    {
        if (transform.position.y < worldBottomBounndary)
        {
            GameManager.Instance.PlayerDied();
        }
    }
}
