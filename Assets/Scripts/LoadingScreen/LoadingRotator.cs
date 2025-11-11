using UnityEngine;

public class LoadingRotator : MonoBehaviour
{
    private float _rotationSpeed;
    private bool _clockwise;

    private void Awake()
    {
        _rotationSpeed = 90f;
        _clockwise = true;
    }

    private void Update()
    {
        float direction = _clockwise ? -1f : 1f;

        transform.Rotate(Vector3.back, _rotationSpeed * direction * Time.deltaTime);
    }
}