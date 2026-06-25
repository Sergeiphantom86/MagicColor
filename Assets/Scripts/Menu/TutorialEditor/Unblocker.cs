using DG.Tweening;
using PuzzleEditor;
using PuzzleEditor.Walls;
using PuzzleEditor.Walls.WallEditor;
using UnityEngine;

namespace Menu.TutorialEditor
{
    public class Unblocker : MonoBehaviour
    {
        [SerializeField]
        private Point _endPoint;

        [SerializeField]
        private float _moveDuration;

        [SerializeField]
        private float _rotationDuration;

        [SerializeField]
        private Ease _moveEase = Ease.InOutSine;

        [SerializeField]
        private Ease _rotationEase = Ease.InOutSine;

        [SerializeField]
        private Rotator _rotate;

        private Sequence _movementSequence;
        private Vector3 _firstPointTarget;
        private Vector3 _secondPointTarget;
        private Collider _collider;
        private int _angleX;
        private float _durationMultiplier;
        private float _heightMultiplier;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.enabled = false;
            _durationMultiplier = 0.25f;
            _heightMultiplier = 7;
            _angleX = 90;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Wall wall))
            {
                _firstPointTarget = wall.EndPoint;
                _secondPointTarget.y = wall.Height;

                CreateMovementSequence();
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

        public void Play()
        {
            _movementSequence.Play();
        }

        public void CreateMovementSequence()
        {
            _movementSequence?.Kill();
            _movementSequence = DOTween.Sequence();

            Move(_firstPointTarget, _moveDuration * _durationMultiplier);

            SetLiftingHeight();

            AddWaypointToSequence(
                _firstPointTarget,
                GetAngleRotation(),
                _moveDuration,
                _rotationDuration
            );
            AddWaypointToSequence(
                _endPoint.transform.position,
                GetAngleRotation(_angleX),
                _moveDuration,
                _rotationDuration
            );

            _movementSequence.Pause();
        }

        private void SetLiftingHeight()
        {
            _firstPointTarget.y += _secondPointTarget.y * _heightMultiplier;
        }

        private void TurnOnCollider()
        {
            _collider.enabled = true;
        }

        private void AddWaypointToSequence(
            Vector3 position,
            Vector3 angleRotation,
            float moveDuration,
            float rotationDuration
        )
        {
            Rotate(angleRotation, rotationDuration);

            Move(position, moveDuration);
        }

        private Vector3 GetAngleRotation(float angleX = 0, float angleY = 0, float angleZ = 0)
        {
            return new Vector3(angleX, angleY, angleZ);
        }

        private void Rotate(Vector3 angleRotation, float rotationDuration)
        {
            _movementSequence.Append(
                transform.DORotate(angleRotation, rotationDuration).SetEase(_rotationEase)
            );
        }

        private void Move(Vector3 position, float moveDuration)
        {
            _movementSequence.Join(transform.DOMove(position, moveDuration).SetEase(_moveEase));
        }

        private void OnDestroy()
        {
            _movementSequence?.Kill();
        }
    }
}