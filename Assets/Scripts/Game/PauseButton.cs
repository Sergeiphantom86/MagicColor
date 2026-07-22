namespace Game
{
    public class PauseButton : ButtonMenu
    {
        public override void OnPressButton()
        {
            base.OnPressButton();

            PauseMenu.Stop();
        }
    }
}