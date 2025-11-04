using UnityEngine;

public class RootMotionRelay : MonoBehaviour
{
    public Sword sword;

    void Start()
    {
        if (sword == null)
            sword = GetComponentInParent<Sword>();
    }

    void OnAnimatorMove()
    {
        if (sword != null && sword.ShouldApplyRootMotion())
        {
            sword.ApplyRootMotionFromChild(GetComponent<Animator>().deltaPosition, GetComponent<Animator>().deltaRotation);
        }
    }
}
