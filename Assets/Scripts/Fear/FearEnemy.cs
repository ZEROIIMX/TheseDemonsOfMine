using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class FearEnemy : MonoBehaviour
{
    [Header("Behavior")]
    [SerializeField] private float activationDistance = 30f;
    [SerializeField] private float firstTeleportDistance = 15f;
    [SerializeField] private float secondTeleportDistance = 7f;
    [SerializeField] private float attackDistance = 3f;
    [SerializeField] private float timeBetweenTeleports = 2f;
    [SerializeField] private float portalBehindDistance = 2f;

    [Header("References")]
    [SerializeField] private Renderer[] graphics;
    [SerializeField] private GameObject portalPrefab;

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private bool isActivated = false;
    private Animator animator;
    private GameObject currentPortal;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        SetVisibility(false);

        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }
    }

    private void Update()
    {
        if (isActivated || player == null)
        {
            return;
        }

        if (Vector3.Distance(transform.position, player.position) <= activationDistance)
        {
            Activate();
        }
    }

    private void Activate()
    {
        isActivated = true;
        CreatePortal(transform.position);
        StartCoroutine(TeleportAttackSequence());
    }

    private IEnumerator TeleportAttackSequence()
    {
        yield return new WaitForSeconds(timeBetweenTeleports);
        TeleportTowardsPlayer(firstTeleportDistance);

        yield return new WaitForSeconds(timeBetweenTeleports);
        TeleportTowardsPlayer(secondTeleportDistance);

        yield return new WaitForSeconds(timeBetweenTeleports);
        TeleportAndAttack();
    }

    private void TeleportTowardsPlayer(float distance)
    {
        if (player == null) return;

        Vector3 directionToPlayer = (transform.position - player.position).normalized;
        Vector3 targetPosition = player.position + directionToPlayer * distance;

        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
        {
            CreatePortal(hit.position);
            Teleport(hit.position);
        }
        else
        {
            CreatePortal(targetPosition);
            Teleport(targetPosition);
        }
    }

    private void TeleportAndAttack()
    {
        if (player == null) return;

        Vector3 directionFromPlayer = (transform.position - player.position).normalized;
        Vector3 targetPosition = player.position + directionFromPlayer * attackDistance;

        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
        {
            CreatePortal(hit.position);
            Teleport(hit.position);
        }
        else
        {
            CreatePortal(targetPosition);
            Teleport(targetPosition);
        }

        SetVisibility(true);

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    private void Teleport(Vector3 position)
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }
        transform.position = position;
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = true;
        }

        if (player != null)
        {
            transform.LookAt(player.position);
        }
    }

    private void SetVisibility(bool isVisible)
    {
        foreach (var renderer in graphics)
        {
            renderer.enabled = isVisible;
        }
    }

    private void CreatePortal(Vector3 enemyTeleportPosition)
    {
        if (currentPortal != null)
        {
            Destroy(currentPortal);
        }

        if (portalPrefab != null)
        {
            Vector3 portalPosition = enemyTeleportPosition;
            Quaternion portalRotation = Quaternion.identity;

            if (player != null)
            {
                Vector3 directionFromPlayer = (enemyTeleportPosition - player.position).normalized;
                
                portalPosition = enemyTeleportPosition + directionFromPlayer * portalBehindDistance;
                
                portalRotation = Quaternion.LookRotation(player.position - portalPosition);
            }
            
            currentPortal = Instantiate(portalPrefab, portalPosition, portalRotation);
        }
    }
}
