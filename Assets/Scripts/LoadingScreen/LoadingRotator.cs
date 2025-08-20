using UnityEngine;

public class LoadingRotator : MonoBehaviour
{
    [SerializeField] private Transform _childTransform;

    private float _rotationSpeed;
    private bool _clockwise;

    private void Awake()
    {
        _rotationSpeed = 90f;
        _clockwise = true;
    }

    private void Reset()
    {
        if (transform.childCount > 0)
        {
            _childTransform = transform.GetChild(0);
        }
    }

    private void Update()
    {
        if (_childTransform == null) return;

        float direction = _clockwise ? -1f : 1f;

        _childTransform.Rotate(Vector3.back, _rotationSpeed * direction * Time.deltaTime);
    }
}