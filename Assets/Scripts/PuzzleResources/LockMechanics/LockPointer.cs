using UnityEngine;

namespace PuzzleResources.LockMechanics
{
    public class LockPointer : MonoBehaviour
    {
        private ColorChanger _colorChanger;

        private void Awake()
        {
            _colorChanger = GetComponentInChildren<ColorChanger>();

            if (_colorChanger == null)
            {
                Debug.LogError("ColorChanger components not found in children");
                return;
            }
        }

        public void SetColor()
        {
            _colorChanger.SetGreenColor();
        }
    }
}