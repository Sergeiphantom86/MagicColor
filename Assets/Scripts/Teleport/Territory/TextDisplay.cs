using System.Collections;
using UnityEngine;

public class TextDisplay : MonoBehaviour
{
    [SerializeField] private InteractionHandler _interaction;

    private float _displayDuration;
    private Coroutine _displayRoutine;
    private TextDisplayController _textController;
    private string _nameTerritory;

    public string NameTerritory => _nameTerritory;

    private void Awake()
    {
        _displayDuration = 3f;
        _textController = GetComponent<TextDisplayController>();
    }

    private void OnEnable()
    {
        if (_textController == null) return;

        _interaction.OnContactTerrain += Show;
    }

    private void OnDisable()
    {
        if (_textController == null) return;

        _interaction.OnContactTerrain -= Show;
    }

    private void Show(string name)
    {
        if (string.IsNullOrEmpty(name)) return;

        if (_displayRoutine != null)
            StopCoroutine(_displayRoutine);
        _nameTerritory = name;
        _displayRoutine = StartCoroutine(DisplayRoutine(name));
    }

    private IEnumerator DisplayRoutine(string name)
    {
        _textController.SetText(name);
        _textController.ShowText();

        yield return new WaitForSeconds(_displayDuration);

        _textController.HideText();
    }
}