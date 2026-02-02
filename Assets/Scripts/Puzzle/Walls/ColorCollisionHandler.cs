using System;
using UnityEngine;

[RequireComponent(typeof(Wall), typeof(ColorMatchService), typeof(IBlockDestroySequence))]
[RequireComponent(typeof(LockFeedbackService), typeof(IColorMatchService), typeof(ILockFeedbackService))]
public class ColorCollisionHandler : MonoBehaviour
{
    private Wall _wall;
    private WallBlockProcessor _wallProcessor;
    private LockInteractionHandler _lockHandler;
    private BlockInteractionService _blockInteraction;
    private ICollisionProcessor _collisionProcessor;
    private IColorMatchService _colorMatch;
    private IBlockDestroySequence _destroySequence;
    private ILockFeedbackService _lockFeedback;
    private ICollisionHandler _collisionHandler;

    public event Action<Block> IsTouched;

    private void Awake()
    {
        _wall = GetComponent<Wall>();
        _wallProcessor = new WallBlockProcessor(_wall);

        _colorMatch = GetComponent<ColorMatchService>();
        _lockFeedback = GetComponent<LockFeedbackService>();
        _collisionHandler = GetComponent<CollisionHandler>();
        _destroySequence = GetComponent<BlockDestroySequence>();

        _blockInteraction = new BlockInteractionService(_wall, _destroySequence);
        _lockHandler = new LockInteractionHandler();
    }

    private void OnEnable()
    {
        _collisionHandler.OnEnter += Enter;
        _collisionHandler.OnExit += Exit;
    }

    private void OnDisable()
    {
        _collisionHandler.OnEnter -= Enter;
        _collisionHandler.OnExit -= Exit;
    }

    public bool Initialize(IColorPrecision colorPrecision, HintKey hintKey, ErrorPanel errorPanel)
    {
        if (Validate(colorPrecision, hintKey, errorPanel) == false)
            return false;

        _lockHandler.SetHint(hintKey);
        _blockInteraction.SetPanelError(errorPanel);
        _colorMatch.Initialize(colorPrecision);

        _collisionProcessor = new CollisionProcessor(
            _colorMatch,
            _blockInteraction
        );

        return true;
    }

    private bool Validate(IColorPrecision colorPrecision, HintKey hintKey, ErrorPanel errorPanel)
    {
        if (_colorMatch == null) return Log("ColorMatchService");
        if (_lockFeedback == null) return Log("LockFeedbackService");
        if (_collisionHandler == null) return Log("CollisionHandler");
        if (_destroySequence == null) return Log("IBlockDestroySequence");
        if (colorPrecision == null) return Log(nameof(colorPrecision));
        if (hintKey == null) return Log(nameof(hintKey));
        if (errorPanel == null) return Log(nameof(errorPanel));

        return true;
    }

    private bool Log(string name)
    {
        Debug.LogError($"{nameof(ColorCollisionHandler)} missing dependency: {name}", this);
        return false;
    }

    private void Enter(Collider other)
    {
        _collisionProcessor.ProcessEnter(other);
    }

    private void Exit(Collider other)
    {
        _collisionProcessor.ProcessExit(other);
    }

    public void UnblockWall()
    {
        _wallProcessor.UnblockWall();
        _lockHandler.Unblock();
    }

    public void TriggerContactEvent(Block block)
    {
        IsTouched?.Invoke(block);
    }
}