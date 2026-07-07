using UnityEngine;
using YG;

namespace PuzzleResources.Stars
{
    [RequireComponent(typeof(StarsController))]

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
            if (YG2.saves.Stars != 0)
            {
                _starsController.ShowWithAnimation(YG2.saves.Stars);
            }
        }
    }
}