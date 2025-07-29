using UnityEngine;
using UnityEngine.UI;
using YG;

[RequireComponent(typeof(Image))]
public class PuzzleSelector : MonoBehaviour
{
    [SerializeField] private Button _button;

    private Image _puzzleImage;

    private void Awake()
    {
        _puzzleImage = GetComponent<Image>();


        if (_button == null)
        {
            Debug.LogError("Button не назначен");
            return;
        }
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