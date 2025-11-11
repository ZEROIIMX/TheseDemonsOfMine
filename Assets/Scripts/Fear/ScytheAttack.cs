using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ScytheAttack : MonoBehaviour
{
    private Sword playerSword;
    private bool hasAttacked = false;
    public FearEnemy Spawner { get; set; }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerSword = playerObject.GetComponent<Sword>();
        }

        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasAttacked || !other.CompareTag("Player") || playerSword == null)
        {
            return;
        }

        hasAttacked = true;
            if (playerSword.IsParrying())
        {
            Spawner?.OnScytheParried(transform.position);
            Destroy(gameObject);
        }
        else
        {
            Destroy(other.gameObject);
            Spawner?.OnPlayerFailedParry();
        }
    }
}