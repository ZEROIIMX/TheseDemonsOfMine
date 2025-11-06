using UnityEngine;

public class SlimeHitbox : MonoBehaviour
{
    public Slime slime;

    private int Health = 10;

    public void TakeDamage(int damageAmount)
    {
        Debug.Log($"Slime took {damageAmount} damage.");
        Health -= damageAmount;
        if (Health <= 0) Die();
    }

    public void Die()
    {
        Destroy(slime.gameObject);
    }
}
