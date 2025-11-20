using DG.Tweening;
using System;
using UnityEngine;

public static class SafeAnimationHelper
{
    public static void SafeDelayedCall(this MonoBehaviour behaviour, float delay, Action action)
    {
        if (behaviour == null) return;

        DOVirtual.DelayedCall(delay, () =>
        {
            if (behaviour != null && behaviour.isActiveAndEnabled)
                action?.Invoke();
        });
    }
}