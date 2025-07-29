public class CloseGameButton : ButtonMenu
{
    public override void PressButton()
    {
        PauseManager.ResumeGame();
    }
}
