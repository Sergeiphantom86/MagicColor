using UnityEngine;

[RequireComponent(typeof(GridDragMovement), typeof(Magnifier), typeof(TouchColorTransparency))]
public class TouchDragInput : MonoBehaviour
{
    [SerializeField] private EffectsHandler _effectsHandler;

    private IEffectsHandler _ieffectsHandler;
    private bool _isSelected;
    private Touch _touch;
    private Camera _mainCamera;
    private Magnifier _selectable;
    private GridDragMovement _dragMovement;
    private IColorable _colorable;
    private Vector3 _lastMousePosition;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _colorable = GetComponent<IColorable>();
        _dragMovement = GetComponent<GridDragMovement>();
        _selectable = GetComponent<Magnifier>();
        _ieffectsHandler = _effectsHandler;

        if (_ieffectsHandler == null)
        {
            Debug.LogError("EffectsHandler not assigned in TouchDragInput", this);
        }
        if (_dragMovement == null)
        {
            Debug.LogError("DragMovement not assigned in TouchDragInput", this);
        }
        if (_selectable == null)
        {
            Debug.LogError("SelectableObject not assigned in TouchDragInput", this);
        }
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            HandleTochInput();
        }
        else
        {
            HandleMouseInput();
        }
    }

    private void HandleTochInput()
    {
        _touch = Input.GetTouch(0);

        switch (_touch.phase)
        {
            case TouchPhase.Began:
                SelectBlock(_touch.position);
                break;

            case TouchPhase.Moved:
                Move(_touch.position);
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                ThrowOff();
                break;
        }
    }

    private void HandleMouseInput()
    {
        Vector3 currentMousePosition = Input.mousePosition;

        switch (true)
        {
            case bool _ when Input.GetMouseButtonDown(0):
                SelectBlock(currentMousePosition);
                break;

            case bool _ when Input.GetMouseButton(0) && _isSelected:
                Move(currentMousePosition);
                break;

            case bool _ when Input.GetMouseButtonUp(0) && _isSelected:
                ThrowOff();
                break;
        }
    }

    private void SelectBlock(Vector2 position)
    {
        if (IsTouchingThisObject(position))
        {
            _isSelected = true;
            _selectable.Select();
            _dragMovement.BeginInteraction(position, transform.position);
            _colorable.AssignOriginal();
        }
    }

    private void Move(Vector2 position)
    {
        if (_isSelected)
        {
            _dragMovement.ProcessInput(position, transform.position);
        }
    }

    public void ThrowOff()
    {
        if (_isSelected)
        {
            _isSelected = false;
            _selectable.Deselect();
            _ieffectsHandler?.PlayEndDragging();
            _colorable.Disable();
        }
    }

    private bool IsTouchingThisObject(Vector2 screenPosition)
    {
        Ray ray = _mainCamera.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.collider.transform == transform ||
                   hit.collider.transform.IsChildOf(transform);
        }

        return false;
    }
}