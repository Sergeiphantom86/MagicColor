using UnityEngine;

public class RouletteGameStarter : MonoBehaviour
{
    [SerializeField] AutomaticTransitionInstaller _automaticTransitionInstaller;

    public void SetProgressSaver(IProgressSaver progressSaver, SpriteTransmitter spriteTransmitter)
    {
        _automaticTransitionInstaller.SetProgressSaver(progressSaver, spriteTransmitter);
    }
}