using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ScytheAttack : MonoBehaviour
{
    private Sword playerSword;
    public FearEnemy Spawner { get; set; }

    public LayerMask targetLayer;

    private Collider hitbox;

    private void Start()
    {
        hitbox = GetComponentInChildren<Collider>();
        hitbox.enabled = true;
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerSword = playerObject.GetComponent<Sword>();
        }

        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayer) == 0) return;

        if (playerSword.IsParrying())
        {
            hitbox.enabled = false;
            Spawner?.OnParried(transform.position);
            Destroy(gameObject);
        }
        else
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(300);
            }
            Spawner?.OnFailedParry();
        }
    }
}