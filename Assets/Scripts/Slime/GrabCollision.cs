using System;
using UnityEngine;

public class GrabCollision : MonoBehaviour
{
    public Collider grabLCollider;
    public Collider grabRCollider;
    public Collider punchLCollider;
    public Collider punchRCollider;
  
    private SlimeConnection slimeConnection;

    public LayerMask targetLayer;

    private bool grab = false;

    void Start()
    {
        grabLCollider.enabled = false;
        grabRCollider.enabled = false;

        punchLCollider.enabled = false;
        punchRCollider.enabled = false;

        slimeConnection = GetComponentInParent<SlimeConnection>();
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
                if (grab)
                {
                    DeactivateGrab();
                    sword.ParryTime();
                }
                else
                {
                    DeactivateLPunch();
                    DeactivateRPunch();
                    sword.ParryTime();
                    return;
                }
            }
            if (grab)
            {
                slimeConnection?.SuccesfulGrab(player);
            }
            else
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(30);
                }
            }
        }
    }

    public void ActivateGrab()
    {
        grabLCollider.enabled = true;
        grabRCollider.enabled = true;
        grab = true;
    }

    public void DeactivateGrab()
    {
        grabLCollider.enabled = false;
        grabRCollider.enabled = false;
        grab = false;
    }

    public void ActivateRPunch()
    {
        punchRCollider.enabled = true;
        grabRCollider.enabled = true;
    }

    public void DeactivateRPunch()
    {
        punchRCollider.enabled = false;
        grabRCollider.enabled = false;
    }

    public void ActivateLPunch()
    {
        punchLCollider.enabled = true;
        grabLCollider.enabled = true;
    }

    public void DeactivateLPunch()
    {
        punchLCollider.enabled = false;
        grabLCollider.enabled = false;
    }
}
