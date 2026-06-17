using System;
using UnityEngine;
using DG.Tweening;

public static class SafeAnimationHelper
{
    public static void SafeDelayedCall(this MonoBehaviour behaviour, float delay, Action action)
    {
        if (behaviour == null) 
            return;

        DOVirtual.DelayedCall(delay, () =>
        {
            if (behaviour != null && behaviour.isActiveAndEnabled)
                action?.Invoke();
        });
    }
}