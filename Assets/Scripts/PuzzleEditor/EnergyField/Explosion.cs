using PuzzleEditor.Walls.Partitions;
using UnityEngine;

namespace PuzzleEditor.EnergyField
{
    public class Explosion : MonoBehaviour
    {
        [SerializeField]
        private float _radius = 5f;

        [SerializeField]
        private float _force = 700f;

        [SerializeField]
        private float _upwardModifier = 1f;

        [SerializeField]
        private LayerMask _affectedLayers;

        private Collider[] _colliderBuffer;

        private void Awake()
        {
            _colliderBuffer = new Collider[100];
        }

        public void Explode()
        {
            int hitCount = Physics.OverlapSphereNonAlloc(
                transform.position,
                _radius,
                _colliderBuffer,
                _affectedLayers
            );

            for (int i = 0; i < hitCount; i++)
            {
                Collider collider = _colliderBuffer[i];
                ProcessCollider(collider);
            }
        }

        private void ProcessCollider(Collider collider)
        {
            Rigidbody rigidbody = collider.attachedRigidbody;

            if (collider.TryGetComponent(out Partition partition))
            {
                rigidbody = partition.Rigidbody;

                if (rigidbody != null)
                {
                    rigidbody.useGravity = true;
                    rigidbody.isKinematic = false;
                }
            }

            if (rigidbody != null)
            {
                rigidbody.AddExplosionForce(
                    _force,
                    transform.position,
                    _radius,
                    _upwardModifier,
                    ForceMode.Impulse
                );
            }

            if (partition != null)
            {
                partition.DestroyPartition();
            }
        }
    }
}