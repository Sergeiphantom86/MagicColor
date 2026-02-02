using UnityEngine;

[RequireComponent(typeof(InputHandler), typeof(ColorCollisionHandler))]
public class WallInteractionController : MonoBehaviour
{
    private IUnlockPolicy _unlockPolicy;
    private IWallInteractor _wall;

    private InputHandler _inputHandler;
    private ColorCollisionHandler _colorCollisionHandler;

    private bool _initialized;

    private void Awake()
    {
        _inputHandler = GetComponent<InputHandler>();
        _colorCollisionHandler = GetComponent<ColorCollisionHandler>();
    }

    public void Initialize(IUnlockPolicy unlockPolicy, IWallInteractor wall)
    {
        if (_initialized)
            return;

        _unlockPolicy = unlockPolicy ??
            throw new System.ArgumentNullException(nameof(unlockPolicy));

        _wall = wall ??
            throw new System.ArgumentNullException(nameof(wall));

        Subscribe();

        _initialized = true;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        _inputHandler.OnSelected += OnSelected;
        _colorCollisionHandler.IsTouched += OnBlockTouch;
    }

    private void Unsubscribe()
    {
        if (_initialized == false)
            return;

        _inputHandler.OnSelected -= OnSelected;
        _colorCollisionHandler.IsTouched -= OnBlockTouch;
    }

    private void OnSelected(Vector2 screenPosition)
    {
        if (_unlockPolicy.TryUnlock())
        {
            _wall.Unlock();
            return;
        }

        //_wall.PushMovement();
    }

    private void OnBlockTouch(Block block)
    {
        _wall.PushMovement();
    }
}