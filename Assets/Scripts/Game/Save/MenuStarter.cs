using UnityEngine;

public class MenuStarter : MonoBehaviour
{
    [SerializeField] private QuestCollector _questCollector;

    public void Initialize(IProgressSaver progressSaver, SpriteTransmitter spriteTransmitter)
    {
        _questCollector.Initialize(progressSaver, spriteTransmitter);
    }
}