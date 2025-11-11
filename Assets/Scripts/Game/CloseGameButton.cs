public class CloseGameButton : ButtonMenu
{
    public override void PressButton()
    {
        PauseMenu.ResumeGame();
    }
}
