using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PuzzleSelector : MonoBehaviour
{
    private Image _puzzleImage;

    public Sprite Sprite => _puzzleImage.sprite;
    public string Name => _puzzleImage.sprite.name;

    private void Awake()
    {
        _puzzleImage = GetComponent<Image>();

        if (_puzzleImage == null)
        {
            Debug.LogError("Image не назначен");
            return;
        }
    }
}