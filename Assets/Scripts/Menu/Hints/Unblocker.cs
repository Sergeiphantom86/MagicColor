using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Unblocker : MonoBehaviour
{
    [SerializeField] private Point _endPoint;
    [SerializeField] private float _moveDuration = 2f;
    [SerializeField] private float _rotationDuration = 1f;
    [SerializeField] private Ease _moveEase = Ease.InOutSine;
    [SerializeField] private Ease _rotationEase = Ease.InOutSine;
    [SerializeField] private Rotator _rotate;

    private Sequence _movementSequence;
    private Transform _target;
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Wall wall))
        {
            _target = wall.Point;
            CreateMovementSequence(_target.position);
        }
    }

    private void OnEnable()
    {
        _rotate.OnRotated += TurnOnCollider;
    }

    private void OnDisable()
    {
        _rotate.OnRotated -= TurnOnCollider;
    }

    private void TurnOnCollider()
    {
        _collider.enabled = true;
    }

    public void Play()
    {
        _movementSequence.Play();
    }

    public void CreateMovementSequence(Vector3 position)
    {
        _movementSequence?.Kill();
        _movementSequence = DOTween.Sequence();
        
        Move(position, _moveDuration / 4);
        AddWaypointToSequence(GetTarget(position), new Vector3(), _moveDuration /2, _rotationDuration / 2);
        AddWaypointToSequence(_endPoint.transform.position, new Vector3(90, 0, 0), _moveDuration / 2, _rotationDuration);
        _movementSequence.Pause();
    }

    private void AddWaypointToSequence(Vector3 position, Vector3 angleRotation, float moveDuration, float rotationDuration)
    {
        Rotate(angleRotation, rotationDuration);

        Move(position, moveDuration);
    }

    private void Rotate(Vector3 angleRotation, float rotationDuration)
    {
        _movementSequence.Append(transform.DORotate(angleRotation, rotationDuration)
            .SetEase(_rotationEase));
    }

    private void Move(Vector3 position, float moveDuration)
    {
        _movementSequence.Join(transform.DOMove(position, moveDuration)
           .SetEase(_moveEase));
    }

    private Vector3 GetTarget(Vector3 position)
    {
        position.y = 0.001f * 0.001f;

        return position;
    }

    private void OnDestroy()
    {
        _movementSequence?.Kill();
    }
}