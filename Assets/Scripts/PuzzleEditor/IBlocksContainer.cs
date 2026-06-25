using System;
using UnityEngine;
namespace PuzzleEditor
{

public interface IBlocksContainer
{
    public event Action EverythDestroyed;

    public int ActiveBlocksCount { get; }

    public Transform Transform { get; }
}
}