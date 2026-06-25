using System;
using PuzzleEditor.MovingBlocks.GridEditor;
using PuzzleEditor.SoundEditor;
using UnityEngine;

namespace PuzzleEditor.MovingBlocks
{
    [RequireComponent(typeof(GridDragMovement), typeof(Magnifier), typeof(IInputHandler))]
    [RequireComponent(typeof(IColorable), typeof(Voiceover))]
    public class TouchDragInput : MonoBehaviour, ITouchDragInput
    {
        private bool _isSelected;
        private Magnifier _selectable;
        private IColorable _colorable;
        private GridDragMovement _dragMovement;
        private IInputHandler _inputHandler;
        private Voiceover _voiceover;
        private Outline _outline;

        public bool IsSelected => _isSelected;

        public event Action<Vector2> OnTouchClick;

        public event Action<Vector2> OnTouchDrag;

        public event Action OnDropped;

        private void Awake()
        {
            _colorable = GetComponent<IColorable>();
            _selectable = GetComponent<Magnifier>();
            _voiceover = GetComponent<Voiceover>();
            _inputHandler = GetComponent<IInputHandler>();
            _dragMovement = GetComponent<GridDragMovement>();
            _outline = GetComponent<Outline>();

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

        private void SelectBlock(Vector2 position)
        {
            _isSelected = true;
            _outline.enabled = true;
            _colorable.SetRenderQueueSelectedItem();
            _selectable.Select();
            _colorable.AssignOriginal();
            OnTouchClick?.Invoke(position);
        }

        private void Move(Vector2 position)
        {
            if (_isSelected)
            {
                OnTouchDrag?.Invoke(position);
            }
        }

        public void ThrowOff()
        {
            if (_isSelected)
            {
                _isSelected = false;
                _outline.enabled = false;
                _selectable.Deselect();
                _colorable.Disable();
                _colorable.SetStartRenderQueueSelectedItem();
                OnDropped?.Invoke();
            }
        }
    }
}