using UnityEngine;
using YG;

public class QuantityMenuStarsIndicator : MonoBehaviour
{
    private StarsController _starsController;

    private void Awake()
    {
        _starsController = GetComponent<StarsController>();
    }

    private void Start()
    {
        ShowQuantity();
    }

    private void ShowQuantity()
    {
        if (YG2.saves.CountStars != 0)
        {
            _starsController.ShowStars(YG2.saves.CountStars);
        }
    }
}