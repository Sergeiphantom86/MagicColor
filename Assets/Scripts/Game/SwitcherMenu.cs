public class SwitcherMenu : ButtonMenu
{
    private const string Menu = nameof(Menu);

    public override void PressButton()
    {
        ProgressSaver.Saves.SetIndexExit();

        PauseMenu.Load(Menu);
    }
}