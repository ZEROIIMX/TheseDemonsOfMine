using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class MultiplyEnemy : MonoBehaviour
{
    [Header("Stats")]
    public float Health;

    [Header("Behavior")]
    public float activationDistance = 25f;
    public float rotationSpeed = 10f;
    public float movementSpeed = 8f;

    [Header("Spawning")]
    public GameObject enemyCopyPrefab;
    public int numberOfCopies = 8;
    public float spawnInterval = 1f;
    public float spawnRadius = 5f;
    public float spawnDelay = 2f;
    public float trackingDelay = 2f;
    public bool trackingStarted = false;

    [Header("Death")]
    public float deathAnimationDuration;

    private bool isDead;
    private bool isActivated = false;
    private List<AIUnit> groupUnits = new List<AIUnit>();
    private Transform player;
    private Animator m_animator;
    private AIUnit selfAIUnit;
    private MultiplyEnemy leader;
    private bool wasCopy = false;
    private Rigidbody rb;

    private CloseDamage closeDamage;

    private bool isTemporarilyStunned = false;

    private void Awake()
    {
        closeDamage = GetComponent<CloseDamage>();
        closeDamage.enabled = false;
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
    }

    private void Update()
    {
        if (isDead) return;

        if (!isActivated && Vector3.Distance(transform.position, player.position) <= activationDistance)
        {
            Activate();
            return;
        }

        if (leader != null && leader.isDead)
        {
            leader = null;
            wasCopy = true;
            if (!groupUnits.Contains(selfAIUnit)) groupUnits.Add(selfAIUnit);
        }

        RotateTowardPlayer();

        if (leader != null) return;

        if (trackingStarted && !isTemporarilyStunned)
        {
            groupUnits.RemoveAll(item => item == null);
            foreach (var unit in groupUnits)
            {
                if (unit != null && unit.Agent.isActiveAndEnabled)
                {
                    unit.MoveTo(player.position);
                }
            }
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
        isActivated = true;

        if (selfAIUnit.Agent != null)
        {
            selfAIUnit.Agent.isStopped = false;
            selfAIUnit.Agent.speed = movementSpeed;
        }

        if (leader == null && !wasCopy)
        {
            if (!groupUnits.Contains(selfAIUnit)) groupUnits.Add(selfAIUnit);
            StartSpawningCopies();
        }
    }

    public void StartSpawningCopies()
    {
        StartCoroutine(SpawnCopiesOverTime());
        StartCoroutine(DelayedTrackingStart());
    }

    private IEnumerator DelayedTrackingStart()
    {
        float totalSpawnTime = spawnDelay + (numberOfCopies * spawnInterval);
        yield return new WaitForSeconds(totalSpawnTime + trackingDelay);

        trackingStarted = true;

        if (closeDamage != null)
        {
            closeDamage.enabled = true;
        }

        m_animator?.SetBool("Run", true);

        foreach (var unit in groupUnits)
        {
            MultiplyEnemy copy = unit.GetComponent<MultiplyEnemy>();
            copy?.StartTrackingAnimation();

            CloseDamage copyDamage = copy?.GetComponent<CloseDamage>();
            if (copyDamage != null)
            {
                copyDamage.enabled = true;
            }
        }
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

            if (!groupUnits.Contains(aiUnit))
            {
                groupUnits.Add(aiUnit);
            }

            if (trackingStarted && aiUnit.Agent.isOnNavMesh)
            {
                aiUnit.MoveTo(player.position);
            }
        }

        if (copyEnemy != null)
        {
            copyEnemy.SetLeader(this);

            if (trackingStarted)
            {
                copyEnemy.StartTrackingAnimation();
            }
        }

        CloseDamage copyDamage = copyObject.GetComponent<CloseDamage>();
        if (copyDamage != null)
        {
            copyDamage.enabled = trackingStarted;
        }
    }

    public void StartTrackingAnimation()
    {
        m_animator?.SetBool("Run", true);
    }

    public void SetLeader(MultiplyEnemy leaderInstance)
    {
        leader = leaderInstance;
        wasCopy = true;
    }

    public void RemoveUnitFromGroup(AIUnit unit)
    {
        if (groupUnits.Contains(unit)) groupUnits.Remove(unit);
    }

    public void TakeDamage(int damageAmount, Vector3 hitDirection, float force)
    {
        if (isDead) return;

        if (!isActivated) Activate();

        Health -= damageAmount;
        isTemporarilyStunned = true;
        m_animator?.SetBool("Run", false);
        m_animator?.SetTrigger("OnHit");
        StartCoroutine(ApplyHitForce(hitDirection, force));

        if (Health <= 0)
        {
            isDead = true;
            m_animator?.SetTrigger("Death");

            if (selfAIUnit?.Agent != null) selfAIUnit.Agent.enabled = false;
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }

            if (leader != null) leader.RemoveUnitFromGroup(selfAIUnit);
            else groupUnits.Clear();

            StartCoroutine(DeathSequence());
        }
    }

    private IEnumerator ApplyHitForce(Vector3 hitDirection, float force)
    {
        if (selfAIUnit.Agent.isActiveAndEnabled) selfAIUnit.Agent.enabled = false;
        rb.isKinematic = false;
        rb.AddForce(hitDirection * force, ForceMode.Impulse);

        yield return new WaitForSeconds(0.5f);

        if (!isDead)
        {
            rb.isKinematic = true;
            selfAIUnit.Agent.enabled = true;
            selfAIUnit.Agent.updateRotation = false;
        }
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(deathAnimationDuration);
        Destroy(gameObject);
    }

    public void Death()
    {
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }

        if (!isDead) StartCoroutine(DeathSequence());
    }
    public void ResumeTracking()
    {
        isTemporarilyStunned = false;
    }
}
