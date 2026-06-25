using PuzzleEditor.MovingBlocks.GridEditor;
using PuzzleEditor.Walls.Partitions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleEditor.Spawners
{
    public class PartitionChainSpawner
    {
        private readonly GridSystem _grid;

        public PartitionChainSpawner(GridSystem grid)
        {
            _grid = grid;
        }

        public void TrySpawnChain(ChainSpawnData chainSpawnData, Func<Partition> spawnFunc, Action<Partition, Vector2Int> placeFunc)
        {
            List<Vector2Int> directions = GetDirections(chainSpawnData.Direction);

            foreach (var direction in directions)
            {
                TrySpawnSingleDirectionChain(chainSpawnData, direction, spawnFunc, placeFunc);
            }
        }

        private void TrySpawnSingleDirectionChain(ChainSpawnData chainSpawnData, Vector2Int direction, Func<Partition> spawnFunc, Action<Partition, Vector2Int> placeFunc)
        {
            Vector2Int currentOrigin = chainSpawnData.StartOrigin;

            for (int i = 0; i < chainSpawnData.Count; i++)
            {
                if (TrySpawnNext(chainSpawnData, direction, spawnFunc, placeFunc) == false)
                    break;
            }
        }

        private bool TrySpawnNext(ChainSpawnData chainSpawnData, Vector2Int direction, Func<Partition> spawnFunc, Action<Partition, Vector2Int> placeFunc)
        {
            Vector2Int nextOrigin = chainSpawnData.StartOrigin + direction * chainSpawnData.Spacing;

            if (_grid.CanPlaceBlock(nextOrigin, chainSpawnData.Size) == false)
                return false;

            Partition partition = spawnFunc();

            if (partition == null)
                return false;

            placeFunc(partition, nextOrigin);

            chainSpawnData.StartOrigin = nextOrigin;

            return true;
        }

        private List<Vector2Int> GetDirections(ChainSpawnDirection direction)
        {
            return direction switch
            {
                ChainSpawnDirection.X => new()
            {
                Vector2Int.right,
            },

                ChainSpawnDirection.Y => new()
            {
                Vector2Int.up,
            },

                ChainSpawnDirection.Diagonal => new()
            {
                new Vector2Int(1, 1),
                new Vector2Int(1, -1),
            },

                ChainSpawnDirection.Both => new()
            {
                Vector2Int.right,
                Vector2Int.up,
            },

                ChainSpawnDirection.All => new()
            {
                Vector2Int.right,
                Vector2Int.up,
                new Vector2Int(1, 1),
                new Vector2Int(1, -1),
            },

                _ => new()
            };
        }
    }
}