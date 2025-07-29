using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Flag : MonoBehaviour
{
    private Image _image;

    public Image Image => _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }
}