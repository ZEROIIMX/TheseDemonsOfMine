using UnityEngine;

public class MultiplyEnemyConnection : MonoBehaviour
{
    private MultiplyEnemy multiplyEnemy;

    void Start()
    {
        multiplyEnemy = GetComponentInParent<MultiplyEnemy>();
    }

    public void CallDeath()
    {
        multiplyEnemy?.Death();
    }
}
