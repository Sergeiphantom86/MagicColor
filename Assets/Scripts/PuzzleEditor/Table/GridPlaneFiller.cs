using System;
using PuzzleEditor.MovingBlocks.GridEditor;
using UnityEngine;

namespace PuzzleEditor.Table
{
    public class GridPlaneFiller : MonoBehaviour
    {
        [SerializeField] private GameObject _planePrefab;
        [SerializeField][Min(0.1f)] private float _planeHeight;
        [SerializeField][Min(1)] private int _scaleDivider;
        [SerializeField][Min(1)] private int _positionDivider;
        [SerializeField][Range(1f, 1.2f)] private float _scaleMultiplierX;
        [SerializeField][Range(1f, 1.2f)] private float _scaleMultiplierZ;

        private GameObject _planeInstance;
        private GridSystem _grid;
        private Renderer _renderer;
        private int _scaleMultiplier;
        private int _multiplierPositions;
        private float _positionY;

        public event Action<Material, int, int, float> HasChanged;

        private void Awake()
        {
            _grid = GetComponent<GridSystem>();
        }

        private void OnEnable()
        {
            _grid.Initialized += OnStartSpawn;

            if (_grid.IsInitialized)
                OnStartSpawn();
        }

        private void OnDisable()
        {
            _grid.Initialized -= OnStartSpawn;
        }

        public void OnStartSpawn()
        {
            if (_planePrefab == null)
            {
                Debug.LogError($"{nameof(GridPlaneFiller)}: Plane prefab is not assigned!", this);
                return;
            }

            if (_grid == null)
            {
                Debug.LogError($"{nameof(GridPlaneFiller)}: GridSystem instance is null!", this);
                return;
            }

            if (ValidateGridSize() == false)
            {
                Debug.LogError("ValidateGridSize");
                return;
            }

            _positionY = _planeHeight;
            _scaleMultiplier = _scaleDivider;
            _multiplierPositions = _positionDivider;

            try
            {
                _planeInstance = Instantiate(_planePrefab, transform);

                if (TryGetRenderer() == false)
                {
                    CleanupInstance();
                    Debug.LogError("ValidateGridSize");
                    return;
                }

                SetScale();
                SetPosition();

                HasChanged?.Invoke(_renderer.material, _grid.GridSizeX, _grid.GridSizeY, _grid.CellSize);
            }
            catch (Exception exception)
            {
                Debug.LogError($"{nameof(GridPlaneFiller)}: Failed to spawn plane. Error: {exception.Message}", this);
                CleanupInstance();
            }
        }

        private bool ValidateGridSize()
        {
            bool isValid = true;

            if (_grid.GridSizeX <= 0)
            {
                Debug.LogError($"{nameof(GridPlaneFiller)}: GridSizeX must be greater than 0. Current value: {_grid.GridSizeX}", this);
                isValid = false;
            }

            if (_grid.GridSizeY <= 0)
            {
                Debug.LogError($"{nameof(GridPlaneFiller)}: GridSizeY must be greater than 0. Current value: {_grid.GridSizeY}", this);
                isValid = false;
            }

            if (_grid.CellSize <= 0)
            {
                Debug.LogError($"{nameof(GridPlaneFiller)}: CellSize must be greater than 0. Current value: {_grid.CellSize}", this);
                isValid = false;
            }

            return isValid;
        }

        private bool TryGetRenderer()
        {
            _renderer = _planeInstance.GetComponent<Renderer>();

            if (_renderer == null)
            {
                Debug.LogError($"{nameof(GridPlaneFiller)}: Plane prefab must have a Renderer component!", this);
                return false;
            }

            if (_renderer.material == null)
            {
                Debug.LogWarning($"{nameof(GridPlaneFiller)}: Renderer material is null. Using default material.", this);
            }

            return true;
        }

        private float GetScreenDimensions(float size, int divider)
        {
            if (divider <= 0)
            {
                Debug.LogWarning($"{nameof(GridPlaneFiller)}: Divider is 0 or negative. Using default value 1.");
                divider = 1;
            }

            return size * _grid.CellSize / divider;
        }

        private void SetScale()
        {
            try
            {
                float scaleX = GetScreenDimensions(_grid.GridSizeX, _scaleMultiplier) * _scaleMultiplierX;
                float scaleZ = GetScreenDimensions(_grid.GridSizeY, _scaleMultiplier) * _scaleMultiplierZ;

                _planeInstance.transform.localScale = new Vector3(scaleX, _positionY, scaleZ);
            }
            catch (Exception exception)
            {
                Debug.LogError($"{nameof(GridPlaneFiller)}: Failed to set scale. Error: {exception.Message}", this);
                _planeInstance.transform.localScale = Vector3.one;
            }
        }

        private void SetPosition()
        {
            try
            {
                float posX = GetScreenDimensions(_grid.GridSizeX, _multiplierPositions);
                float posZ = GetScreenDimensions(_grid.GridSizeY, _multiplierPositions);

                _planeInstance.transform.localPosition = new Vector3(posX, _positionY, posZ);
            }
            catch (Exception exception)
            {
                Debug.LogError($"{nameof(GridPlaneFiller)}: Failed to set position. Error: {exception.Message}", this);
                _planeInstance.transform.localPosition = Vector3.zero;
            }
        }

        private void CleanupInstance()
        {
            if (_planeInstance != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(_planeInstance);
                }
                else
                {
                    DestroyImmediate(_planeInstance);
                }

                _planeInstance = null;
            }
        }

        private void OnValidate()
        {
            _planeHeight = Mathf.Max(0.01f, _planeHeight);
            _scaleDivider = Mathf.Max(1, _scaleDivider);
            _positionDivider = Mathf.Max(1, _positionDivider);
            _scaleMultiplierX = Mathf.Clamp(_scaleMultiplierX, 0.5f, 2f);
            _scaleMultiplierZ = Mathf.Clamp(_scaleMultiplierZ, 0.5f, 2f);
        }

        private void OnDestroy()
        {
            CleanupInstance();
        }
    }
}