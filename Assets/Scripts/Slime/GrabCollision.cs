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
                sword.ParryTime();
                DeactivateGrab();
            }
            else
            {
                slimeConnection?.SuccesfulGrab(player);
            }
        }
    }

    public void ActivateGrab()
    {
        grabLCollider.enabled = true;
        grabRCollider.enabled = true;
    }

    public void DeactivateGrab()
    {
        grabLCollider.enabled = false;
        grabRCollider.enabled = false;
    }
}
