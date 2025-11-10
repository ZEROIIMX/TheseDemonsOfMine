using UnityEngine;

public class StressCollision : MonoBehaviour
{
    public Collider LTrigger;
    public Collider RTrigger;
    public Collider LCollider;
    public Collider RCollider;

    private StressConnection stressConnection;

    public LayerMask targetLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LTrigger.enabled = false;
        RTrigger.enabled = false;

        LCollider.enabled = false;
        RCollider.enabled = false;

        stressConnection = GetComponentInParent<StressConnection>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Detected");
        if (((1 << other.gameObject.layer) & targetLayer) == 0) return;
    }

    public void A1()
    {
        RCollider.enabled = true;
        RTrigger.enabled = true;
    }

    public void A1END()
    {
        RCollider.enabled = false;
        RTrigger.enabled = false;
    }

    public void A2()
    {
        LCollider.enabled = true;
        LTrigger.enabled = true;
    }

    public void A2END()
    {
        LCollider.enabled = false;
        LTrigger.enabled = false;
    }

    public void A3()
    {
        RCollider.enabled = true;
        RTrigger.enabled = true;
        LCollider.enabled = true;
        LTrigger.enabled = true;
    }

    public void A3END()
    {
        RCollider.enabled = false;
        RTrigger.enabled = false;
        LCollider.enabled = false;
        LTrigger.enabled = false;
    }
}
