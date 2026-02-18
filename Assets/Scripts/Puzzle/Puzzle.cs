using DG.Tweening;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    private Rotator _rotation;
    private IProgressSaver _progressSaver;

    private void Awake()
    {
        _rotation = GetComponent<Rotator>();
        _progressSaver = new ProgressSaver();
    }

    private void Start()
    {
        _progressSaver.SetAutomaticTransition(false);
        _progressSaver.SaveProgress();
    }

    public void Return(float duration)
    {
        MoveX(duration);
    }

    public void StartRotation()
    {
        _rotation.StartRotation();
    }

    private void MoveX(float duration)
    {
        transform.DOMoveX(transform.position.x - 1000, duration)
           .SetEase(Ease.Linear);
    }
}