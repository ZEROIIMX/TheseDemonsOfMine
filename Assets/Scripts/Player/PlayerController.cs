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
    public float dashDurationDuringS2 = 0.05f;
    public float dashSpeedDuringS2 = 20f;

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
    private bool S2 = false;

    public bool isRootMotionActive = false;

    private Sword sword;

    // Input buffering
    private bool isJumpHeld = false;

    private bool isDashHeld = false;

    private bool isMoveHeld = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        sword = GetComponent<Sword>();
    }

    void Update()
    {
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
            StartCoroutine(Dash());
            animator?.SetBool("Dash", true);
        }

        if (isJumpHeld)
        {
            bool canWallJump = isTouchingWall && !hasWallJumped;

            if (isGrounded && !isJumping && !S2)
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

        if (S2 || sword.IsParrying())
        {
            moveInput = Vector3.zero;
        }
        else
        {
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
        }


        // Handle rotation
        if (!isDashing && !isRootMotionActive && moveInput != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // Gravity and grounded check
        if (controller.isGrounded && !isJumping)
        {
            velocity.y = -2f;
            isGrounded = true;
            hasWallJumped = false;
            wallJump = false;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // Combine movement
        if (!isDashing && !isRootMotionActive)
        {
            Vector3 move = moveInput * moveSpeed;
            move.y = velocity.y;
            controller.Move(move * Time.deltaTime);
        }
        else if (!isDashing && isRootMotionActive)
        {
            controller.Move(new Vector3(0, velocity.y, 0) * Time.deltaTime);
        }

        // Animator updates
        if (animator != null)
        {
            animator.SetBool("Run", !S2 && moveInput != Vector3.zero);
            animator.SetBool("IsGrounded", isDashing ? true : controller.isGrounded);
        }
    }


    public void OnMove(InputAction.CallbackContext context)
    {
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

            if (S2)
            {
                bufferedMoveInput = newInput;
                moveInput = Vector3.zero;
            }
            else
            {
                moveInput = newInput;
                bufferedMoveInput = Vector3.zero;
            }
        }
    }


    public void OnJump(InputAction.CallbackContext context)
    {
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
        if (context.started)
        {
            isDashHeld = true;
        }
        else if (context.canceled)
        {
            isDashHeld = false;
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        float timer = 0f;
        float currentDashDuration = S2 ? dashDurationDuringS2 : dashDuration;
        float currentDashSpeed = S2 ? dashSpeedDuringS2 : dashSpeed;

        Vector3 dashDirection = latestMoveInput != Vector3.zero ? latestMoveInput.normalized : transform.forward;
        transform.rotation = Quaternion.LookRotation(dashDirection);

        while (timer < currentDashDuration)
        {
            controller.Move(dashDirection * currentDashSpeed * Time.deltaTime);
            timer += Time.deltaTime;
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

    public void Attacking()
    {
        moveSpeed = 0f;
        S2 = true;
    }

    public void AttackingFinished()
    {
        moveSpeed = 6.5f;
        S2 = false;
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

    public void Parrying()
    {
        moveSpeed = 0f;
        S2 = true;
    }

    public void ParryEnd()
    {
        moveSpeed = 6.5f;
        S2 = false;
    }
    public bool IsDashing()
    {
        return isDashing;
    }
}
