using UnityEngine;

[RequireComponent(typeof(StarsController))]
public class QuantityMenuStarsIndicator : MonoBehaviour
{
    private StarsController _starsController;
    private IProgressSaver _progressSaver;

    private void Awake()
    {
        _starsController = GetComponent<StarsController>();
        _progressSaver = new ProgressSaver();
    }

    private void Start()
    {
        ShowQuantity();
    }

    private void ShowQuantity()
    {
        if (_progressSaver.Saves.CountStars != 0)
        {
            _starsController.ShowWithAnimation(_progressSaver.Saves.CountStars);
        }
    }
}