using UnityEngine;

public class SlimeConnection : MonoBehaviour
{
    private Slime slime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slime = GetComponentInParent<Slime>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SlimeDie()
    {
        slime?.Death();
    }
}
