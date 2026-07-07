using System.Collections;
using PuzzleEditor.InkResources;
using PuzzleEditor.ObjectPool;
using UnityEngine;

namespace PuzzleEditor.Spawners
{
    [RequireComponent(typeof(DropPooler))]

    public class InkSpawner : BaseSpawner<Drop>
    {
        [SerializeField] private Ink _ink;

        private float _delay;
        private int _quantity;
        private Coroutine _spawnRoutine;
        private Vector3 _spawnPosition;
        private WaitForSeconds _waitForSeconds;

        protected override void Awake()
        {
            base.Awake();

            _delay = 0.1f;
            _quantity = 10;
            _waitForSeconds = new WaitForSeconds(_delay);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public void ActivateInkDrops(Color color, WaitForSeconds waitForDuration)
        {
            if (color == null)
            {
                Debug.LogError($"{nameof(ActivateInkDrops)}: Color == null!", this);
            }

            if (_spawnRoutine != null)
            StopCoroutine(_spawnRoutine);

            _spawnRoutine = StartCoroutine(SpawnAndActivateRoutine(color, waitForDuration));
        }

        private IEnumerator SpawnAndActivateRoutine(Color color, WaitForSeconds waitForDuration)
        {
            yield return waitForDuration;

            for (int i = 0; i < _quantity; i++)
            {
                yield return _waitForSeconds;

                SpawnSingleInkDrop(color);
            }
        }

        private void SpawnSingleInkDrop(Color color)
        {
            _spawnPosition = transform.position;

            Drop inkDrop = SpawnObject(_spawnPosition, _ink.transform);

            if (inkDrop.TryGetComponent(out IColorable colorable))
            {
                colorable.SetColor(color);
            }

            if (inkDrop.TryGetComponent(out IDropAnimation animator))
            {
                animator.Play(_spawnPosition);
                inkDrop.PlaySoundSpawn();
            }
        }
    }
}