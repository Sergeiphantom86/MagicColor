using System;

public class PauseButton : ButtonMenu
{
    public event Action OnClick;

    public override void PressButton()
    {
        base.PressButton();

        PauseMenu.Stop();
        OnClick?.Invoke();
    }
}