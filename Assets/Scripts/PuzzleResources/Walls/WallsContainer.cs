using System.Collections.Generic;
using Menu.Tutorials.TutorialPuzzle;
using PuzzleResources.LockMechanics;
using PuzzleResources.PenEditor;
using PuzzleResources.MinigamesRoulette;
using PuzzleResources.Walls.WallEngineResources;
using UnityEngine;
using Wallets.WalletEconomy;

namespace PuzzleResources.Walls
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

            if (ValidationHelper.AllNotNull(
                this,
                (colorPrecision, nameof(colorPrecision)),
                (bag, nameof(bag)),
                (rotator, nameof(rotator)),
                (hintKey, nameof(hintKey)),
                (@lock, nameof(@lock)),
                (errorPanel, nameof(errorPanel)),
                (activator, nameof(activator))) == false)
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
    }
}