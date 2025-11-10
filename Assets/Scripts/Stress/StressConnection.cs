using UnityEngine;

public class StressConnection : MonoBehaviour
{
    private MultiplyEnemy multiplyEnemy;

    private StressCollision stressCollision;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        multiplyEnemy = GetComponentInParent<MultiplyEnemy>();
        stressCollision = GetComponentInChildren<StressCollision>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void A1ON()
    { 
        stressCollision?.A1();
    }

    public void A1OFF()
    {
        stressCollision?.A1END();
    }

    public void A2ON()
    {
        stressCollision?.A2();
    }

    public void A2OFF()
    {
        stressCollision?.A2END();
    }

    public void A3ON()
    {
        stressCollision?.A3();
    }

    public void A3OFF()
    {
        stressCollision?.A3END();
    }

    public void Death()
    {
        multiplyEnemy?.Destroy();
    }

    public void Tracking()
    {
        multiplyEnemy?.ResumeTracking();
    }
}
