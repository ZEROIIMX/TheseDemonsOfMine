using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Animations;

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

    private bool isDead = true;    

    public void UseUnscaledTime(bool value)
    {
        useUnscaledTime = value;
        if (animator != null)
        {
            animator.updateMode = value ? AnimatorUpdateMode.UnscaledTime : AnimatorUpdateMode.Normal;
        }
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        sword = GetComponent<Sword>();
        initialPositionAndRotation = (transform.position, transform.rotation);
    }

    void Update()
    {
        if (isDead) return;
        float delta = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        if (isMoveHeld)
        {
            if (bufferedMoveInput != Vector3.zero)
            {
                moveInput = bufferedMoveInput;
                bufferedMoveInput = Vector3.zero;
            }
            else if (latestMoveInput != Vector3.zero)
            {
                moveInput = latestMoveInput;
            }
        }
        else
        {
            moveInput = Vector3.zero;
        }

        if (isDashHeld && canDash && !isDashing && latestMoveInput != Vector3.zero)
        {
            StartCoroutine(Dash(delta));
            animator?.SetBool("Dash", true);
            sword?.Slash3();
        }

        if (isJumpHeld)
        {
            bool canWallJump = isTouchingWall && !hasWallJumped;

            if (isGrounded && !isJumping)
            {
                isGrounded = false;
                StartCoroutine(JumpResetDelay());
                animator?.SetTrigger("Jump");
            }
            else if (canWallJump && wallJump)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator?.SetTrigger("WallJump");
                hasWallJumped = true;
            }
        }

        isTouchingWall = false;


        if (!isDashing && !isRootMotionActive && moveInput != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, delta * 10f);
        }
        else if (!isDashing && isRootMotionActive)
        {
            controller.Move(new Vector3(0, velocity.y, 0) * delta);
        }

        if (controller.isGrounded && !isJumping)
        {
            velocity.y = -2f;
            isGrounded = true;
            hasWallJumped = false;
            wallJump = false;
        }
        else
        {
            velocity.y += gravity * delta;
        }

        if (!isDashing && !isRootMotionActive)
        {
            Vector3 move = moveInput * moveSpeed;
            move.y = velocity.y;
            controller.Move(move * delta);
        }
        else if (!isDashing && isRootMotionActive)
        {
            controller.Move(new Vector3(0, velocity.y, 0) * delta);
        }

        if (animator != null)
        {
            animator.SetBool("Run", moveInput != Vector3.zero);
            animator.SetBool("IsGrounded", isDashing ? true : controller.isGrounded);
        }

        CheckBounds();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (isDead)
            return;

        if (context.canceled)
        {
            isMoveHeld = false;
            latestMoveInput = Vector3.zero;
            bufferedMoveInput = Vector3.zero;
            moveInput = Vector3.zero;
            return;
        }

        if (context.performed)
        {
            isMoveHeld = true;

            Vector2 input = context.ReadValue<Vector2>();
            float x = input.y;
            float z = input.x;
            Vector3 newInput = new Vector3(-x, 0, z);
            latestMoveInput = newInput;
            moveInput = newInput;
            bufferedMoveInput = Vector3.zero;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (isDead)
            return;

        if (context.started)
        {
            isJumpHeld = true;
        }
        else if (context.canceled)
        {
            isJumpHeld = false;
        }
    }

    private IEnumerator JumpResetDelay()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        isJumping = true;
        yield return new WaitForSeconds(jumpResetDelay);
        isJumping = false;
        wallJump = true;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (isDead)
            return;

        if (context.started)
        {
            isDashHeld = true;
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

        float timer = 0f;

        Vector3 dashDirection = latestMoveInput != Vector3.zero ? latestMoveInput.normalized : transform.forward;
        transform.rotation = Quaternion.LookRotation(dashDirection);

        while (timer < dashDuration)
        {
            controller.Move(dashDirection * dashSpeed * delta);
            timer += delta;
            yield return null;
        }

        isDashing = false;
        animator?.SetBool("Dash", false);

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (((1 << hit.gameObject.layer) & doubleJumpLayer) != 0)
        {
            isTouchingWall = true;
        }
    }

    public void JumpingSlash()
    {
        moveSpeed = 6.5f;
    }

    public void JumpingSlashFinished()
    {
        moveSpeed = 6.5f;
        sword?.setS3False();
        sword?.RestartAttackCooldown();
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
        transform.position = position;
        Physics.SyncTransforms();
        look.x = rotation.eulerAngles.y;
        look.y = rotation.eulerAngles.z;
        velocity = Vector3.zero;
    }

    void CheckBounds()
    {
        if (transform.position.y < worldBottomBounndary)
        {
            var (position, rotation) = initialPositionAndRotation;
            Destroy(gameObject);
        }
    }

    public void DisableControls()
    {
        isDead = true;
    }

    public void EnableControls()
    {
        isDead = false;
    }
}
