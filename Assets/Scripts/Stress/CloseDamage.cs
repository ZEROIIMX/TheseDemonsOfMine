using UnityEngine;

public class CloseDamage : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackDistance = 3f;
    [SerializeField] private float attackCooldown = 2f;

    private Transform player;
    private float cooldownTimer;

    private Animator m_animator;

    void Start()
    {
        m_animator = GetComponentInChildren<Animator>();
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
        int attackIndex = Random.Range(0, 3);

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

        cooldownTimer = attackCooldown;
    }

    private void A1()
    {
        m_animator.SetTrigger("1");
    }

    private void A2()
    {
        m_animator.SetTrigger("2");
    }

    private void A3()
    {
        m_animator.SetTrigger("3");
    }
}

