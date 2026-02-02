using System;

public class FillSpeedController
{
    private bool _isAccelerated;

    public void TryAccelerate(float remainingTime, Action<float> notify, Action applySpeed)
    {
        if (_isAccelerated) return;

        _isAccelerated = true;
        notify?.Invoke(remainingTime);
        applySpeed?.Invoke();
    }
}