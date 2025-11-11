using System;
using UnityEngine;

public interface IBlocksContainer
{
    public event Action BlockDestroyed;

    public int ActiveBlocksCount { get; }

    public Transform Transform  { get; }
}