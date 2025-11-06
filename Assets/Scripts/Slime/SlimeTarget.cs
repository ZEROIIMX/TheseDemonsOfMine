using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Slime))]
public class SlimeTarget : MonoBehaviour
{
    public Transform Target;
    public float AttackDistance;
    public float ActivationDistance;

    private NavMeshAgent agent;
    private Animator animator;
    private Slime slime;
    private bool isChasing = false;
    private bool Activated = false;

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

        float distance = Vector3.Distance(transform.position, Target.position);

        if (!Activated)
        {
            if (distance > ActivationDistance)
            {
                return;
            }
            else
            {
                Activated = true;
            }
        }

        if (slime.IsBusy())
        {
            if (isChasing)
            {
                isChasing = false;
                Activated = false;
            }
            return;
        }

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
