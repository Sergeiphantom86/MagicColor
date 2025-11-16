using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PathMover), typeof(TouchColorTransparency), typeof(Renderer))]
public class Block : ColorableObject, IDestroyable
{
    [SerializeField] private Rotator _rotation;

    private Vector2Int _gridPosition;
    private PathMover _pathMover;
    private InkSpawner _inkSpawner;
    private Renderer _renderer;
    private TouchDragInput _touchDragInput;
    private Collider _collider;
    private Voiceover _voiceover;
    private AudioClip _audioClip;

    public event Action<Block> OnDestroyed;

    public void  Initialize(AudioClip audioClip)
    {
        _pathMover = GetComponent<PathMover>();
        _touchDragInput = GetComponent<TouchDragInput>();
        _inkSpawner = GetComponentInChildren<InkSpawner>();
        _renderer = GetComponent<Renderer>();
        _voiceover = GetComponent<Voiceover>();
        _collider = GetComponent<Collider>();
        _audioClip = audioClip;
        _collider.enabled = true;
        InitializeComponents();
    }

    public void Destroy(Transform waypoint, Transform endPoint)
    {
        _collider.enabled = false;
        LetGo();
        AssignOriginal();
        _voiceover.Play(_audioClip);
        _pathMover.Move(waypoint, endPoint, ExecuteDestruction);
    }

    private void LetGo()
    {
        _touchDragInput.ThrowOff();
    }

    private void ExecuteDestruction()
    {
        OnDestroyed?.Invoke(this);

        if (GridSystem.Instance != null)
        {
            GridSystem.Instance.ClearCell(_gridPosition);
        }

        _inkSpawner.ActivateInkDrops(GetColor());

       StartCoroutine(Wait());
    }

    public void SetGridPosition(Vector2Int gridPosition)
    {
        _gridPosition = gridPosition;
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.3f);

        _renderer.enabled = false;
    }
}