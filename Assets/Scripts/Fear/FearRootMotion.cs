using UnityEngine;

public class FearRootMotion : MonoBehaviour
{
    public FearDamage fearDamage;

    void Start()
    {
        if (fearDamage == null)
            fearDamage = GetComponentInParent<FearDamage>();
    }

    void OnAnimatorMove()
    {
        if (fearDamage != null && fearDamage.ShouldApplyRootMotion())
        {
            fearDamage.ApplyRootMotionFromChild(GetComponent<Animator>().deltaPosition, GetComponent<Animator>().deltaRotation);
        }
    }
}