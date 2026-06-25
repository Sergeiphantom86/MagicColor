using System;

namespace Game
{
    public class PauseButton : ButtonMenu
    {
        public override void PressButton()
        {
            base.PressButton();

            PauseMenu.Stop();
        }
    }
}