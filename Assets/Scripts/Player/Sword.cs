using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sword : MonoBehaviour
{
    // References
    public Animator childAnimator;
    public Transform handContainer;
    public Transform backContainer;
    public Transform sword;

    // Sword positions
    public Vector3 swordHandPos;
    public Vector3 swordHandRot;
    public Vector3 swordBackPostion;
    public Vector3 swordBackRot;

    // State
    private bool equipped = false;
    private bool attackOnCooldown = false;
    private bool slash2 = false;
    private bool slash3 = false;
    private bool s2 = false;
    private bool s3 = false;
    private bool parry = true;
    private bool isParryHeld = false;
    private bool isParryTimeActive = false;

    private Coroutine attackCooldownRoutine;
    private Coroutine saveCooldownRoutine;
    private Coroutine parryCooldownRoutine;
    private Coroutine parryTimeRoutine;


    // Components
    private SwordHitbox SwordHitbox;
    private CharacterController controller;
    private PlayerController playerController;
    private ParentConnection parentConnection;

    // Root motion cache
    private Vector3 rootMotionDeltaPosition = Vector3.zero;
    private Quaternion rootMotionDeltaRotation = Quaternion.identity;
    private bool applyRootMotionThisFrame = false;

    public float attackCd;
    public float saveCd;
    public float parryCd;

    public bool isParrying = false;
    public bool isHeld = false;

    public bool isDead = true;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        SwordHitbox = GetComponentInChildren<SwordHitbox>();
        controller = GetComponent<CharacterController>();
        parentConnection = GetComponentInChildren<ParentConnection>();

        if (childAnimator == null)
            childAnimator = GetComponentInChildren<Animator>();

        childAnimator.applyRootMotion = true;
    }

    void Update()
    {
        if (isDead) return;

        if (isParryHeld && parry && !isHeld)
        {
            if (!equipped)
            {
                childAnimator.SetTrigger("Parry");
            }
            else
            {
                parentConnection?.P();
            }
        }
    }

    void FixedUpdate()
    {
        if (!applyRootMotionThisFrame || controller == null || playerController == null) return;

        if (!playerController.IsDashing())
        {
            controller.Move(rootMotionDeltaPosition);
            transform.rotation *= rootMotionDeltaRotation;
        }
        else
        {
            controller.Move(new Vector3(0, rootMotionDeltaPosition.y, 0));
        }

        rootMotionDeltaPosition = Vector3.zero;
        rootMotionDeltaRotation = Quaternion.identity;
        applyRootMotionThisFrame = false;
    }

    public bool ShouldApplyRootMotion()
    {
        return childAnimator.GetCurrentAnimatorStateInfo(0).IsTag("RootMotion");
    }

    public void ApplyRootMotionFromChild(Vector3 deltaPos, Quaternion deltaRot)
    {
        rootMotionDeltaPosition += deltaPos;
        rootMotionDeltaRotation *= deltaRot;
        applyRootMotionThisFrame = true;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (isDead || !context.started || attackOnCooldown || childAnimator == null) return;

        if (!equipped && isHeld)
        {
            childAnimator.SetTrigger("HeldDraw");
            attackOnCooldown = true;
            RestartAttackCooldown();
        }
        else if (isHeld)
        {
            childAnimator.SetTrigger("HeldAttack");
            attackOnCooldown = true;
            RestartAttackCooldown();
        }
        else if (!equipped && !slash2 && !slash3)
        {
            childAnimator.SetTrigger("Draw");
            attackOnCooldown = true;
        }
        else if (equipped && !slash2 && !slash3)
        {
            childAnimator.SetTrigger("1");
            attackOnCooldown = true;
        }
        else if (equipped && slash2 && !slash3)
        {
            childAnimator.SetTrigger("2");
            slash2 = false;
            attackOnCooldown = true;
            s2 = true;
            RestartAttackCooldown();
        }
        else if (equipped && !slash2 && slash3)
        {
            playerController?.JumpingSlash();
            childAnimator.SetTrigger("3");
            slash3 = false;
            attackOnCooldown = true;
            s3 = true;
            RestartAttackCooldown();
        }
        else if (!equipped && !slash2 && slash3)
        {
            playerController?.JumpingSlash();
            childAnimator.SetTrigger("SpinDraw");
            slash3 = false;
            attackOnCooldown = true;
            s3 = true;
            RestartAttackCooldown();
        }

            RestartSaveCooldown();
    }

    private IEnumerator AttackCooldown()
    {
        attackOnCooldown = true;
        yield return new WaitForSecondsRealtime(attackCd);
        attackOnCooldown = false;
        attackCooldownRoutine = null;
    }

    public void RestartAttackCooldown()
    {
        if (attackCooldownRoutine != null)
        {
            StopCoroutine(attackCooldownRoutine);
        }
        if (!s2 && !s3)
        {
            attackCooldownRoutine = StartCoroutine(AttackCooldown());
        }
    }

    private IEnumerator SaveCooldown()
    {
        yield return new WaitForSecondsRealtime(saveCd);
        childAnimator.SetTrigger("Save");
        saveCooldownRoutine = null;
    }

    public void RestartSaveCooldown()
    {
        if (saveCooldownRoutine != null)
        {
            StopCoroutine(saveCooldownRoutine);
        }
        saveCooldownRoutine = StartCoroutine(SaveCooldown());
    }

    public void PlaceSwordToHand()
    {
        sword.SetParent(handContainer, false);
        sword.localPosition = swordHandPos;
        sword.localEulerAngles = swordHandRot;
        equipped = true;
    }

    public void PlaceSwordToBack()
    {
        sword.SetParent(backContainer, false);
        sword.localPosition = swordBackPostion;
        sword.localEulerAngles = swordBackRot;
        equipped = false;
    }

    public void Slash2()
    {
        slash2 = true;
        attackOnCooldown = false;
        StartCoroutine(ResetSlash2());
        Debug.Log("S2 available for 0.3");
    }

    private IEnumerator ResetSlash2()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        Debug.Log("S2 end");
        slash2 = false;
        attackOnCooldown = true;
        RestartAttackCooldown();
    }

    public void Slash3()
    {
        slash3 = true;
        attackOnCooldown = false;
        StartCoroutine(ResetSlash3());
        Debug.Log("S3 available for 0.3");
        s2 = false;
    }

    private IEnumerator ResetSlash3()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        Debug.Log("S3 end");
        slash3 = false;
        attackOnCooldown = true;
        RestartAttackCooldown();
    }

    public void OnParry(InputAction.CallbackContext context)
    {
        if (isDead || childAnimator == null) return;

        if (context.started)
        {
            isParryHeld = true;
        }
        else if (context.canceled)
        {
            isParryHeld = false;
        }
    }

    public void Parry()
    {
        if (isDead) return;

        parry = false;
        isParrying = true;
        childAnimator.SetTrigger("Parry2");
        RestartParryCooldown();
        RestartSaveCooldown();
        SwordHitbox?.ParryHitbox();

        if (attackOnCooldown && !s2 && !s3)
        {
            Slash2();
        }
        if (s2 && !s3)
        {
            Slash3();
        }
        if (!s2 && s3)
        {
            playerController?.JumpingSlashFinished();
        }
    }


    public void FinishParry()
    {
        isParrying = false;
        SwordHitbox?.ParryHitboxEnd();
    }

    public void setS3False()
    {
        s3 = false;
    }

    private IEnumerator ParryCooldown()
    {
        yield return new WaitForSecondsRealtime(parryCd);
        parryCooldownRoutine = null;
        parry = true;
    }

    public void RestartParryCooldown()
    {
        if (parryCooldownRoutine != null)
        {
            StopCoroutine(parryCooldownRoutine);
        }
        parryCooldownRoutine = StartCoroutine(ParryCooldown());
    }

    public bool IsParrying()
    {
        return isParrying;
    }

    public void Held()
    {
        isHeld = true;
    }

    public void NotHeld()
    {
        isHeld = false;
    }

    public void ParryTime()
    {
        if (isParryTimeActive) return;
        isParryTimeActive = true;
        parryTimeRoutine = StartCoroutine(SlowWorldExceptPlayer(0.1f, 5f));
        Debug.Log("Parry Time Activated");
    }

    private IEnumerator SlowWorldExceptPlayer(float slowFactor, float duration)
    {
        isParryTimeActive = true;

        Time.timeScale = slowFactor;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        childAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        playerController.UseUnscaledTime(true);

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        childAnimator.updateMode = AnimatorUpdateMode.Normal;
        playerController.UseUnscaledTime(false);

        isParryTimeActive = false; // Unlock after cooldown

    }
    public void CancelParryTime()
    {
        if (parryTimeRoutine != null)
        {
            StopCoroutine(parryTimeRoutine);
            parryTimeRoutine = null;
        }

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        childAnimator.updateMode = AnimatorUpdateMode.Normal;
        playerController.UseUnscaledTime(false);
        isParryTimeActive = false;
    }

    public void DisableSword()
    {
        isDead = true;
    }

    public void EnableSword()
    {
        isDead = false;
    }
}