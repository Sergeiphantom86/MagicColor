using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float _radius = 5f;
    [SerializeField] private float _force = 700f;
    [SerializeField] private float _upwardModifier = 1f;
    [SerializeField] private LayerMask _affectedLayers;

    public void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position,
                                                     _radius,
                                                     _affectedLayers);
        Wait(colliders);
    }

    private void Wait(Collider[] collider)
    {
        foreach (Collider col in collider)
        {
            if (col.TryGetComponent(out Partition partition))
            {
                Rigidbody rigidbody = partition.Rigidbody;
                rigidbody.useGravity = true;
                rigidbody.isKinematic = false;
            }

            if (col.attachedRigidbody == null)
                continue;

            col.attachedRigidbody.AddExplosionForce(_force, transform.position, _radius, _upwardModifier, ForceMode.Impulse);

            partition.DestroyPartition();
        }
    }
}