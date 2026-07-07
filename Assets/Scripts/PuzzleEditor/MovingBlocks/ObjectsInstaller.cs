using System.Collections.Generic;
using Menu.Tutorials;
using PuzzleEditor.LockMechanics;
using UnityEngine;
using YG;

namespace PuzzleEditor.MovingBlocks
{
    [RequireComponent(typeof(Repainter), typeof(KeyInstaller), typeof(LockInstaller))]

    public class ObjectsInstaller : MonoBehaviour
    {
        [SerializeField] private Key _key;
        [SerializeField] private Lock _lock;

        private Repainter _repainter;
        private KeyInstaller _keyInstaller;
        private LockInstaller _lockInstaller;
        private bool _isTutorial;

        private void Awake()
        {
            _repainter = GetComponent<Repainter>();
            _keyInstaller = GetComponent<KeyInstaller>();
            _lockInstaller = GetComponent<LockInstaller>();

            _isTutorial = YG2.saves.IsUnlockKey;

            if (_repainter == null)
            {
                Debug.LogError("Repainter == null");
                return;
            }

            _keyInstaller.Initialized(_key, _isTutorial);
            _lockInstaller.Initialized(_lock, _isTutorial);
        }

        private void OnEnable()
        {
            _repainter.RecoloredWalls += OnPlaceLockOnRepaintedWalls;
            _repainter.RecoloredBlock += OnPlaceKeyOnUnrepaintedBlock;
        }

        private void OnDisable()
        {
            _repainter.RecoloredWalls -= OnPlaceLockOnRepaintedWalls;
            _repainter.RecoloredBlock -= OnPlaceKeyOnUnrepaintedBlock;
        }

        private void OnPlaceLockOnRepaintedWalls(List<IColorable> colorables)
        {
            if (_isTutorial == false)
            return;

            _lockInstaller.RightPlace(colorables);
        }

        private void OnPlaceKeyOnUnrepaintedBlock(List<IColorable> colorables)
        {
            if (_isTutorial == false)
            return;

            _keyInstaller.RightPlace(colorables);
        }
    }
}