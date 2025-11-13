using UnityEngine;

public class FearDamage : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackDistance = 3f;
    [SerializeField] private float attackCooldown = 2f;

    private Transform player;
    private float cooldownTimer;
    private bool onCd = false;

    private Animator m_animator;
    private Rigidbody rb;
    private FearMovement fearMovement;

    private bool dead = false;
    private int attackIndex = 0; // Tracks which attack to play next

    // Root motion cache
    private Vector3 rootMotionDeltaPosition = Vector3.zero;
    private Quaternion rootMotionDeltaRotation = Quaternion.identity;
    private bool applyRootMotionThisFrame = false;

    void Start()
    {
        cooldownTimer = attackCooldown;
        player = AIManager.Instance.Target;
        fearMovement = GetComponent<FearMovement>();
        rb = GetComponent<Rigidbody>();
        m_animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (player == null || dead) return;

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0 && !onCd)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackDistance)
            {
                Attack();
                onCd = true;
            }
        }
    }

    private void Attack()
    {
        switch (attackIndex)
        {
            case 0:
                A1();
                break;
            case 1:
                A2();
                break;
            case 2:
                A3();
                break;
        }

        attackIndex = (attackIndex + 1) % 3; // Loop back to 0 after 2
    }

    public void ResetCooldown()
    {
        cooldownTimer = attackCooldown;
        onCd = false;
    }

    private void A1()
    {
        m_animator.SetTrigger("1");
        Attacking();
    }

    private void A2()
    {
        m_animator.SetTrigger("2");
        Attacking();
    }

    private void A3()
    {
        m_animator.SetTrigger("3");
        Attacking();
    }

    public void Dead()
    {
        dead = true;
    }
    public bool ShouldApplyRootMotion()
    {
        return m_animator.GetCurrentAnimatorStateInfo(0).IsTag("RootMotion");
    }

    public void ApplyRootMotionFromChild(Vector3 deltaPos, Quaternion deltaRot)
    {
        rootMotionDeltaPosition += deltaPos;
        rootMotionDeltaRotation *= deltaRot;
        applyRootMotionThisFrame = true;
    }
    void FixedUpdate()
    {
        if (!applyRootMotionThisFrame || rb == null) return;

        // Apply vertical movement from root motion
        Vector3 newPosition = rb.position + new Vector3(0, rootMotionDeltaPosition.y, 0);
        rb.MovePosition(newPosition);

        rootMotionDeltaPosition = Vector3.zero;
        rootMotionDeltaRotation = Quaternion.identity;
        applyRootMotionThisFrame = false;
    }

    public void Attacking()
    { 
    fearMovement?.Stop();
    }

    void OnEnable()
    {
        PlayerEvents.OnPlayerDeath += HandlePlayerDeath;
    }

    void OnDisable()
    {
        PlayerEvents.OnPlayerDeath -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        dead = true;
    }

}
