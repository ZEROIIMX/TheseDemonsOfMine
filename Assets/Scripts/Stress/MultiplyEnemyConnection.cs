using UnityEngine;

public class MultiplyEnemyConnection : MonoBehaviour
{
    private MultiplyEnemy multiplyEnemy;

    void Start()
    {
        Debug.Log("MultiplyEnemyConnection Start called");
        multiplyEnemy = GetComponentInParent<MultiplyEnemy>();
    }

    public void CallDeath()
    {
        multiplyEnemy?.Death();
    }
}
