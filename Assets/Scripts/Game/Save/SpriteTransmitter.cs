using UnityEngine;

public class SpriteTransmitter : MonoBehaviour
{
    private Sprite _new;
    private Sprite _current;
    private bool _isAutomaticallyNewLevel;

    public Sprite New => _new;

    public Sprite Current => _current;

    public bool IsAutomaticallyNewLevel => _isAutomaticallyNewLevel;

    public void SetNew(Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.LogWarning($"[SpriteTransmitter] Попытка установить null в _new на объекте {gameObject.name}");
            return;
        }

        if (_new == sprite)
        {
            return;
        }

        _new = sprite;
    }

    public void SetCurrent(Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.LogWarning($"[SpriteTransmitter] Попытка установить null в _current на объекте {gameObject.name}");
            return;
        }

        if (_current == sprite)
        {
            return;
        }

        _current = sprite;
    }

    public void SetAutomaticTransition(bool isAutomaticallyNewLevel)
    {
        _isAutomaticallyNewLevel = isAutomaticallyNewLevel;
    }
}