using Menu.ButtonEditor;
using System.Collections.Generic;
using UnityEngine;
namespace Menu.LanguageManager
{

public class LanguageBar : MonoBehaviour
{
    [SerializeField] private List<LanguageButton> _buttons;

    public List<LanguageButton> Buttons => _buttons;
}
}