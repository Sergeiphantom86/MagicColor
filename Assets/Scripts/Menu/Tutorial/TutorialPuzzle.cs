using UnityEngine;
using UnityEngine.UI;

public class TutorialPuzzle : MonoBehaviour
{
    private Image _image;

    public Sprite Sprite => _image.sprite;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }
}