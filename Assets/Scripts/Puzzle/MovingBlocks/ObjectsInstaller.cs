using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Repainter), typeof(KeyInstaller), typeof(LockInstaller))]
public class ObjectsInstaller : MonoBehaviour
{
    [SerializeField] private Key _key;
    [SerializeField] private Lock _lock;

    private Repainter _repainter;
    private KeyInstaller _keyInstaller;
    private LockInstaller _lockInstaller;
    private IProgressSaver _progressSaver;
    private bool _isTutorial;

    private void Awake()
    {
        _repainter = GetComponent<Repainter>();
        _keyInstaller = GetComponent<KeyInstaller>();
        _lockInstaller = GetComponent<LockInstaller>();
        _progressSaver = new ProgressSaver();

        _isTutorial = _progressSaver.Saves.IsUnlockKey;

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
        _repainter.OnRecoloredWalls += PlaceLockOnRepaintedWalls;
        _repainter.OnRecoloredBlock += PlaceKeyOnUnrepaintedBlock;
    }

    private void OnDisable()
    {
        _repainter.OnRecoloredWalls -= PlaceLockOnRepaintedWalls;
        _repainter.OnRecoloredBlock -= PlaceKeyOnUnrepaintedBlock;
    }

    private void PlaceLockOnRepaintedWalls(List<IColorable> colorables)
    {
        if (_isTutorial == false) 
            return;

        _lockInstaller.TryPlaceLock(colorables);
    }

    private void PlaceKeyOnUnrepaintedBlock(List<IColorable> colorables)
    {
        if (_isTutorial == false) 
            return;

        _keyInstaller.TryPlaceKey(colorables);
    }
}