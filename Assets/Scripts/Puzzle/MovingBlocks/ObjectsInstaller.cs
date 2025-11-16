using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YG;

[RequireComponent(typeof(Repainter))]
public class ObjectsInstaller : MonoBehaviour
{
    [SerializeField] private Key _key;
    [SerializeField] private Lock _lock;
    [SerializeField] private Rotator _rotation;
    [SerializeField] private Canvas _transformParent;

    private bool _isPlacedKey;
    private bool _isPlacedLock;
    private int _desiredBlockCount;
    private Repainter _repainter;
    private Vector3 _angleRotationHorizontal;

    private void Awake()
    {
        _desiredBlockCount = 3;
        _repainter = GetComponent<Repainter>();
        _angleRotationHorizontal = new Vector3(60, 0, 0);

        if (_repainter == null)
        {
            Debug.LogError("Repainter == null");
            return;
        }

        if (YG2.saves.IsTutorial == false)
        {
            _key.gameObject.SetActive(false);
            _lock.gameObject.SetActive(false);
            Debug.Log("ofkvof");
            _key = null;
            _lock = null;
        }
    }

    private void OnEnable()
    {
        _repainter.OnRecoloredWalls += PlaceLockOnRepaintedWalls;
        _repainter.OnRecoloredBlock += PlaceKeyOnUnrepaintedBlock;
        _rotation.OnRotated += SetParent;
    }

    private void OnDisable()
    {
        _repainter.OnRecoloredWalls -= PlaceLockOnRepaintedWalls;
        _repainter.OnRecoloredBlock -= PlaceKeyOnUnrepaintedBlock;
        _rotation.OnRotated -= SetParent;
    }

    private void SetParent()
    {
        if (_lock == null) return;
        if (_key == null) return;

        _lock.transform.SetParent(_transformParent.transform);
        _key.transform.SetParent(_transformParent.transform);
    }

    private void PlaceLockOnRepaintedWalls(List<IColorable> colorables)
    {
        if (CanPlaceLock() == false) return;

        var eligibleWall = colorables
            .FirstOrDefault(colorable => IsEligibleWallForLock(colorable)) as Wall;

        if (eligibleWall != null)
        {
            PlaceLockOnWall(eligibleWall);
        }
    }

    private void PlaceKeyOnUnrepaintedBlock(List<IColorable> colorables)
    {
        if (CanPlaceKey() == false) return;

        var eligibleBlocks = FindEligibleBlocksForKey(colorables);

        if (eligibleBlocks.Count > 0)
        {
            PlaceKeyOnRandomBlock(eligibleBlocks);
        }
    }

    private bool CanPlaceKey()
    {
        return _key != null && _isPlacedKey == false;
    }

    private bool CanPlaceLock()
    {
        return _lock != null && _isPlacedLock == false;
    }

    private bool IsEligibleWallForLock(IColorable colorable)
    {
        return colorable is Wall wall &&
               colorable.IsRepainted &&
               wall.CenterFence != null;
    }

    private void PlaceLockOnWall(Wall wall)
    {
        if (_lock == null) return;

        _lock.transform.position = wall.CenterFence.position;
        wall.Block();

        AdjustLockRotation(wall);
        _isPlacedLock = true;

    }

    private void AdjustLockRotation(Wall wall)
    {
        if (wall.GetAngleY() == 0)
        {
            _lock.SetAngle(_angleRotationHorizontal);
        }
    }

    private List<Block> FindEligibleBlocksForKey(List<IColorable> colorables)
    {
        return PickRandomSubset(GetUnpaintedBlocks(colorables), _desiredBlockCount);
    }

    private List<Block> GetUnpaintedBlocks(List<IColorable> colorables)
    {
        return colorables.OfType<Block>()
                         .Where(block => block.IsRepainted == false)
                         .ToList();
    }

    private List<Block> PickRandomSubset(List<Block> blocks, int maxCount)
    {
        return blocks.Count <= maxCount ? blocks : GetRandomSelection(blocks, maxCount);
    }

    private List<Block> GetRandomSelection(List<Block> blocks, int count) =>
        blocks.OrderBy(_ => Random.value).Take(count).ToList();

    private void PlaceKeyOnRandomBlock(List<Block> eligibleBlocks)
    {
        _key.transform.position = GetSelectedBlock(eligibleBlocks).transform.position;

        _isPlacedKey = true;
    }

    private Block GetSelectedBlock(List<Block> eligibleBlocks)
    {
        return eligibleBlocks[GetRandomIndex(eligibleBlocks.Count)];
    }

    private int GetRandomIndex(int quantity)
    {
        return Random.Range(0, quantity);
    }
}