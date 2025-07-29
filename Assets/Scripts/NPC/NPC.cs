using System;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private Sprite _firstSprite;
    [SerializeField] private Sprite _secondSprite;

    private string _name = "Right";
    private Sprite _selectedSprite;

    public Sprite Sprite => _selectedSprite
    ? _selectedSprite
    : throw new InvalidOperationException("Sprite отсутствует");

    private void Awake()
    {
        _name = "Right";
    }


    public void GetSelectedSprite(string desiredSprite)
    {
        if (_name == desiredSprite)
        {
            _selectedSprite = _secondSprite;
            return;
        }

        _selectedSprite = _firstSprite;
    }
}