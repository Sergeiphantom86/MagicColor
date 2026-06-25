namespace Game
{
    public class SwitcherMenu : ButtonMenu
    {
        private const string Menu = nameof(Menu);

        public override void PressButton()
        {
            PauseMenu.Load(Menu);
        }
    }
}