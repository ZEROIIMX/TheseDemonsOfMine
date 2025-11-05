using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIUnit))]
public class MultiplyEnemy : MonoBehaviour
{
    [Header("Group Settings")]
    public GameObject enemyCopyPrefab;
    public int numberOfCopies = 5;
    public float spawnRadius = 3f;

    [Header("Behavior Settings")]
    public float circleFormationDistance = 15f;
    public float circleRadius = 8f;

    private List<AIUnit> groupUnits = new List<AIUnit>();
    private Transform player;
    private bool isCircling = false;

    void Start()
    {
        player = AIManager.Instance.Target;
        if (player == null)
        {
            enabled = false;
            return;
        }

        groupUnits.Add(GetComponent<AIUnit>());

        for (int i = 0; i < numberOfCopies; i++)
        {
            SpawnCopy();
        }
    }

    void Update()
    {
        if (isCircling)
        {
            AIManager.Instance.MakeAgentsCircleTarget(groupUnits, circleRadius);
        }
        else
        {
            if (Vector3.Distance(transform.position, player.position) <= circleFormationDistance)
            {
                isCircling = true;
            }
            else
            {
                foreach (var unit in groupUnits)
                {
                    if (unit != null)
                    {
                        unit.MoveTo(player.position);
                    }
                }
            }
        }
    }

    void SpawnCopy()
    {
        if (enemyCopyPrefab == null) return;

        Vector3 spawnPosition = transform.position + (Vector3)Random.insideUnitCircle * spawnRadius;
        GameObject copy = Instantiate(enemyCopyPrefab, spawnPosition, Quaternion.identity);
        AIUnit aiUnit = copy.GetComponent<AIUnit>();
        if (aiUnit != null)
        {
            groupUnits.Add(aiUnit);
        }
    }
}
