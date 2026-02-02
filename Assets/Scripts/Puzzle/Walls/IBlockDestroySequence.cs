using UnityEngine;

public interface IBlockDestroySequence
{
    public void WaitStart(Block block, Color color, Wall wall);
}