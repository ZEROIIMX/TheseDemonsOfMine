using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Slime))]
public class SlimeTarget : MonoBehaviour
{
    public Transform Target;
    public float AttackDistance;
    public float ActivationDistance;

    private NavMeshAgent agent;
    private Animator m_animator;
    private Slime slime;
    private bool isChasing = false;
    private bool Activated = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponentInChildren<Animator>();
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
                m_animator.SetBool("Walk", false);
            }
            return;
        }

        if (distance <= AttackDistance)
        {
            if (isChasing)
            {
                isChasing = false;
                slime.SetChaseState(false);
                m_animator.SetBool("Walk", false);
            }
            agent.isStopped = true;
        }
        else
        {
            if (!isChasing)
            {
                isChasing = true;
                slime.SetChaseState(true);
                m_animator.SetBool("Walk", true);
            }
            agent.isStopped = false;
            agent.destination = Target.position;
        }
    }
}
