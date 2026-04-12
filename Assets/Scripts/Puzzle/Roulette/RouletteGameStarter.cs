using UnityEngine;

public class RouletteGameStarter : MonoBehaviour
{
    [SerializeField] AutomaticTransitionInstaller _automaticTransitionInstaller;

    public void SetProgressSaver(IProgressSaver progressSaver, Sprite newSprite)
    {
        _automaticTransitionInstaller.SetProgressSaver(progressSaver, newSprite);
    }
}