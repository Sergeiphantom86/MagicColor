using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace PuzzleEditor.RouletteEditor
{

public class ErrorPanel : MonoBehaviour
{
    private int _blinkCount;
    private float _blinkDuration;
    private Image _renderer;
    private WaitForSeconds _sleep;

    private void Awake()
    {
        _blinkCount = 4;
        _blinkDuration = 0.1f;
        _renderer = GetComponent<Image>();
        _sleep = new WaitForSeconds(_blinkDuration);

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        StartCoroutine(BlinkAndDisable());
    }

    private IEnumerator BlinkAndDisable()
    {
        for (int i = 0; i < _blinkCount; i++)
        {
            _renderer.enabled = true;
            yield return _sleep;

            _renderer.enabled = false;
            yield return _sleep;
        }

        gameObject.SetActive(false);
    }

    public void TurnOn()
    {
        gameObject.SetActive(true);
    }
}
}