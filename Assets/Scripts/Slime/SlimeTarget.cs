using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Slime))]
public class SlimeTarget : MonoBehaviour
{
    public Transform Target;
    public float AttackDistance;

    private NavMeshAgent agent;
    private Animator animator;
    private Slime slime;
    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        slime = GetComponent<Slime>();

        if (Target == null && AIManager.Instance != null)
        {
            Target = AIManager.Instance.Target;
        }
    }

    void Update()
    {
        if (Target == null) return;

        if (slime.IsBusy())
        {
            if (isChasing)
            {
                isChasing = false;
            }
            return;
        }

        float distance = Vector3.Distance(transform.position, Target.position);

        if (distance <= AttackDistance)
        {
            if (isChasing)
            {
                isChasing = false;
                slime.SetChaseState(false);
            }
            agent.isStopped = true;
        }
        else
        {
            if (!isChasing)
            {
                isChasing = true;
                slime.SetChaseState(true);
            }
            agent.isStopped = false;
            agent.destination = Target.position;
        }
    }
}
