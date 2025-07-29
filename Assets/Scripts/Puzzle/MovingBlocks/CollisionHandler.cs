using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CollisionHandler : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleLayers;

    public int ObstacleCollisionCount { get; private set; }
    public bool IsCollidingWithObstacle => ObstacleCollisionCount > 0;

    private void OnCollisionEnter(Collision collision)
    {
        if (IsObstacle(collision.gameObject))
        {
            ObstacleCollisionCount++;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (IsObstacle(collision.gameObject))
        {
            ObstacleCollisionCount = Mathf.Max(0, ObstacleCollisionCount - 1);
        }
    }

    public bool IsObstacle(GameObject obj)
    {
        return obstacleLayers == (obstacleLayers | (1 << obj.layer));
    }

    public bool CheckPathObstructed(Vector3 position, Vector3 direction, float distance, float radius)
    {
        return Physics.SphereCast(position, radius, direction, out _, distance, 
            obstacleLayers, QueryTriggerInteraction.Ignore);
    }
}