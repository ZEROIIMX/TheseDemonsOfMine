using UnityEngine;

public class CloseDamage : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackDistance = 3f;
    [SerializeField] private float attackCooldown = 2f;

    private Transform player;
    private float cooldownTimer;

    void Start()
    {
        player = AIManager.Instance.Target;
        cooldownTimer = 0;
    }

    void Update()
    {
        if (player == null) return;

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackDistance)
            {
                Attack();
            }
        }
    }

    private void Attack()
    {
        if (player.TryGetComponent<PlayerHealth>(out var playerHealth))
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log($"{gameObject.name} attacked the player for {attackDamage} damage.");
        }
        cooldownTimer = attackCooldown;
    }
}

