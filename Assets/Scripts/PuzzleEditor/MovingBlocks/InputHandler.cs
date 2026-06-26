using System;
using UnityEngine;

namespace PuzzleEditor.MovingBlocks
{
    public class InputHandler : MonoBehaviour, IInputHandler
    {
        private Touch _touch;

        public Vector3 Point { get; private set; }
        public event Action<Vector2> Selected;
        public event Action<Vector2> Moved;
        public event Action Throwed;

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
                Moved?.Invoke(_touch.position);
                break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                Throwed?.Invoke();
                Debug.Log(Input.imeIsSelected);
                break;
            }
        }

        private void HandleMouseInput()
        {
            Vector3 currentMousePosition = Input.mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                SelectBlock(currentMousePosition);
            }
            else if (Input.GetMouseButton(0))
            {
                Moved?.Invoke(currentMousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Throwed?.Invoke();
            }
        }

        private void SelectBlock(Vector2 position)
        {
            if (IsTouchingThisObject(position))
            {
                Selected?.Invoke(position);
            }
        }

        private bool IsTouchingThisObject(Vector2 screenPosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Point = hit.point;
                return hit.collider.transform == transform
                || hit.collider.transform.IsChildOf(transform);
            }

            Point = Vector2.zero;
            return false;
        }
    }
}