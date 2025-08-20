using UnityEngine;

public class Indicator : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TurnOffSpriteRenderer()
    {
        if (_spriteRenderer == null) return;
        _spriteRenderer.enabled = false;
    }
    public void TurnOnSpriteRenderer()
    {
        if (_spriteRenderer == null) return;
        _spriteRenderer.enabled = true;
    }
}