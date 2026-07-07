using System;
using UnityEngine;

namespace PuzzleResources
{
    public interface IBlocksContainer
    {
        public event Action EverythingDestroyed;

        public Transform Transform { get; }
    }
}