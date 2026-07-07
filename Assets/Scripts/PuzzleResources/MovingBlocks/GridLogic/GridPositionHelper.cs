using System.Collections.Generic;
using UnityEngine;

namespace PuzzleResources.MovingBlocks.GridLogic
{
    public class GridPositionHelper
    {
        private readonly GridSystem _gridSystem;

        public GridPositionHelper(GridSystem gridSystem)
        {
            _gridSystem = gridSystem;
        }

        public List<Vector2Int> GetAvailableCenters(Vector2Int blockSize, int marginFromBorder = 0)
        {
            List<Vector2Int> available = new();

            int totalCellsX = _gridSystem.GridSizeX;
            int totalCellsY = _gridSystem.GridSizeY;

            Vector2Int offset = GetCenterOffset(blockSize);

            int minX = offset.x + marginFromBorder;
            int maxX = totalCellsX - (blockSize.x - offset.x) - 1 - marginFromBorder;

            int minY = offset.y + marginFromBorder;
            int maxY = totalCellsY - (blockSize.y - offset.y) - 1 - marginFromBorder;

            if (minX > maxX || minY > maxY)
                return available;

            int width = maxX - minX + 1;
            int total = width * (maxY - minY + 1);

            for (int i = 0; i < total; i++)
            {
                int x = minX + i % width;
                int y = minY + i / width;

                Vector2Int center = new(x, y);
                Vector2Int origin = _gridSystem.GetOriginFromCenter(center, blockSize);

                if (_gridSystem.CanPlaceBlock(origin, blockSize))
                    available.Add(center);
            }

            return available;
        }

        public List<Vector2Int> GetAvailableOrigins(Vector2Int blockSize, int margin = 0)
        {
            List<Vector2Int> available = new();

            for (int x = margin; x <= _gridSystem.GridSizeX - blockSize.x - margin; x++)
            {
                for (int y = margin; y <= _gridSystem.GridSizeY - blockSize.y - margin; y++)
                {
                    Vector2Int origin = new(x, y);

                    if (_gridSystem.CanPlaceBlock(origin, blockSize))
                        available.Add(origin);
                }
            }

            return available;
        }

        private Vector2Int GetCenterOffset(Vector2Int size)
        {
            return new Vector2Int(Mathf.FloorToInt((size.x - 1) / 2f), Mathf.FloorToInt((size.y - 1) / 2f));
        }
    }
}