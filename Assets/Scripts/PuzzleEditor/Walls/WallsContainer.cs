using System.Collections.Generic;
using Menu.TutorialEditor.TutorialPuzzle;
using PuzzleEditor.LockEditor;
using PuzzleEditor.PenEditor;
using PuzzleEditor.RouletteEditor;
using PuzzleEditor.Walls.WallEngineEditor;
using UnityEngine;
using Wallets.WalletEditor;

namespace PuzzleEditor.Walls
{
    public class WallsContainer : MonoBehaviour
    {
        [SerializeField] private Vector2Int _gridSize;
        [SerializeField] private Vector3 _position;

        private WallEngine _wall;
        private List<WallEngine> _walls;

        public Vector2Int GridSize => _gridSize;

        public Vector3 Position => _position;

        public void InitializeWalls(
            IColorPrecision colorPrecision,
            BagKey bag,
            Rotator rotator,
            Messager hintKey,
            Lock @lock,
            ErrorPanel errorPanel,
            Activator activator,
            AudioClip audioClip)
        {
            if (_walls == null)
                _walls = new List<WallEngine>();
            else
                _walls.Clear();

            if (ValidateDependencies(colorPrecision, bag, rotator, hintKey, @lock, errorPanel, activator) == false)
                return;

            foreach (Transform child in transform)
            {
                _wall = child.GetComponent<WallEngine>();

                if (_wall == null)
                    continue;

                if (_wall.Initialize(
                    colorPrecision,
                    bag,
                    rotator,
                    hintKey,
                    @lock,
                    errorPanel,
                    activator,
                    audioClip) == false)
                {
                    Debug.LogError("WallEngine failed to initialize", _wall);
                    continue;
                }

                _walls.Add(_wall);
            }
        }

        private bool ValidateDependencies(IColorPrecision colorPrecision,
            BagKey bag, Rotator rotator,
            Messager hintKey,
            Lock @lock,
            ErrorPanel errorPanel,
            Activator activator)
        {
            if (colorPrecision == null)
                return LogNull(nameof(colorPrecision));

            if (bag == null)
                return LogNull(nameof(bag));

            if (rotator == null)
                return LogNull(nameof(rotator));

            if (hintKey == null)
                return LogNull(nameof(hintKey));

            if (@lock == null)
                return LogNull(nameof(@lock));

            if (errorPanel == null)
                return LogNull(nameof(errorPanel));

            if (activator == null)
                return LogNull(nameof(activator));

            return true;
        }

        private bool LogNull(string dependencyName)
        {
            Debug.LogError($"{nameof(WallEngine)} initialization failed: {dependencyName} is NULL", this);

            return false;
        }
    }
}