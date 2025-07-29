using UnityEngine;

public class PauseButton : ButtonMenu
{
    public override void PressButton()
    {
        base.PressButton();

        PauseManager.PauseGame();
    }
}