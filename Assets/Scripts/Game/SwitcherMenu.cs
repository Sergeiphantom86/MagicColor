namespace Game
{
    public class SwitcherMenu : ButtonMenu
    {
        private const string Menu = nameof(Menu);

        public override void OnPressButton()
        {
            PauseMenu.Load(Menu);
        }
    }
}