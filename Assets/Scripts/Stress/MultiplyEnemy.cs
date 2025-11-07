using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class MultiplyEnemy : MonoBehaviour
{
    public float Health;
    private bool isDead;
    public GameObject enemyCopyPrefab;
    public int numberOfCopies;
    public float spawnRadius;
    public float circleFormationDistance;
    public float circleRadius;
    public float deathAnimationDuration;
    private List<AIUnit> groupUnits = new List<AIUnit>();
    private Transform player;
    private bool isCircling;
    private Animator m_animator;
    private AIUnit selfAIUnit;
    private MultiplyEnemy leader;
    private bool wasCopy = false;
    private Rigidbody rb;

    private void Awake()
    {
        m_animator = GetComponentInChildren<Animator>();
        selfAIUnit = GetComponent<AIUnit>();
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    private void Start()
    {
        player = AIManager.Instance.Target;

        if (player == null)
        {
            Debug.LogError($"[{gameObject.name}] Player target not found via AIManager. Disabling script.", this);
            enabled = false;
            return;
        }

        if (leader == null && !wasCopy)
        {
            if (enemyCopyPrefab == null)
            {
                Debug.LogError($"[{gameObject.name}] 'Enemy Copy Prefab' is not assigned in the Inspector! Cannot spawn copies.", this);
                if (!groupUnits.Contains(selfAIUnit))
                {
                    groupUnits.Add(selfAIUnit);
                }
                return;
            }

            if (!groupUnits.Contains(selfAIUnit))
            {
                groupUnits.Add(selfAIUnit);
            }
            for (int i = 0; i < numberOfCopies; i++)
            {
                SpawnCopy();
            }
        }
    }

    private void Update()
    {
        if (leader != null && leader.isDead)
        {
            leader = null;
            wasCopy = true; 
            if (!groupUnits.Contains(selfAIUnit))
            {
                groupUnits.Add(selfAIUnit);
            }
        }

        if (isDead || leader != null) return;

        if (isCircling)
        {
            AIManager.Instance.MakeAgentsCircleTarget(groupUnits, circleRadius);
        }
        else
        {
            if (Vector3.Distance(transform.position, player.position) <= circleFormationDistance)
            {
                isCircling = true;
            }
            else
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
    }

    private void SpawnCopy()
    {
        if (enemyCopyPrefab == null) return;

        Vector3 spawnPosition = transform.position + (Vector3)Random.insideUnitCircle * spawnRadius;
        GameObject copyObject = Instantiate(enemyCopyPrefab, spawnPosition, Quaternion.identity);

        AIUnit aiUnit = copyObject.GetComponent<AIUnit>();
        if (aiUnit != null)
        {
            groupUnits.Add(aiUnit);
        }

        MultiplyEnemy copyEnemy = copyObject.GetComponent<MultiplyEnemy>();
        if (copyEnemy != null)
        {
            copyEnemy.SetLeader(this);
        }
    }

    public void SetLeader(MultiplyEnemy leaderInstance)
    {
        leader = leaderInstance;
        wasCopy = true;
    }

    public void RemoveUnitFromGroup(AIUnit unit)
    {
        if (groupUnits != null && groupUnits.Contains(unit))
        {
            groupUnits.Remove(unit);
        }
    }

    public void TakeDamage(int damageAmount, Vector3 hitDirection, float force)
    {
        if (isDead) return;

        Health -= damageAmount;
        m_animator?.SetTrigger("OnHit");
    
        StartCoroutine(ApplyHitForce(hitDirection, force));

        if (Health <= 0)
        {
            isDead = true;
            m_animator?.SetTrigger("Death");

            if (selfAIUnit != null && selfAIUnit.Agent != null)
            {
                selfAIUnit.Agent.enabled = false;
            }

            var collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            if (leader != null)
            {
                if (leader != null)
                {
                    leader.RemoveUnitFromGroup(selfAIUnit);
                }
            }
            else
            {
                groupUnits.Clear();
            }

            StartCoroutine(DeathSequence());
        }
    }

    private IEnumerator ApplyHitForce(Vector3 hitDirection, float force)
    {
        if (selfAIUnit.Agent.isActiveAndEnabled)
        {
            selfAIUnit.Agent.enabled = false;
        }
        rb.isKinematic = false;
        rb.AddForce(hitDirection * force, ForceMode.Impulse);

        yield return new WaitForSeconds(0.5f);

        if (!isDead)
        {
            rb.isKinematic = true; 
            selfAIUnit.Agent.enabled = true;
        }
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(deathAnimationDuration);
        Destroy(gameObject);
    }

    public void Death()
    {
        if (!isDead)
        {
            StartCoroutine(DeathSequence());
        }
    }
}
