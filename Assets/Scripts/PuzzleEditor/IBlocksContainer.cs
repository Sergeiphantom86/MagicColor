using System;
using UnityEngine;

namespace PuzzleEditor
{
    public interface IBlocksContainer
    {
        public event Action EverythingDestroyed;

        public Transform Transform { get; }
    }
}