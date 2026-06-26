using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace PuzzleEditor.PenEditor
{
    [RequireComponent(typeof(PenScaleController))]

    public class MoverPen : MonoBehaviour, IMover
    {
        [SerializeField] private float _radius = 0.5f;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private float _positionX = 1.5f;
        [SerializeField] private float _positionZ = 5f;
        [SerializeField] private float _centerChangeInterval = 10f;

        private PenScaleController _scaleController;
        private Coroutine _movementCoroutine;
        private Tween _moveTween;
        private Vector3 _center;
        private bool _isRunning;

        private void Awake()
        {
            _center = new Vector3(_positionX, 0, _positionZ);

            _scaleController = GetComponent<PenScaleController>();

            if (_scaleController == null)
            {
                Debug.LogError($"{nameof(MoverPen)}: Не найден PenScaleController!", this);
            }
        }

        private void OnEnable()
        {
            StartMovementLoop();
        }

        private void OnDisable()
        {
            StopAllMovement();
        }

        private void OnDestroy()
        {
            StopAllMovement();
        }

        public IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
        {
            StopMovementLoop();

            if (ValidateMoveParameters(targetPosition, duration) == false)
            yield break;

            PrepareForProgrammaticMove();

            yield return ExecuteMove(targetPosition, duration);

            FinalizeMove();

            StartMovementLoop();
        }

        private void StartMovementLoop()
        {
            if (_movementCoroutine != null)
            return;

            _isRunning = true;
            _movementCoroutine = StartCoroutine(MovementLoop());
        }

        private void StopMovementLoop()
        {
            _isRunning = false;

            if (_movementCoroutine != null)
            {
                StopCoroutine(_movementCoroutine);
                _movementCoroutine = null;
            }

            _moveTween?.Kill();
            _moveTween = null;
        }

        private void StopAllMovement()
        {
            StopMovementLoop();
        }

        private IEnumerator MovementLoop()
        {
            while (_isRunning)
            {
                _center = new Vector3(_positionX, 0, _positionZ);
                yield return RandomMoveForSeconds(_centerChangeInterval);

                _center = new Vector3(-_positionX, 0, _positionZ);
                yield return RandomMoveForSeconds(_centerChangeInterval);
            }
        }

        private IEnumerator RandomMoveForSeconds(float time)
        {
            float timer = 0f;

            while (timer < time && _isRunning)
            {
                Vector3 target = GetRandomPointInCircle(_center, _radius);

                yield return ExecuteMove(target, _duration);

                timer += _duration;
            }
        }

        private IEnumerator ExecuteMove(Vector3 targetPosition, float duration)
        {
            _moveTween = transform.DOMove(targetPosition, duration).SetEase(Ease.InOutSine);

            yield return _moveTween.WaitForCompletion();
        }

        private bool ValidateMoveParameters(Vector3 targetPosition, float duration)
        {
            if (float.IsNaN(targetPosition.x) || float.IsInfinity(targetPosition.x))
            {
                Debug.LogError($"{nameof(MoverPen)}: Некорректная координата X", this);
                return false;
            }

            if (duration <= 0)
            {
                Debug.LogError($"{nameof(MoverPen)}: duration должен быть > 0", this);
                return false;
            }

            return true;
        }

        private void PrepareForProgrammaticMove()
        {
            if (_scaleController == null)
            return;

            _scaleController.Stop();
            _scaleController.StartScaleUp();
        }

        private void FinalizeMove()
        {
            if (_scaleController != null)
            {
                _scaleController.StartScaleDown();
            }
        }

        private Vector3 GetRandomPointInCircle(Vector3 center, float radius)
        {
            Vector2 randomPoint = Random.insideUnitCircle * radius;

            return new Vector3(center.x + randomPoint.x, center.y, center.z + randomPoint.y);
        }
    }
}