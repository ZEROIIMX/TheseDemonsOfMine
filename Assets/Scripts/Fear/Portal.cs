using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] Transform destination;
    [SerializeField] private float spawnHeight = 1.0f;

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, spawnHeight, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent<PlayerController>(out var player))
        {
            player.Teleport(destination.position, destination.rotation);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(destination.position, 0.4f);
        var direction = destination.TransformDirection(Vector3.forward);
        Gizmos.DrawRay(destination.position, direction);
    }
}
