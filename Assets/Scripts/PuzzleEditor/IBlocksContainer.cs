using System;
using UnityEngine;

namespace PuzzleEditor
{
    public interface IBlocksContainer
    {
        public event Action EverythDestroyed;

        public Transform Transform { get; }
    }
}