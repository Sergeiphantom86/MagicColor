using UnityEngine;
using UnityEngine.UI;

namespace PuzzleResources
{
    public class UIMaterialOrder : MonoBehaviour
    {
        [SerializeField] private int _uiBackgroundQueue;
        [SerializeField] private bool _isImmediate;

        private Graphic _uiGraphic;
        private Material _originalMaterial;
        private Material _clonedMaterial;
        private Renderer _renderer;

        private void Start()
        {
            _uiGraphic = GetComponent<Graphic>();
            _renderer = GetComponent<Renderer>();

            if (_uiGraphic != null && _isImmediate)
            {
                _originalMaterial = _uiGraphic.material;
                _clonedMaterial = new Material(_originalMaterial);
                _uiGraphic.material = _clonedMaterial;

                SetUIBackground();
            }
        }

        private void OnDestroy()
        {
            if (_uiGraphic != null && _originalMaterial != null)
                _uiGraphic.material = _originalMaterial;
        }

        public void SetUIBackground()
        {
            if (_clonedMaterial != null)
            _clonedMaterial.renderQueue = _uiBackgroundQueue;
        }

        public void SetOrder()
        {
            if (_renderer != null)
            {
                _renderer.material.renderQueue = _uiBackgroundQueue;
            }
        }
    }
}