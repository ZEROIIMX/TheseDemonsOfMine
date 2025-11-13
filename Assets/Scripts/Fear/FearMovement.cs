using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FearMovement : MonoBehaviour
{
    [Header("Stats")]
    public float Health = 100f;

    [Header("Behavior")]
    public float rotationSpeed = 10f;
    public float movementSpeed = 8f;

    [Header("Teleportation")]
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private float teleportRange = 3f;
    [SerializeField] private float teleportCooldown = 0f;

    [SerializeField] private GameObject hitVFXPrefab;

    private float teleportTimer;
    private bool isDead;
    private bool isTemporarilyStunned = false;
    private bool damaged = false;

    public Transform Target;
    private PlayerController playerController;
    private Animator m_animator;
    public Rigidbody rb;
    private NavMeshAgent agent;
    private FearConnection fearConnection;
    private FearDamage fearDamage;

    private void Awake()
    {
        fearDamage = GetComponent<FearDamage>();
        fearConnection = GetComponentInChildren<FearConnection>();
        m_animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (AIManager.Instance == null || AIManager.Instance.Target == null)
        {
            Debug.LogWarning("AIManager or Target is missing.");
            enabled = false;
            return;
        }

        Target = AIManager.Instance.Target;
        playerController = Target.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (isDead) return;

        RotateTowardPlayer();
        m_animator?.SetBool("Run", true);

        if (isTemporarilyStunned) return;

        agent.isStopped = false;
        agent.destination = Target.position;

        HandleTeleportation();
    }

    private void RotateTowardPlayer()
    {
        if (Target == null) return;

        Vector3 direction = (Target.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void HandleTeleportation()
    {
        teleportTimer -= Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, Target.position);
        if (distanceToPlayer > teleportRange && teleportTimer <= 0f)
        {
            Vector3 predictedPosition = GetPlayerPredictedPosition();

            // Spawn portal at current position
            GameObject portalA = Instantiate(portalPrefab, transform.position, Quaternion.identity);

            // Spawn portal at predicted position
            GameObject portalB = Instantiate(portalPrefab, predictedPosition, Quaternion.identity);

            // Teleport enemy
            transform.position = predictedPosition;

            // Clean up portals after 0.3 seconds
            Destroy(portalA, 0.3f);
            Destroy(portalB, 0.3f);

            // Reset cooldown
            teleportTimer = teleportCooldown;
        }
    }


    private Vector3 GetPlayerPredictedPosition()
    {
        if (playerController != null)
        {
            return Target.position + playerController.GetVelocity() * Time.deltaTime;
        }
        return Target.position;
    }

    public void TakeDamage(int damageAmount, Vector3 hitPoint)
    {
        if (isDead) return;
        if (!damaged)
        {
            damaged = true;

            fearConnection?.FE();

            Health -= damageAmount;
            isTemporarilyStunned = true;
            m_animator?.SetBool("Run", false);
            m_animator?.SetTrigger("OnHit");

            if (hitVFXPrefab != null)
            {
                GameObject vfx = Instantiate(hitVFXPrefab, hitPoint, Quaternion.identity);
                Destroy(vfx, 1f);
            }

            if (rb != null)
            {
                rb.isKinematic = false;
                rb.constraints = RigidbodyConstraints.None;
            }

            StartCoroutine(DamageCooldown());

            if (Health <= 0)
            {
                isDead = true;
                m_animator?.SetTrigger("Death");
                fearDamage?.Dead();
            }
        }
    }

    private IEnumerator DamageCooldown()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        damaged = false;
    }

    public void ResumeTracking()
    {
        if (isDead) return;
        isTemporarilyStunned = false;
        m_animator.SetBool("Run", true);
    }

    public void TakeForce()
    {
        StartCoroutine(DelayedResetVelocity());
    }

    private IEnumerator DelayedResetVelocity()
    {
        yield return new WaitForSeconds(0.3f);
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.isKinematic = true;
        }
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void Stop()
    {
        isTemporarilyStunned = true;
        agent.isStopped = true;
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
        isTemporarilyStunned = true;
        agent.isStopped = true;
        m_animator?.SetBool("Run", false);
        isDead = true;
    }
}
