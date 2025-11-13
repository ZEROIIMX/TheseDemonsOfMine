using UnityEngine;

public class FearConnection : MonoBehaviour
{
    private FearDamage fearDamage;

    private FearMovement fearMovement;

    private FearHitbox fearHitbox;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fearDamage = GetComponentInParent<FearDamage>();
        fearMovement = GetComponentInParent<FearMovement>();
        fearHitbox = GetComponentInChildren<FearHitbox>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void F()
    {
        fearHitbox?.FON();
    }

    public void FE()
    {
        fearHitbox?.FOFF();
    }

    public void Death()
    {
        fearMovement?.DestroySelf();
    }

    public void ResetCd()
    { 
    fearDamage?.ResetCooldown();
    }

    public void Track()
    {
        fearMovement?.ResumeTracking();
    }

}
