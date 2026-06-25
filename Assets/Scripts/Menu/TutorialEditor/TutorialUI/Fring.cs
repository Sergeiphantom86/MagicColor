using UnityEngine;
using UnityEngine.UI;

public class Fring : MonoBehaviour
{
    [SerializeField] private Button _button;

    public Button Button => _button;

    public void SetActive(bool isOn)
    {
        gameObject.SetActive(isOn);
    }
}