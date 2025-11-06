using System;
using UnityEngine;

public class GrabCollision : MonoBehaviour
{
    private Collider grabCollider;

    private SlimeConnection slimeConnection;

    public LayerMask targetLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grabCollider = GetComponent<Collider>();
        grabCollider.enabled = false;
        slimeConnection = GetComponentInParent<SlimeConnection>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            slimeConnection?.SuccesfulGrab(player);
        }
    }

    public void ActivateGrab()
    {
        grabCollider.enabled = true;
    }

    public void DeactivateGrab()
    {
        grabCollider.enabled = false;
    }
}
