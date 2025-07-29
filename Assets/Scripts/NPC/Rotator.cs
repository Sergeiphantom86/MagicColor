using DG.Tweening;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private DialogueAnimator _dialogueSystem;

    private Sequence _rotationSequence;
    private float _minDirection;
    private float _rotationDuration;

    private void Awake()
    {
        _minDirection = 0.001f;
        _rotationDuration = 1.5f;
    }

    public void StartRotationToPlayer(Vector3 playerPosition)
    {
        CleanupExistingSequence();

        Vector3 direction = GetDirection(playerPosition);

        direction.y = 0;

        if (IsDirectionTooSmall(direction))
        {
            StartDialogue();
            return;
        }

        CreateRotationSequence(direction);
        PlayRotationSequence();
    }

    private void CleanupExistingSequence()
    {
        if (_rotationSequence != null && _rotationSequence.IsActive())
        {
            _rotationSequence.Kill();
        }
        _rotationSequence = DOTween.Sequence();
    }

    private Vector3 GetDirection(Vector3 playerPosition)
    {
        Vector3 direction = playerPosition - transform.position;

        return direction;
    }

    private bool IsDirectionTooSmall(Vector3 direction)
    {
        return direction.sqrMagnitude < _minDirection;
    }

    private void CreateRotationSequence(Vector3 direction)
    {
        AddRotationToSequence(CalculateTargetRotation(direction));

        AddDialogueCallback();
    }

    private Quaternion CalculateTargetRotation(Vector3 direction)
    {
        return Quaternion.LookRotation(direction);
    }

    private void AddRotationToSequence(Quaternion targetRotation)
    {
        _rotationSequence.Append(transform.DORotateQuaternion(targetRotation, _rotationDuration));
    }

    private void AddDialogueCallback()
    {
        _rotationSequence.OnComplete(() => StartDialogue());
    }

    private void PlayRotationSequence()
    {
        _rotationSequence.Play();
    }

    private void StartDialogue()
    {
        if (_dialogueSystem == null)
        {
            Debug.LogWarning("DialogueSystem не назначен!");
            return;
        }

        _dialogueSystem.Show();
    }
}