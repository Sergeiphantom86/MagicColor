using UnityEngine;
using UnityEngine.UI;
using YG;

[RequireComponent(typeof(Image))]
public class PuzzleSelector : MonoBehaviour
{
    [SerializeField] private Button _button;

    private Image _puzzleImage;
    private string _puzzleName;

    private void Awake()
    {
        _puzzleImage = GetComponent<Image>();


        if (_button == null)
        {
            Debug.LogError("Button не назначен");
            return;
        }

        if (_puzzleImage == null)
        {
            Debug.LogError("Image не назначен");
            return;
        }

        _puzzleName = _puzzleImage.sprite.name;
    }

    public string GetName()
    {
        return _puzzleName;
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnPuzzleSelected);
    }

    private void OnPuzzleSelected()
    {
        YG2.saves.SetSprite(_puzzleImage.sprite);
        
        SceneLoader.Instance.LoadSceneWithSplash("Puzzle");
    }
}