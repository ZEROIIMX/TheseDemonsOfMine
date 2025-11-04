using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    private CapsuleCollider hitbox;
    public LayerMask targetLayer;

    private Sword sword;

    void Start()
    {
        hitbox = GetComponent<CapsuleCollider>();
        hitbox.enabled = true;
        sword = GetComponentInParent<Sword>();
    }

    public void EnableHitbox()
    {
        hitbox.enabled = true;
    }

    public void DisableHitbox()
    {
        hitbox.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            
            Debug.Log("EnemyHit");
            // Add damage logic here

        }
        
    }
}
