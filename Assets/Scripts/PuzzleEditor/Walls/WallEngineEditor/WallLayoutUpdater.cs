using System;
using PuzzleEditor.Walls.WallEditor;
using UnityEngine;

namespace PuzzleEditor.Walls.WallEngineEditor
{
    [RequireComponent(typeof(Wall))]
    public class WallLayoutUpdater : MonoBehaviour
    {
        private Wall _wall;
        private Point _point;
        private Indicator _indicator;
        private Rotator _rotator;

        private Vector2Int _lastResolution;

        public void Initialize(Rotator rotator)
        {
            if (ValidateDependencies(rotator) == false)
                return;

            _rotator = rotator;
            _rotator.Rotated += OnRotated;
        }

        private void Awake()
        {
            _wall = GetComponent<Wall>();
            _point = GetComponentInChildren<Point>();
            _indicator = GetComponentInChildren<Indicator>();

            _lastResolution = new Vector2Int(Screen.width, Screen.height);
        }

        private void Update()
        {
            if (_lastResolution.x != Screen.width || _lastResolution.y != Screen.height)
            {
                _lastResolution = new Vector2Int(Screen.width, Screen.height);
                Recalculate();
            }
        }

        private void OnDestroy()
        {
            if (_rotator != null)
                _rotator.Rotated -= OnRotated;
        }

        private void OnRotated()
        {
            Recalculate();
        }

        public void Recalculate()
        {
            _wall.SetPosition(_indicator.transform.position, _point.transform.position);
        }

        private bool ValidateDependencies(Rotator rotator)
        {
            if (rotator == null)
                return LogNull(nameof(rotator));

            return true;
        }

        private bool LogNull(string dependencyName)
        {
            Debug.LogError(
                $"{nameof(WallEngine)} initialization failed: {dependencyName} is NULL",
                this
            );

            return false;
        }
    }
}