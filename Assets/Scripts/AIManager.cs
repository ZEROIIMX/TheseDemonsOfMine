using System.Collections.Generic;
using UnityEngine;


[DefaultExecutionOrder(0)]
public class AIManager : MonoBehaviour
{
    private static AIManager _instance;
    public static AIManager Instance
    {
        get
        {
            return _instance;
        }
        private set { _instance = value; }
    }

    public Transform Target;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            if (Target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    Target = player.transform;
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MakeAgentsCircleTarget(List<AIUnit> group, float radius)
    {
        if (Target == null || group.Count == 0) return;

        for (int i = 0; i < group.Count; i++)
        {
            AIUnit unit = group[i];
            if (unit != null)
            {
                float angle = i * (2 * Mathf.PI / group.Count);
                Vector3 circlePosition = new Vector3(
                    Target.position.x + radius * Mathf.Cos(angle),
                    Target.position.y,
                    Target.position.z + radius * Mathf.Sin(angle)
                );
                unit.MoveTo(circlePosition);
            }
        }
    }
}
