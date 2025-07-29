using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerAnimation), typeof(EffectTarget))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool _isMoving;
    [SerializeField] private float _heightAboveSurface;

    private bool _hasValidTarget;
    private Touch _touch;
    private Camera _camera;
    private PlayerMovement _movement;
    private PlayerAnimation _animation;
    private EffectTarget _effectTarget;
    private Vector3 _lastTargetPosition;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _animation = GetComponent<PlayerAnimation>();
        _effectTarget = GetComponent<EffectTarget>();

        _camera = Camera.main;
    }

    private void Update()
    {
        HandleInput();
        _animation.UpdateAnimation(_movement.Velocity, _movement.Speed);
    }

    public void TurnOffMovement()
    {
        _isMoving = false;
        _movement.Stop(true);
    }

    public void TurnOnMovement()
    {
        _isMoving = true;
        _movement.Stop(false);
    }

    private void HandleInput()
    {
        if (Input.touchCount > 0 && _isMoving)
        {
            _touch = Input.GetTouch(0);

            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            switch (_touch.phase)
            {
                case TouchPhase.Began:
                case TouchPhase.Moved:
                    SetPointMovement();
                    break;

                case TouchPhase.Ended:
                    TurnOnTargetEffect();
                    break;
            }
        }
    }

    private void SetPointMovement()
    {
        if (Physics.Raycast(_camera.ScreenPointToRay(_touch.position), out RaycastHit hitInfo))
        {
            if (_movement.TryGetNavMeshPosition(hitInfo.point) == false)
            {
                return;
            }
            
            _lastTargetPosition = hitInfo.point;
            MoveTargetPoint();
            _hasValidTarget = true;
        }
    }

    private void MoveTargetPoint()
    {
        _movement.SetDestination(_lastTargetPosition);
    }

    private void TurnOnTargetEffect()
    {
        if (_hasValidTarget)
        {
            _effectTarget.PlayDustEffect(_lastTargetPosition);
            _effectTarget.DisableWithDelay();
            _hasValidTarget = false;
        }
    }
}