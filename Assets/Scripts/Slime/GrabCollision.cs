using System;
using UnityEngine;

public class GrabCollision : MonoBehaviour
{
    public Collider grabLCollider;
    public Collider grabRCollider;
    private SlimeConnection slimeConnection;

    public LayerMask targetLayer;

    void Start()
    {
        grabLCollider.enabled = false;
        grabRCollider.enabled = false;

        grabLCollider.isTrigger = true;
        grabRCollider.isTrigger = true;

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
                DeactivateGrab();
                sword.ParryTime();
            }
            else
            {
                slimeConnection?.SuccesfulGrab(player);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    { 
    Debug.Log("Collision Detected");
    }

    public void ActivateGrab()
    {
        grabLCollider.isTrigger = true;
        grabRCollider.isTrigger = true;
        grabLCollider.enabled = true;
        grabRCollider.enabled = true;
    }

    public void DeactivateGrab()
    {
        grabLCollider.enabled = false;
        grabRCollider.enabled = false;
    }

    public void ActivateRPunch()
    {
        grabRCollider.isTrigger = false;
        grabRCollider.enabled = true;
    }

    public void DeactivateRPunch()
    {
        grabRCollider.enabled = false;
    }

    public void ActivateLPunch()
    {
        grabLCollider.isTrigger = false;
        grabLCollider.enabled = true;
    }

    public void DeactivateLPunch()
    {
        grabLCollider.enabled = false;
    }
}
