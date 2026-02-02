using DG.Tweening;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PenScaleController))]
public class MoverPen : MonoBehaviour, IMover
{
    private PenScaleController _scaleController;
    private Coroutine _randomMoveCoroutine;
    private Tween _randomMoveTween;
    private Quaternion _initialRotation;

    private void Awake()
    {
        _scaleController = GetComponent<PenScaleController>();

        if (_scaleController == null)
        {
            Debug.LogError($"{nameof(MoverPen)}: Не удалось получить PenscaleController!", this);
            return;
        }
    }

    private void Start()
    {
        StartRandomMove(new Vector3(2.5f, 0, 5), 1);
    }

    public IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        StopRandomMove();

        if (ValidateMoveParameters(targetPosition, duration) == false)
            yield break;

        PrepareForProgrammaticMove();
        yield return ExecuteMoveSequence(targetPosition, duration);
        FinalizeMove();
    }

    public void StartRandomMove(
    Vector3 center,
    float radius,
    float minDuration = 0.5f,
    float maxDuration = 1.5f)
    {
        StopRandomMove();
        transform.rotation = _initialRotation;
        _randomMoveCoroutine = StartCoroutine(RandomMoveRoutine(center, radius, minDuration, maxDuration));
    }

    private void StopRandomMove()
    {
        if (_randomMoveCoroutine != null)
        {
            StopCoroutine(_randomMoveCoroutine);
            _randomMoveCoroutine = null;
        }

        _randomMoveTween?.Kill();
        _randomMoveTween = null;
    }


    private bool ValidateMoveParameters(Vector3 targetPosition, float duration)
    {
        if (float.IsNaN(targetPosition.x) || float.IsInfinity(targetPosition.x))
        {
            Debug.LogError($"{nameof(MoverPen)}: Неверная координата X целевого положения!", this);
            return false;
        }

        if (duration <= 0)
        {
            Debug.LogError($"{nameof(MoverPen)}: Продолжительность должна быть положительной величиной! Получен: {duration}", this);
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

    private IEnumerator ExecuteMoveSequence(Vector3 targetPosition, float duration)
    {
        yield return transform.DOMove(targetPosition, duration)
            .SetEase(Ease.OutQuad).WaitForCompletion();
    }

    private void FinalizeMove()
    {
        if (_scaleController == null)
            return;

        _scaleController.StartScaleDown();

        StartRandomMove(new Vector3(2.5f, 0, 5), 1);
    }

    private IEnumerator RandomMoveRoutine(
     Vector3 center,
     float radius,
     float minDuration,
     float maxDuration)
    {
        while (true)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * radius;
            randomPoint.y = center.y;

            float duration = Random.Range(minDuration, maxDuration);

            _randomMoveTween = transform.DOMove(randomPoint, duration)
                .SetEase(Ease.InOutSine);

            yield return _randomMoveTween.WaitForCompletion();
        }
    }
}