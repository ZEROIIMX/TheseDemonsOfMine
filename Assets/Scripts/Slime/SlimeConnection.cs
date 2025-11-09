using UnityEngine;

public class SlimeConnection : MonoBehaviour
{
    private Slime slime;

    private GrabCollision GrabCollision;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slime = GetComponentInParent<Slime>();
        GrabCollision = GetComponentInChildren<GrabCollision>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SlimeDie()
    {
        slime?.Death();
    }

    public void GrabOn()
    {
        GrabCollision.ActivateGrab();
    }

    public void GrabOff() 
    { 
        GrabCollision.DeactivateGrab(); 
    }

    public void SuccesfulGrab(PlayerController player)
    {
        slime?.Grab(player);
    }

    public void RPunchOn()
    {
        GrabCollision?.ActivateRPunch();
    }

    public void RPunchOff()
    {
        GrabCollision?.DeactivateRPunch();
    }

    public void LPunchOn()
    { 
    GrabCollision?.ActivateLPunch();
    }

    public void LPunchOff()
    { 
    GrabCollision?.DeactivateLPunch();
    }
}
