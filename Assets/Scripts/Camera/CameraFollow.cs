using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float sideOffsetZ = -10f;     // Negative = left of player (world Z), Positive = right
    public float heightOffset = 5f;      // Above player
    public float depthOffsetX = -10f;    // Behind or in front of player (world X)
    public float followSpeed = 5f;
    public bool lookAtTarget = true;

    void LateUpdate()
    {
        if (target == null) return;

        // ✅ Fixed world-space offsets
        Vector3 sideOffset = Vector3.forward * sideOffsetZ;
        Vector3 depthOffset = Vector3.right * depthOffsetX;
        Vector3 heightOffsetVec = Vector3.up * heightOffset;

        Vector3 desiredPosition = target.position + sideOffset + depthOffset + heightOffsetVec;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.unscaledDeltaTime);

        if (lookAtTarget)
        {
            transform.LookAt(target.position + Vector3.up * 1.5f); // Optional: look slightly above the player
        }
    }
}
