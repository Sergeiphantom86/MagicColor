using System.Collections.Generic;
using UnityEngine;

public class LanguageBar : MonoBehaviour
{
    [SerializeField] private List<LanguageButton> _buttons;

    public List<LanguageButton> Buttons => _buttons;
}