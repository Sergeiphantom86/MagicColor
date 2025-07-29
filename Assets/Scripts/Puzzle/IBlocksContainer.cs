using System;

public interface IBlocksContainer
{
    event Action<string> StoppingTimer;
    event Action BlockDestroyed;
    int ActiveBlocksCount { get; }
}