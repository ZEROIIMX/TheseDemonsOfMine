using System.Collections;
using UnityEngine;

public class StressCollision : MonoBehaviour
{
    public Collider LTrigger;
    public Collider RTrigger;
    public Collider LCollider;
    public Collider RCollider;

    private StressConnection stressConnection;

    public LayerMask targetLayer;

    private float damage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LTrigger.enabled = false;
        RTrigger.enabled = false;

        LCollider.enabled = false;
        RCollider.enabled = false;

        stressConnection = GetComponentInParent<StressConnection>();
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
                    A1END();
                    A2END();
                    A3END();
                    sword.ParryTime();
                    return;
            }

            else
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    Collision();
                }
            }
        }
    }

    public void A1()
    {
        RCollider.enabled = true;
        RTrigger.enabled = true;
        damage = 10;
    }

    public void A1END()
    {
        RCollider.enabled = false;
        RTrigger.enabled = false;
    }

    public void A2()
    {
        LCollider.enabled = true;
        LTrigger.enabled = true;
        damage = 15;
    }

    public void A2END()
    {
        LCollider.enabled = false;
        LTrigger.enabled = false;
    }

    public void A3()
    {
        RCollider.enabled = true;
        RTrigger.enabled = true;
        LCollider.enabled = true;
        LTrigger.enabled = true;
        damage = 20;
    }

    public void A3END()
    {
        RCollider.enabled = false;
        RTrigger.enabled = false;
        LCollider.enabled = false;
        LTrigger.enabled = false;
    }

    private void Collision()
    {
        StartCoroutine(DelayedResetCollision());
    }

    IEnumerator DelayedResetCollision()
    {
        yield return new WaitForSeconds(0.01f);
        RCollider.enabled = false;
        LCollider.enabled = false;
    }
}
