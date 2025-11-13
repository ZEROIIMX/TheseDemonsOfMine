using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class MultiplyEnemy : MonoBehaviour
{
    [Header("Stats")]
    public float Health = 100f;

    [Header("Behavior")]
    public float activationDistance = 25f;
    public float rotationSpeed = 10f;
    public float movementSpeed = 8f;

    [Header("Spawning")]
    public GameObject enemyCopyPrefab;
    public GameObject spawnVFXPrefab;
    public int numberOfCopies = 8;
    public float spawnInterval = 1f;
    public float spawnRadius = 5f;
    public float spawnDelay = 2f;
    public float trackingDelay = 2f;

    [SerializeField] private GameObject hitVFXPrefab;
    [SerializeField] private GameObject AuraVFX;

    private bool isDead = false;
    private bool isActivated = false;
    private bool trackingStarted = false;
    private bool wasCopy = false;
    private bool isTemporarilyStunned = false;
    private bool damaged = false;
    private bool isLeader = false;

    private List<AIUnit> groupUnits = new List<AIUnit>();
    private Transform player;
    private Animator m_animator;
    private AIUnit selfAIUnit;
    private MultiplyEnemy leader;
    public Rigidbody rb;
    private CloseDamage closeDamage;
    private StressConnection stressConnection;
    private GameObject auraInstance;

    private void Awake()
    {
        stressConnection = GetComponent<StressConnection>();
        closeDamage = GetComponent<CloseDamage>();
        if (closeDamage != null) closeDamage.enabled = false;

        m_animator = GetComponentInChildren<Animator>();
        selfAIUnit = GetComponent<AIUnit>();
        rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        if (selfAIUnit?.Agent != null)
        {
            selfAIUnit.Agent.updateRotation = false;
            selfAIUnit.Agent.isStopped = true;
        }
    }

    private void Start()
    {
        player = AIManager.Instance.Target;
        if (player == null) enabled = false;

        if (leader == null && !wasCopy)
        {
            isLeader = true;
        }
    }

    private void Update()
    {
        if (isDead) return;

        if (!isActivated && isLeader && Vector3.Distance(transform.position, player.position) <= activationDistance)
        {
            Activate();
            return;
        }

        RotateTowardPlayer();

        if (isTemporarilyStunned || !trackingStarted) return;

        if (selfAIUnit != null && selfAIUnit.Agent.isActiveAndEnabled)
        {
            selfAIUnit.MoveTo(player.position);
        }
    }

    private void RotateTowardPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void Activate()
    {
        if (isActivated) return;

        if (isLeader)
        {
            AddToGroup(selfAIUnit);
            StartSpawningCopies();

            if (AuraVFX != null && auraInstance == null)
            {
                Vector3 auraOffset = new Vector3(0, 1.5f, 0);
                auraInstance = Instantiate(AuraVFX, transform.position + auraOffset, Quaternion.identity, transform);
            }
        }

        isActivated = true;

        if (selfAIUnit.Agent != null)
        {
            selfAIUnit.Agent.isStopped = false;
            selfAIUnit.Agent.speed = movementSpeed;
        }
    }

    public void StartSpawningCopies()
    {
        StartCoroutine(SpawnCopiesOverTime());
        StartCoroutine(DelayedTrackingStart());
    }

    private IEnumerator SpawnCopiesOverTime()
    {
        yield return new WaitForSeconds(spawnDelay);
        for (int i = 0; i < numberOfCopies; i++)
        {
            SpawnSingleCopy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private IEnumerator DelayedTrackingStart()
    {
        float totalSpawnTime = spawnDelay + (numberOfCopies * spawnInterval);
        yield return new WaitForSeconds(totalSpawnTime + trackingDelay);

        if (isLeader && auraInstance != null)
        {
            Destroy(auraInstance);
        }

        foreach (var unit in groupUnits)
        {
            MultiplyEnemy enemy = unit.GetComponent<MultiplyEnemy>();
            if (enemy != null)
            {
                enemy.trackingStarted = true;
                enemy.isActivated = true;

                if (enemy.closeDamage != null)
                    enemy.closeDamage.enabled = true;

                enemy.m_animator?.SetBool("Run", true);
            }
        }
    }

    private void SpawnSingleCopy()
    {
        if (enemyCopyPrefab == null) return;

        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

        GameObject copyObject = Instantiate(enemyCopyPrefab, spawnPosition, Quaternion.identity);

        AIUnit aiUnit = copyObject.GetComponent<AIUnit>();
        MultiplyEnemy copyEnemy = copyObject.GetComponent<MultiplyEnemy>();

        if (aiUnit != null && aiUnit.Agent != null)
        {
            aiUnit.Agent.enabled = true;
            aiUnit.Agent.isStopped = false;
            aiUnit.Agent.speed = movementSpeed;

            AddToGroup(aiUnit);
        }

        if (copyEnemy != null)
        {
            copyEnemy.Health = 100f;
            copyEnemy.SetLeader(this);
        }

        CloseDamage copyDamage = copyObject.GetComponent<CloseDamage>();
        if (copyDamage != null)
        {
            copyDamage.enabled = false;
        }
    }

    public void SetLeader(MultiplyEnemy leaderInstance)
    {
        leader = leaderInstance;
        wasCopy = true;

        if (spawnVFXPrefab != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0, 1f, 0);
            GameObject vfx = Instantiate(spawnVFXPrefab, spawnPosition, Quaternion.identity);
            Destroy(vfx, 2f);
        }
    }

    public void TakeDamage(int damageAmount, Vector3 hitPoint)
    {
        if (isDead || !trackingStarted) return;
        if (!damaged)
        {
            damaged = true;

            stressConnection?.A1OFF();
            stressConnection?.A2OFF();
            stressConnection?.A3OFF();

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
                closeDamage?.Dead();

                if (selfAIUnit?.Agent != null) selfAIUnit.Agent.enabled = false;
                Collider col = GetComponent<Collider>();
                if (col != null) col.enabled = false;

                if (leader != null) leader.RemoveUnitFromGroup(selfAIUnit);
                else groupUnits.Clear();
            }
        }
    }

    IEnumerator DamageCooldown()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        damaged = false;
    }

    public void ResumeTracking()
    {
        isTemporarilyStunned = false;
        m_animator.SetBool("Run", true);
    }

    public void TakeForce()
    {
        StartCoroutine(DelayedResetVelocity());
    }

    IEnumerator DelayedResetVelocity()
    {
        yield return new WaitForSeconds(0.3f);
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.isKinematic = true;
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void AddToGroup(AIUnit unit)
    {
        if (unit != null && !groupUnits.Contains(unit))
        {
            groupUnits.Add(unit);
        }
    }
    public void RemoveUnitFromGroup(AIUnit unit)
    {
        if (groupUnits.Contains(unit))
        {
            groupUnits.Remove(unit);
        }
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
        m_animator?.SetBool("Run", false);
    }

}
