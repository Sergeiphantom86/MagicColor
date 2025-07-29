using UnityEngine;
using UnityEngine.UI;

public class Star : MonoBehaviour
{
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

   public void SetActive(bool isOn)
    {
        _image.enabled = isOn;
    }
}