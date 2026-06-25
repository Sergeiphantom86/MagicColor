namespace Game
{
public class CloseGameButton : ButtonMenu
{
    public override void PressButton()
    {
        PauseMenu.Resume();
    }
}

}