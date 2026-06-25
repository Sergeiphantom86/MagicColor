using UnityEngine.SceneManagement;
namespace Game
{

public class Restarter : ButtonMenu
{
    public override void PressButton()
    {
        base.PressButton();
        PauseMenu.Load(SceneManager.GetActiveScene().name);
    }
}
}