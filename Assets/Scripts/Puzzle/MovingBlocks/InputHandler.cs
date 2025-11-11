using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private Touch _touch;

    public event Action<Vector2> OnSelected;
    public event Action<Vector2> OnMoved;
    public event Action OnThrowed;

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
                OnMoved?.Invoke(_touch.position);
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                OnThrowed?.Invoke();
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

            case bool _ when Input.GetMouseButton(0):
                OnMoved?.Invoke(currentMousePosition);
                break;

            case bool _ when Input.GetMouseButtonUp(0):
                OnThrowed?.Invoke();
                break;
        }
    }

    private void SelectBlock(Vector2 position)
    {
        if (IsTouchingThisObject(position))
        {
            OnSelected?.Invoke(position);
        }
    }

    private bool IsTouchingThisObject(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.collider.transform == transform ||
                   hit.collider.transform.IsChildOf(transform);
        }

        return false;
    }
}