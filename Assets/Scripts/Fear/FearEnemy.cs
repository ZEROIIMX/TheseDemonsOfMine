using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;

public class FearEnemy : MonoBehaviour
{
    [Header("Behavior")]
    [SerializeField] private float activationDistance = 30f;
    [SerializeField] private float firstTeleportDistance = 15f;
    [SerializeField] private float secondTeleportDistance = 7f;
    [SerializeField] private float attackDistance = 3f;
    [SerializeField] private float timeBetweenTeleports = 2f;
    [SerializeField] private float portalFrontDistance = 2f;
    [SerializeField] private bool canTeleport = true;
    [SerializeField] private float portalLifetime = 1.5f;
    [SerializeField] private float portalSideOffset = 5f;
    [SerializeField] private float finalPortalScale = 2f;

    [Header("References")]
    [SerializeField] private Renderer[] graphics;
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private GameObject scythePrefab;
    [SerializeField] private float scytheLifetime = 2f;

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private bool isActivated = false;
    private Animator animator;
    private List<GameObject> activePortals = new List<GameObject>();
    private Transform mainCamera;
    private PlayerController playerController;
    private int teleportIndex = 0;

    private Vector3 offset = new Vector3(0.5f, 0f, 0f);

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }
        mainCamera = Camera.main.transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        SetVisibility(false);

        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }
    }

    private void OnDestroy()
    {
        foreach (var portal in activePortals)
        {
            if (portal != null)
            {
                Destroy(portal);
            }
        }
        activePortals.Clear();
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
        CreatePortal(transform.position, 1f);

        if (canTeleport)
        {
            StartCoroutine(TeleportAttackSequence());
        }
    }

    private IEnumerator TeleportAttackSequence()
    {
        teleportIndex = 0;

        yield return new WaitForSeconds(timeBetweenTeleports);
        teleportIndex++;
        TeleportTowardsPlayer(firstTeleportDistance);

        yield return new WaitForSeconds(timeBetweenTeleports);
        teleportIndex++;
        TeleportTowardsPlayer(secondTeleportDistance);

        yield return new WaitForSeconds(timeBetweenTeleports);
        teleportIndex++;
        TeleportAndAttack();
    }

    private Vector3 GetPlayerPredictedPosition()
    {
        if (playerController != null)
        {
            return player.position + playerController.GetVelocity() * Time.deltaTime;
        }
        return player.position;
    }

    private void TeleportTowardsPlayer(float distance)
    {
        if (player == null) return;

        Vector3 predictedPlayerPosition = GetPlayerPredictedPosition();
        Vector3 directionToPlayer = (transform.position - predictedPlayerPosition).normalized;
        if (transform.position.x - predictedPlayerPosition.x > 0)
        {
            directionToPlayer = new Vector3(-directionToPlayer.x, directionToPlayer.y, directionToPlayer.z);
        }
        Vector3 targetPosition = predictedPlayerPosition + directionToPlayer * distance;

        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
        {
            CreatePortal(hit.position, 1f);
            Teleport(hit.position);
        }
        else
        {
            CreatePortal(targetPosition, 1f);
            Teleport(targetPosition);
        }
    }

    private void TeleportAndAttack()
    {
        if (player == null) return;

        Vector3 predictedPlayerPosition = GetPlayerPredictedPosition();
        Vector3 targetPosition = predictedPlayerPosition - new Vector3(1.0f, 0.0f, 0.0f) * attackDistance;
        Vector3 finalPosition;

        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
        {
            finalPosition = hit.position;
        }
        else
        {
            finalPosition = targetPosition;
        }

        CreatePortal(finalPosition, finalPortalScale);

        if (scythePrefab != null)
        {
            GameObject scytheInstance = Instantiate(scythePrefab, finalPosition, Quaternion.LookRotation(player.position - finalPosition));
            
            ScytheAttack scytheAttack = scytheInstance.GetComponent<ScytheAttack>();
            if (scytheAttack != null)
            {
                scytheAttack.Spawner = this;
            }

            Destroy(scytheInstance, scytheLifetime);
        }
    }

    public void OnScytheParried(Vector3 scythePosition)
    {
        Teleport(scythePosition);
        SetVisibility(true);
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    public void OnPlayerFailedParry()
    {
        Destroy(gameObject);
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
    }

    private void SetVisibility(bool isVisible)
    {
        foreach (var renderer in graphics)
        {
            renderer.enabled = isVisible;
        }
    }

    private void CreatePortal(Vector3 enemyTeleportPosition, float scale)
    {
        if (portalPrefab != null)
        {
            Vector3 portalPosition = enemyTeleportPosition;
            Quaternion portalRotation = transform.rotation;

            if (player != null)
            {
                if (teleportIndex == 1)
                {
                    portalPosition = GetPlayerPredictedPosition() + Vector3.forward * portalSideOffset;
                }
                else if (teleportIndex == 2)
                { 
                    portalPosition = GetPlayerPredictedPosition() - Vector3.forward * portalSideOffset;
                }
            }

            GameObject newPortal = Instantiate(portalPrefab, portalPosition, portalRotation);
            newPortal.transform.localScale *= scale;
            activePortals.Add(newPortal);
            Destroy(newPortal, portalLifetime);
        }
    }
}