using System;
using UnityEngine;

namespace PuzzleResources.Spawners
{
    [Serializable]

    public struct ChainSpawnData
    {
        private readonly ChainSpawnDirection _direction;
        private readonly int _count;
        private readonly int _spacing;

        private Vector2Int _startOrigin;
        private Vector2Int _size;

        public readonly Vector2Int StartOrigin => _startOrigin;

        public readonly Vector2Int Size => _size;

        public readonly ChainSpawnDirection Direction => _direction;

        public readonly int Count => _count;

        public readonly int Spacing => _spacing;

        public ChainSpawnData(Vector2Int startOrigin,
            Vector2Int size,
            ChainSpawnDirection direction,
            int count,
            int spacing)
        {
            _startOrigin = startOrigin;
            _size = size;
            _direction = direction;
            _count = count;
            _spacing = spacing;
        }
    }
}