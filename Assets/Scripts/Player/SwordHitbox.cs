using UnityEngine;
using System.Collections;

public class SwordHitbox : MonoBehaviour
{
    private Collider hitbox;
    public LayerMask targetLayer;

    private Sword sword;
    private PlayerController playerController;

    private bool parry = false;
    private bool s1 = false;
    private bool s2 = false;
    private bool s3 = false;

    private int damageAmount;
    public float slashForce = 10f;

    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        sword = GetComponentInParent<Sword>();
        hitbox = GetComponent<Collider>();
        hitbox.enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & targetLayer) == 0) return;

        if (parry) damageAmount = 0;
        else if (s1) damageAmount = 50;
        else if (s2) damageAmount = 75;
        else if (s3) damageAmount = 100;

        var hitDirection = (collision.transform.position - playerController.transform.position).normalized;

        Slime slimeHealth = collision.gameObject.GetComponent<Slime>();
        if (slimeHealth != null)
        {
            if (!parry)
            {
                Vector3 hitPoint = collision.contacts[0].point;
                slimeHealth.TakeDamage(damageAmount, hitPoint);
            }
            if (slimeHealth.rb != null)
            {
                slimeHealth.rb.AddForce(hitDirection * slashForce, ForceMode.Impulse);
                slimeHealth.TakeForce();
            }
            return;
        }

        MultiplyEnemy multiplyEnemy = collision.gameObject.GetComponent<MultiplyEnemy>();
        if (multiplyEnemy != null)
        {
            if (!parry)
            {
                Vector3 hitPoint = collision.contacts[0].point;
                multiplyEnemy.TakeDamage(damageAmount, hitPoint);
            }
            if (multiplyEnemy.rb != null)
            {
                multiplyEnemy.rb.AddForce(hitDirection * (slashForce * 0.6f), ForceMode.Impulse);
                multiplyEnemy.TakeForce();
            }
        }


        FearMovement fearEnemy = collision.gameObject.GetComponent<FearMovement>();
        if (fearEnemy != null)
        {
            if (!parry)
            {
                Vector3 hitPoint = collision.contacts[0].point;
                fearEnemy.TakeDamage(damageAmount, hitPoint);
            }
            if (fearEnemy.rb != null)
            {
                fearEnemy.rb.AddForce(hitDirection * (slashForce * 0.6f), ForceMode.Impulse);
                fearEnemy.TakeForce();
            }
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