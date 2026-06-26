namespace Game
{
    public class CloseGameButton : ButtonMenu
    {
        public override void OnPressButton()
        {
            PauseMenu.Resume();
        }
    }
}