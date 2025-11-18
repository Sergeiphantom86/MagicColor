using System;
using System.Collections.Generic;
using UnityEngine;

public interface IBlocksContainer
{
    public event Action BlockDestroyed;

    public int ActiveBlocksCount { get; }

    public Transform Transform  { get; }

    public List<Block> Blocks {  get; }
}