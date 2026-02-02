using UnityEngine;

namespace YG.Example
{
    public class CanvasScaleForMobile : MonoBehaviour
    {
        public CustomCanvasScaler canvasScaler;
        public float scaleFactor = 1.4f;
        public Vector2 referenceResolution = new Vector2(800, 670);

        private void Start()
        {
            if (YG2.envir.isMobile || YG2.envir.isTablet)
            {
                //canvasScaler.scaleFactor = scaleFactor;
                //canvasScaler.referenceResolution = referenceResolution;
            }
        }
    }
}