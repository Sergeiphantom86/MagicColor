using System.Collections;
using Menu;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(Canvas))]

    public class CustomCanvasScaler : MonoBehaviour
    {
        [Header("Mobile")]
        [SerializeField] private float _mobileScaleMultiplier;

        private Canvas _canvas;
        private Camera _camera;
        private ZoomChanger _zoomChanger;
        private Coroutine _recalculateRoutine;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _zoomChanger = new ZoomChanger();

            _camera = _camera != null ? _camera : Camera.main;
        }

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            StartRecalculate();
        }

        private void Update()
        {
            _zoomChanger.ChangeLocation(StartRecalculate);
        }

        private void StartRecalculate()
        {
            if (_recalculateRoutine != null)
                StopCoroutine(_recalculateRoutine);

            _recalculateRoutine = StartCoroutine(RecalculateDelayed());
        }

        private IEnumerator RecalculateDelayed()
        {
            yield return null;
            yield return null;

            if (_canvas == null)
            {
                Debug.LogError("Camera.main == null");
                yield return null;
            }

            float scale = _zoomChanger.GetScreenSize(_camera);

            if (scale <= 0f)
                yield break;

            if (_zoomChanger.IsMobileWithTallScreen() == false)
                yield break;

            _canvas.scaleFactor = scale * _mobileScaleMultiplier;
        }
    }
}