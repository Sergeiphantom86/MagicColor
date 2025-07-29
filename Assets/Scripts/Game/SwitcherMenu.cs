public class SwitcherMenu : ButtonMenu
{
    public override void PressButton()
    {
        PauseManager.ExitToMainMenu();
    }
}