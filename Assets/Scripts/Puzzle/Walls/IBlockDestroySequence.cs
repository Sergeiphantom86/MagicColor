using System;
using UnityEngine;

public interface IBlockDestroySequence
{
    public void WaitStart(IColorable colorable, Color color);

    public event Action IsTouched;
}