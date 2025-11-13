using System.Collections;
using UnityEngine;

public class FearHitbox : MonoBehaviour
{
    public Collider hitbox;
    public Collider hitbox2;

    public LayerMask targetLayer;

    private float damage = 25f;

    void Start()
    {
        hitbox = GetComponent<Collider>();
        hitbox2 = GetComponent<Collider>();

        if (hitbox != null)
        {
            hitbox.isTrigger = true;
            hitbox.enabled = false;
        }

        if (hitbox2 != null)
        {
            hitbox2.isTrigger = true;
            hitbox2.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayer) == 0) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            Sword sword = player.GetComponent<Sword>();

            if (sword != null && sword.isParrying)
            {
                FOFF();
                sword.ParryTime();
                return;
            }

            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Collision();
            }
        }
    }

    public void FON()
    {
        if (hitbox != null) hitbox.enabled = true;
        if (hitbox2 != null) hitbox2.enabled = true;
    }

    public void FOFF()
    {
        if (hitbox != null) hitbox.enabled = false;
        if (hitbox2 != null) hitbox2.enabled = false;
    }

    private void Collision()
    {
        if (hitbox != null) hitbox.enabled = false;
        if (hitbox2 != null) hitbox2.enabled = false;
    }
}
