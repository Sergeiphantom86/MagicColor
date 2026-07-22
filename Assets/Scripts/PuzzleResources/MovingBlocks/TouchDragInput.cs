using System;
using UnityEngine;
using PuzzleResources.Audio;
using PuzzleResources.ColoringObjects;
using PuzzleResources.MovingBlocks.GridLogic;

namespace PuzzleResources.MovingBlocks
{
    [RequireComponent(typeof(IRenderQueueConfigurable), typeof(IDisable), typeof(IRepaintable))]
    [RequireComponent(typeof(IInputHandler), typeof(IRenderQueueConfigurable))]
    [RequireComponent(typeof(Voiceover), typeof(GridDragMovement), typeof(Magnifier))]

    public class TouchDragInput : MonoBehaviour, ITouchDragInput
    {
        private bool _isSelected;
        private Outline _outline;
        private Voiceover _voiceover;
        private Magnifier _selectable;
        private GridDragMovement _dragMovement;
        private IDisable _disable;
        private IRepaintable _repaintable;
        private IInputHandler _inputHandler;
        private IRenderQueueConfigurable _renderQueueConfigurable;

        public event Action<Vector2> Touched;

        public event Action<Vector2> Dragging;

        public event Action Dropped;

        public bool IsSelected => _isSelected;

        private void Awake()
        {
            _outline = GetComponent<Outline>();
            _selectable = GetComponent<Magnifier>();
            _voiceover = GetComponent<Voiceover>();
            _dragMovement = GetComponent<GridDragMovement>();
            _disable = GetComponent<IDisable>();
            _repaintable = GetComponent<IRepaintable>();
            _inputHandler = GetComponent<IInputHandler>();
            _renderQueueConfigurable = GetComponent<IRenderQueueConfigurable>();

            if (_dragMovement == null)
            {
                Debug.LogError("DragMovement not assigned in TouchDragInput", this);
            }

            if (_selectable == null)
            {
                Debug.LogError("SelectableObject not assigned in TouchDragInput", this);
            }

            if (_voiceover == null)
            {
                Debug.LogError("Voiceover not assigned in TouchDragInput", this);
            }
        }

        private void OnEnable()
        {
            _inputHandler.Selected += OnSelectBlock;
            _inputHandler.Moved += OnMove;
            _inputHandler.Throwed += OnThrowOff;
        }

        private void OnDisable()
        {
            _inputHandler.Selected -= OnSelectBlock;
            _inputHandler.Moved -= OnMove;
            _inputHandler.Throwed -= OnThrowOff;
        }

        public void OnThrowOff()
        {
            if (_isSelected)
            {
                _isSelected = false;
                _outline.enabled = false;
                _selectable.Deselect();
                _disable.Disable();
                _renderQueueConfigurable.SetStartRenderQueueSelectedItem();
                Dropped?.Invoke();
            }
        }

        private void OnSelectBlock(Vector2 position)
        {
            _isSelected = true;
            _outline.enabled = true;
            _renderQueueConfigurable.SetRenderQueueSelectedItem();
            _selectable.Select();
            _repaintable.AssignOriginal();
            Touched?.Invoke(position);
        }

        private void OnMove(Vector2 position)
        {
            if (_isSelected)
            {
                Dragging?.Invoke(position);
            }
        }
    }
}