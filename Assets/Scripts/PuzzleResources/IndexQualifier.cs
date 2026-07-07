using System.Collections;
using System.Collections.Generic;
using PuzzleResources.Spawners;
using PuzzleResources.Walls;
using UnityEngine;

namespace PuzzleResources
{
    public class IndexQualifier : MonoBehaviour
    {
        private readonly List<int> _allowedIndexes = new();

        private BlockSpawner _spawner;
        private PuzzlesIdentifier _identifier;

        private void Awake()
        {
            _spawner = GetComponentInChildren<BlockSpawner>();
            _identifier = GetComponent<PuzzlesIdentifier>();

            if (_spawner == null || _identifier == null)
            {
                Debug.LogError("IndexQualifier: dependencies missing", this);
                enabled = false;
                return;
            }

            _spawner.SpawnerReadyed += OnSpawnerReady;
        }

        private void OnDestroy()
        {
            if (_spawner != null)
            _spawner.SpawnerReadyed -= OnSpawnerReady;
        }

        private void Start()
        {
            StartCoroutine(InitAfterPuzzleCreated());
        }

        private IEnumerator InitAfterPuzzleCreated()
        {
            yield return null;

            BuildAllowedIndexes();

            _spawner.SetIndexProvider(GetFilteredIndex);

            _spawner.SpawnNecessaryBlocks();
        }

        private void BuildAllowedIndexes()
        {
            _allowedIndexes.Clear();

            int totalBlocks = _spawner.Count;
            for (int i = 0; i < totalBlocks; i++)
            _allowedIndexes.Add(i);

            var container = _identifier.CurrentContainer;
            if (container == null)
            return;

            int gridSize = container.GridSize.y;

            if (gridSize <= 13)
            {
                _allowedIndexes.Remove(0);
                _allowedIndexes.Remove(3);
            }
        }

        private int GetFilteredIndex()
        {
            return _allowedIndexes[Random.Range(0, _allowedIndexes.Count)];
        }

        private void OnSpawnerReady()
        {
            BuildAllowedIndexes();

            _spawner.SetIndexProvider(GetFilteredIndex);
        }
    }
}