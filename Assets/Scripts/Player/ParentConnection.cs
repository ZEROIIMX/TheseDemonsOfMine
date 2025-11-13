using UnityEngine;
using UnityEngine.SceneManagement;

public class ParentConnection : MonoBehaviour
{
    private Sword sword;

    private PlayerController playerController;

    private SwordHitbox swordHitbox;

    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sword = GetComponentInParent<Sword>();
        playerController = GetComponentInParent<PlayerController>();
        swordHitbox = GetComponentInChildren<SwordHitbox>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void TakeSword()
    {
        sword?.PlaceSwordToHand();
    }

    public void SaveSword()
    {
        sword?.PlaceSwordToBack();
    }

    public void S2()
    {
        sword?.Slash2();
    }
    public void S3()
    {
        sword?.Slash3();
    }
    public void S3END()
    {
        playerController?.JumpingSlashFinished();
    }

    public void P()
    {
        sword?.Parry();
    }

    public void PEND()
    {
        sword?.FinishParry();
    }

    public void S1HB()
    {
    swordHitbox?.S1Hitbox();
    }

    public void S1HBEnd()
    {
        swordHitbox?.S1HitboxEnd();
    }

    public void S2HB()
    {  
        swordHitbox?.S2Hitbox();
    }

    public void S2HBEnd()
    {
        swordHitbox.S2HitboxEnd();
    }

    public void S3HB()
    {
        swordHitbox?.S3Hitbox();
    }

    public void S3HBEnd()
    {
        swordHitbox?.S3HitboxEnd();
    }

    public void S3Draw()
    { 
    animator?.SetTrigger("3");
    }

    public void Death()
    {
        animator?.SetTrigger("Death");
    }

    public void End()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
    }

    public void Spawned()
    { 
    playerController?.EnableControls();
    sword?.EnableSword();
    }
}
