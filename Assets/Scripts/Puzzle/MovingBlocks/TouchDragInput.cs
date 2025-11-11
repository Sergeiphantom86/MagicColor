using UnityEngine;

[RequireComponent(typeof(GridDragMovement), typeof(Magnifier), typeof(InputHandler))]
[RequireComponent(typeof(IColorable), typeof(Voiceover))]
public class TouchDragInput : MonoBehaviour
{
    private bool _isSelected;
    private Magnifier _selectable;
    private IColorable _colorable;
    private GridDragMovement _dragMovement;
    private InputHandler _inputHandler;
    private Voiceover _voiceover;
    private AudioClip _taking;
    private AudioClip _throwOff;
    private AudioClip _dragging;

    private void Awake()
    {
        _colorable = GetComponent<IColorable>();
        _selectable = GetComponent<Magnifier>();
        _voiceover = GetComponent<Voiceover>();
        _inputHandler = GetComponent<InputHandler>();
        _dragMovement = GetComponent<GridDragMovement>();

        if (_dragMovement == null)
        {
            Debug.LogError("DragMovement not assigned in TouchDragInput", this);
        }

        if (_selectable == null)
        {
            Debug.LogError("SelectableObject not assigned in TouchDragInput", this);
        }

        if (_colorable == null)
        {
            Debug.LogError("IColorable not assigned in TouchDragInput", this);
        }

        if (_voiceover == null)
        {
            Debug.LogError("Voiceover not assigned in TouchDragInput", this);
        }
    }

    private void OnEnable()
    {
        _inputHandler.OnSelected += SelectBlock;
        _inputHandler.OnMoved += Move;
        _inputHandler.OnThrowed += ThrowOff;
    }

    private void OnDisable()
    {
        _inputHandler.OnSelected -= SelectBlock;
        _inputHandler.OnMoved -= Move;
        _inputHandler.OnThrowed -= ThrowOff;
    }

    public void SetAudioClip(AudioClip dragging, AudioClip taking, AudioClip throwOff)
    {
        _dragging = dragging;
        _taking = taking;
        _throwOff = throwOff;
    }

    private void SelectBlock(Vector2 position)
    {
        _isSelected = true;
        _selectable.Select();
        _dragMovement.BeginInteraction(position, transform.position);
        _colorable.AssignOriginal();
        _voiceover.PlaySfx(_taking);
    }

    private void Move(Vector2 position)
    {
        if (_isSelected)
        {
            _dragMovement.ProcessInput(position, transform.position, _voiceover, _dragging);
        }
    }

    public void ThrowOff()
    {
        if (_isSelected)
        {
            _isSelected = false;
            _selectable.Deselect();
            _colorable.Disable();
            _voiceover.PlaySfx(_throwOff);
        }
    }
}