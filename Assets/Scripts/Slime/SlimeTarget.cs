using System.Collections;
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
    private bool activated = false;

    [SerializeField] private float grabCooldown = 3f;
    private bool grab = true;
    private Coroutine grabCooldownRoutine;

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

        if (!activated)
        {
            if (distance > ActivationDistance) return;
            activated = true;
        }

        if (slime.IsBusy())
        {
            if (isChasing)
            {
                isChasing = false;
                activated = false;
                m_animator.SetBool("Walk", false);
            }
            return;
        }

        if (distance <= AttackDistance && grab)
        {
            if (isChasing)
            {
                isChasing = false;
                slime.SetChaseState(false);
                m_animator.SetBool("Walk", false);
                m_animator.SetTrigger("Grab");
                grab = false;

                if (grabCooldownRoutine == null)
                {
                    grabCooldownRoutine = StartCoroutine(GrabCooldown());
                }
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

    private IEnumerator GrabCooldown()
    {
        yield return new WaitForSeconds(grabCooldown);
        grabCooldownRoutine = null;
        grab = true;
    }
}
