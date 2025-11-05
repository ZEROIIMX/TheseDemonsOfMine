using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class AIUnit : MonoBehaviour
{
    public NavMeshAgent Agent;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }
    public void MoveTo(Vector3 position)
    {
        if (Agent.isOnNavMesh && Agent.isActiveAndEnabled)
        {
            Agent.SetDestination(position);
        }
    }
}
