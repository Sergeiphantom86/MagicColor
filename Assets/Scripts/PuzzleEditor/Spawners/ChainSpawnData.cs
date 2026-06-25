using System;
using UnityEngine;
namespace PuzzleEditor.Spawners
{

[Serializable]
public struct ChainSpawnData
{
    public Vector2Int StartOrigin;
    public Vector2Int Size;
    public ChainSpawnDirection Direction;
    public int Count;
    public int Spacing;
}
}