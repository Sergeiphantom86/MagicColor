using UnityEngine.SceneManagement;

namespace Game
{
    public class Restarter : ButtonMenu
    {
        public override void OnPressButton()
        {
            base.OnPressButton();
            PauseMenu.Load(SceneManager.GetActiveScene().name);
        }
    }
}