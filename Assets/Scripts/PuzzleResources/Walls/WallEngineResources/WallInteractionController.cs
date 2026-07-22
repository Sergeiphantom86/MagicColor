using System;
using PuzzleResources.MovingBlocks;
using UnityEngine;

namespace PuzzleResources.Walls.WallEngineResources
{
    [RequireComponent(typeof(IInputHandler), typeof(ColorCollisionHandler))]

    public class WallInteractionController : MonoBehaviour
    {
        private IUnlockPolicy _unlockPolicy;
        private IWallInteractor _wall;

        private IInputHandler _inputHandler;
        private ColorCollisionHandler _colorCollisionHandler;

        private bool _initialized;

        private void Awake()
        {
            _inputHandler = GetComponent<IInputHandler>();
            _colorCollisionHandler = GetComponent<ColorCollisionHandler>();
        }

        private void OnDisable()
        {
            if (_initialized == false)
                return;

            _inputHandler.Selected -= OnSelected;
        }

        public void Initialize(IUnlockPolicy unlockPolicy, IWallInteractor wall)
        {
            if (_initialized)
            return;

            _unlockPolicy = unlockPolicy ?? throw new ArgumentNullException(nameof(unlockPolicy));

            _wall = wall ?? throw new ArgumentNullException(nameof(wall));

            _inputHandler.Selected += OnSelected;

            _initialized = true;
        }

        private void OnSelected(Vector2 screenPosition)
        {
            if (_unlockPolicy.TryUnlock())
            {
                _colorCollisionHandler.UnblockWall();
            }

            _wall.PushMovement();
        }
    }
}