using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    private Collider hitbox;
    public LayerMask targetLayer;

    private Sword sword;

    private bool parry = false;

    private bool s1 = false;

    private bool s2 = false;

    private bool s3 = false;

    private int damageAmount = 50;

    void Start()
    {
        hitbox = GetComponent<Collider>();
        hitbox.enabled = false;
        sword = GetComponentInParent<Sword>();
    }

    void OnTriggerEnter(Collider collision)
    {
        if (((1 << collision.gameObject.layer) & targetLayer) != 0)
        {
            if (parry)
            {
                damageAmount = 0;
            }
            else if (s1)
            {
                damageAmount = 50;
            }
            else if (s2)
            {
                damageAmount = 75;
            }
            else if (s3)
            {
                damageAmount = 100;
            }

            Slime slimeHealth = collision.gameObject.GetComponent<Slime>();
            if (slimeHealth != null) slimeHealth.TakeDamage(damageAmount);
        }
    }


    public void ParryHitbox()
    {
    parry = true;
    hitbox.enabled = true;
    }

    public void ParryHitboxEnd()
    { 
    parry = false;
    hitbox.enabled = false;
    }

    public void S1Hitbox()
    { 
    s1 = true;
    hitbox.enabled = true;
    }

    public void S1HitboxEnd()
    {
    s1 = false;
    hitbox.enabled = false;
    }

    public void S2Hitbox()
    {
        s2 = true;
        hitbox.enabled = true;
    }

    public void S2HitboxEnd()
    {
        s2 = false;
        hitbox.enabled = false;
    }

    public void S3Hitbox()
    {
        s3 = true;
        hitbox.enabled = true;
    }

    public void S3HitboxEnd()
    {
        s3 = false;
        hitbox.enabled = false;
    }
}
