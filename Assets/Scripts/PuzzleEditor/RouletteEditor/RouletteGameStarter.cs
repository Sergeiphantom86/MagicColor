using Game.SaveEditor;
using UnityEngine;
namespace PuzzleEditor.RouletteEditor
{

public class RouletteGameStarter : MonoBehaviour
{
    [SerializeField] private AutomaticTransitionInstaller _automaticTransitionInstaller;

    public void SetProgressSaver(IProgressSaver progressSaver, SpriteTransmitter spriteTransmitter)
    {
        _automaticTransitionInstaller.SetProgressSaver(progressSaver, spriteTransmitter);
    }
}
}