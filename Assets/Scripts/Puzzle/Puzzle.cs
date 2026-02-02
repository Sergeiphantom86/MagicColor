using DG.Tweening;
using UnityEngine;
using YG;

public class Puzzle : MonoBehaviour
{
    private Rotator _rotation;

    private void Awake()
    {
        _rotation = GetComponent<Rotator>();
    }

    private void Start()
    {
        YG2.saves.SetAutomaticTransition(false);
        YG2.SaveProgress();
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