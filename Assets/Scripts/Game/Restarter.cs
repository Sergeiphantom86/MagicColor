using UnityEngine.SceneManagement;

public class Restarter : ButtonMenu
{
    public override void PressButton()
    {
        base.PressButton();
        PauseMenu.Load(SceneManager.GetActiveScene().name);
    }
}