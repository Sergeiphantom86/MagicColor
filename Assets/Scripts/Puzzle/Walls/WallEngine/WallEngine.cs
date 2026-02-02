using UnityEngine;

[RequireComponent(typeof(Wall), typeof(WallMovement))]
public class WallEngine : MonoBehaviour, IWallInteractor
{
    private Wall _wall;
    private WallMovement _movement;
    private Rotator _rotation;
    private WallLayoutUpdater _layoutUpdater;
    private BlockDestroySequence _blockDestroySequence;
    private AudioClip _audioClip;
    private Voiceover _voiceover;

    private void Awake()
    {
        _wall = GetComponent<Wall>();
        _movement = GetComponent<WallMovement>();
        _layoutUpdater = GetComponent<WallLayoutUpdater>();
        _voiceover = GetComponent<Voiceover>();
    }

    private void OnDisable()
    {
        _blockDestroySequence.IsTouched -= jcdc;


        if (_rotation != null)
            _rotation.OnRotated -= _movement.CacheStartPosition;
    }

    public bool Initialize(IColorPrecision colorPrecision, Bag bag, Rotator rotator, HintKey hintKey, Lock @lock, EffectsHandler effectsHandler, ErrorPanel errorPanel, Activator activator, AudioClip audioClip)
    {
        if (ValidateDependencies(colorPrecision, bag, rotator, hintKey, @lock) == false)
            return false;

        _rotation = rotator;
        _blockDestroySequence = GetComponent<BlockDestroySequence>();
        LockFeedbackService lockFeedback = GetComponent<LockFeedbackService>();
        ColorCollisionHandler collisionHandler = GetComponent<ColorCollisionHandler>();
        WallInteractionController interactionController = GetComponent<WallInteractionController>();

        if (collisionHandler == null)
        {
            Debug.LogError("WallEngine initialization failed: ColorCollisionHandler missing", this);
            return false;
        }

        if (interactionController == null)
        {
            Debug.LogError("WallEngine initialization failed: WallInteractionController missing", this);
            return false;
        }

        if (_layoutUpdater == null)
        {
            Debug.LogError("WallEngine initialization failed: WallLayoutUpdater missing", this);
            return false;
        }

        if (lockFeedback == null)
        {
            Debug.LogError("WallEngine initialization failed: LockFeedbackService missing", this);
            return false;
        }

        if (_blockDestroySequence == null)
        {
            Debug.LogError("WallEngine initialization failed: BlockDestroySequence missing", this);
            return false;
        }

        if (_movement != null)
        {
            _movement.CacheStartPosition();
            _rotation.OnRotated += _movement.CacheStartPosition;
        }

        _layoutUpdater.Initialize(_rotation);
        lockFeedback.InitializComponents(@lock, hintKey);
        interactionController.Initialize(new BagUnlockPolicy(bag, 1), this);
        collisionHandler.Initialize(colorPrecision, hintKey, errorPanel);
        _blockDestroySequence.Initialize(effectsHandler, activator);

        _blockDestroySequence.IsTouched += jcdc;

        _audioClip = audioClip;

        return true; 
    }

    private void jcdc(Block block)
    {
        PushMovement();
    }


    public void PushMovement()
    {
        _movement.Push();
 
        if (_voiceover != null && _audioClip != null)
            _voiceover.Play(_audioClip);
    }

    public void Unlock()
    {
        _wall.Unblock();
    }

    private bool ValidateDependencies(IColorPrecision colorPrecision, Bag bag, Rotator rotator, HintKey hintKey, Lock @lock)
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

        return true;
    }

    private bool LogNull(string dependencyName)
    {
        Debug.LogError($"{nameof(WallEngine)} initialization failed: {dependencyName} is NULL", this);

        return false;
    }
}