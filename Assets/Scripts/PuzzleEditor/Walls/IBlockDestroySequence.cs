using System;
using UnityEngine;

public interface IBlockDestroySequence
{
    public event Action IsTouched;

    public void WaitStart(IColorable colorable, Color color);
}