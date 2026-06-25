using System;
using System.Collections;
using PuzzleEditor.MovingBlocks.GridEditor;
using UnityEngine;

namespace PuzzleEditor.Walls.Partitions
{
    [RequireComponent(typeof(Rigidbody))]
    public class Partition : MonoBehaviour, IGridOccupant
    {
        [SerializeField]
        private Vector2Int _sizeInCells;

        private Rigidbody _rigidbody;
        private bool _isDestroyed;

        public event Action<Partition> Destroyed;

        public Rigidbody Rigidbody => _rigidbody;

        public Vector2Int SizeInCells => _sizeInCells;

        public Vector2Int GridPosition { get; private set; }

        public GameObject GameObject => gameObject;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnDisable()
        {
            Destroyed?.Invoke(this);
        }

        public void SetGridPosition(Vector2Int origin)
        {
            GridPosition = origin;
        }

        public void DestroyPartition()
        {
            if (_isDestroyed)
                return;

            _isDestroyed = true;
            Destroyed?.Invoke(this);

            StartCoroutine(DisableAfterDelay());
        }

        private IEnumerator DisableAfterDelay()
        {
            yield return new WaitForSeconds(1);

            gameObject.SetActive(false);
        }
    }
}