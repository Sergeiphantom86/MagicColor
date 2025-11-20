using DG.Tweening;

public static class DOTweenExtensions
{
    public static void SafePlay(this Tween tween)
    {
        if (tween != null && tween.IsActive() && tween.IsPlaying() == false)
        {
            tween.Play();
        }
    }

    public static void SafePause(this Tween tween)
    {
        if (tween != null && tween.IsActive() && tween.IsPlaying())
        {
            tween.Pause();
        }
    }

    public static void SafeKill(this Tween tween, bool complete = false)
    {
        if (tween != null && tween.IsActive())
        {
            tween.Kill(complete);
        }
    }
}