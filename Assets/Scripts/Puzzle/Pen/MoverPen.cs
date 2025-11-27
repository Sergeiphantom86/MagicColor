using DG.Tweening;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PenScaleController))]
public class MoverPen : MonoBehaviour, IMover
{
    private PenScaleController _scaleController;
    private void Awake()
    {
        _scaleController = GetComponent<PenScaleController>();

        if (_scaleController == null)
        {
            Debug.LogError($"{nameof(MoverPen)}: Не удалось получить PenscaleController!", this);
            return;
        }
    }

    public IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        if (ValidateMoveParameters(targetPosition, duration) == false)
            yield break;

        PrepareForProgrammaticMove();

        yield return ExecuteMoveSequence(targetPosition, duration);

        FinalizeMove();
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
    }
}