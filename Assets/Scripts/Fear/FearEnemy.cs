using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;

public class FearEnemy : MonoBehaviour
{
    [Header("Behavior")]
    [SerializeField] private float activationDistance = 30f;
    [SerializeField] private float attackDistance = 3f;
    [SerializeField] private float timeBetweenTeleports = 2f;
    [SerializeField] private bool canTeleport = true;
    [SerializeField] private float portalLifetime = 1.5f;
    [SerializeField] private float finalPortalScale = 2f;
    private float randomPortalScale;

    [Header("References")]
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private GameObject scythePrefab;
    [SerializeField] private GameObject Fear;
    [SerializeField] private float scytheLifetime = 2f;

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private bool isActivated = false;
    private List<GameObject> activePortals = new List<GameObject>();
    private PlayerController playerController;
    private int teleportIndex = 0;

    private bool isParriedOrFailed = false;

    private Vector3 offset = new Vector3(0.5f, 0f, 0f);

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }

        navMeshAgent = GetComponent<NavMeshAgent>();

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

        if (canTeleport)
        {
            StartCoroutine(TeleportAttackSequence());
        }
    }

    private IEnumerator TeleportAttackSequence()
    {
        while (!isParriedOrFailed)
        {
            teleportIndex = 0;

            yield return new WaitForSeconds(timeBetweenTeleports);
            teleportIndex++;
            TeleportTowardsPlayer1();

            yield return new WaitForSeconds(timeBetweenTeleports);
            teleportIndex++;
            TeleportTowardsPlayer2();

            yield return new WaitForSeconds(timeBetweenTeleports);
            teleportIndex++;
            TeleportAndAttack();

            yield return new WaitForSeconds(portalLifetime); // Wait for portal to disappear

            if (!isParriedOrFailed)
            {
                timeBetweenTeleports = Mathf.Max(0.6f, timeBetweenTeleports - 0.2f);
            }
        }
    }


    private Vector3 GetPlayerPredictedPosition()
    {
        if (playerController != null)
        {
            return player.position + playerController.GetVelocity() * Time.deltaTime;
        }
        return player.position;
    }

    private void TeleportTowardsPlayer1()
    {
        if (player == null) return;

        Vector3 predictedPlayerPosition = GetPlayerPredictedPosition();
        Vector3 directionToPlayer = (transform.position - predictedPlayerPosition).normalized;
        if (transform.position.x - predictedPlayerPosition.x > 0)
        {
            directionToPlayer = new Vector3(-directionToPlayer.x, directionToPlayer.y, directionToPlayer.z);
        }

        Vector3 targetPosition = predictedPlayerPosition +
            Vector3.right * Random.Range(.5f, -2f) +   // global X offset
            Vector3.up * Random.Range(1f, 5f) +        // global Y offset
            Vector3.forward * Random.Range(-2f, -6f);  // global Z offset
            randomPortalScale = Random.Range(.5f, 1f);

        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
        {
            CreatePortal(hit.position, randomPortalScale);
        }
        else
        {
            CreatePortal(targetPosition, randomPortalScale);
        }
    }

    private void TeleportTowardsPlayer2()
    {
        if (player == null) return;

        Vector3 predictedPlayerPosition = GetPlayerPredictedPosition();
        Vector3 directionToPlayer = (transform.position - predictedPlayerPosition).normalized;
        if (transform.position.x - predictedPlayerPosition.x > 0)
        {
            directionToPlayer = new Vector3(-directionToPlayer.x, directionToPlayer.y, directionToPlayer.z);
        }

        Vector3 targetPosition = predictedPlayerPosition +
            Vector3.right * Random.Range(.5f, -2f) +   // global X offset
            Vector3.up * Random.Range(1f, 5f) +        // global Y offset
            Vector3.forward * Random.Range(2f, 6f);  // global Z offset
            randomPortalScale = Random.Range(1f, 1.5f);

        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
        {
            CreatePortal(hit.position, randomPortalScale);
        }
        else
        {
            CreatePortal(targetPosition, randomPortalScale);
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

        StartCoroutine(SpawnScytheWithDelay(finalPosition));

    }

    private IEnumerator SpawnScytheWithDelay(Vector3 finalPosition)
    {
        yield return new WaitForSeconds(0.2f);

        if (scythePrefab != null)
        {
            Vector3 raisedPosition = finalPosition + Vector3.up * 1.5f;

            GameObject scytheInstance = Instantiate(scythePrefab, raisedPosition, Quaternion.LookRotation(player.position - raisedPosition));

            ScytheAttack scytheAttack = scytheInstance.GetComponent<ScytheAttack>();
            if (scytheAttack != null)
            {
                scytheAttack.Spawner = this;
            }

            Destroy(scytheInstance, scytheLifetime);
        }
    }

    public void OnParried(Vector3 scythePosition)
    {
        isParriedOrFailed = true;
        //Spawn enemy prefab
        Vector3 raisedPosition = scythePosition + Vector3.up * 1.5f;
        GameObject Fear = Instantiate(this.Fear, scythePosition + offset, Quaternion.LookRotation(player.position - raisedPosition));
    }

    public void OnFailedParry()
    {
        isParriedOrFailed = true;
        StartCoroutine(DelayedDestroy());
    }

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    private void CreatePortal(Vector3 enemyTeleportPosition, float scale)
    {
        if (portalPrefab != null)
        {
            Vector3 portalPosition = enemyTeleportPosition;
            Quaternion portalRotation = transform.rotation;

            GameObject newPortal = Instantiate(portalPrefab, portalPosition, portalRotation);
            newPortal.transform.localScale *= scale;
            activePortals.Add(newPortal);
            Destroy(newPortal, portalLifetime);
        }
    }
}