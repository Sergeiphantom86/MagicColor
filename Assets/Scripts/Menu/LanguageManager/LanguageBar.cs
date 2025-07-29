using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageBar : MonoBehaviour
{
    [SerializeField] private List<Button> _buttons;

    public List<Button> Buttons => _buttons;

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }

    public void TurnOn()
    {
        gameObject.SetActive(true);
    }
}