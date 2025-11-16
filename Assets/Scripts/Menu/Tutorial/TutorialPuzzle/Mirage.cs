using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HandMover), typeof(Renderer), typeof(PathMover))]
[RequireComponent(typeof(Collider), typeof(Voiceover))]
public class Mirage : MonoBehaviour
{
    [SerializeField] private AudioClip _audioClip;

    private bool _isMoved;
    private bool _isColored;
    private float _duration;
    private float _timeRepaint;

    private Color _color;
    private Wall _wall;
    private Material _material;
    private Collider _collider;
    private Voiceover _voiceover;
    private Indicator _indicator;
    private PathMover _pathMover;
    private HandMover _handMover;
    private Renderer _rendererWall;
    private Renderer _rendererBlock;
    private WaitForSeconds _waitColorChange;
    private WaitForSeconds _waitBeforeChangingLanes;
    private ColorCollisionHandler _colorCollisionHandler;

    public event Action OnMovement;
    public event Action OnCompleted;

    private void Awake()
    {
        _duration = 1;
        _timeRepaint = 0.001f;
        _isMoved = true;
        _collider = GetComponent<Collider>();
        _pathMover = GetComponent<PathMover>();
        _voiceover = GetComponent<Voiceover>();
        _handMover = GetComponent<HandMover>();
        _rendererBlock = GetComponent<Renderer>();

        _waitColorChange = new WaitForSeconds(_timeRepaint);
        _waitBeforeChangingLanes = new WaitForSeconds(_duration);

        _rendererBlock.material.color = Color.red;

        gameObject.SetActive(false);
    }

    public void EnableMoveAnimationZ()
    {
        _handMover.EnableMoveAnimationZ();
    }
    public void EnableMoveAnimationX()
    {
        _handMover.EnableMoveAnimationX();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Wall wall) == false) return;

        Move(wall);

        Repaint(wall);
    }

    private void Move(Wall wall)
    {
        if (_isMoved == false) return;

        _voiceover.Play(_audioClip);

        _wall = wall;

        _isMoved = false;

        StartCoroutine(WaitMove());
    }

    private void Repaint(Wall wall)
    {
        if (_isColored == false) return;

        _voiceover.Play(_audioClip);

        _wall = wall;

        _collider.enabled = false;

        InitializingComponents();

        StartCoroutine(WaitColorChange(_rendererBlock, _rendererWall));

    }

    private void InitializingComponents()
    {
        if (_wall.TryGetComponent(out ColorCollisionHandler colorCollisionHandler) == false) return;

        if (_wall.TryGetComponent(out Indicator indicator) == false) return;

        if (_wall.TryGetComponent(out Renderer renderer) == false) return;

        _colorCollisionHandler = colorCollisionHandler;
        _indicator = indicator;
        _rendererWall = renderer;
    }

    private IEnumerator WaitColorChange(Renderer blockRenderer, Renderer wallRenderer)
    {
        SaveMaterial(wallRenderer.material);
        SaveColor(wallRenderer.material.color);

        wallRenderer.material = blockRenderer.material;

        yield return _waitColorChange;

        wallRenderer.material.color = blockRenderer.material.color;

        yield return _waitBeforeChangingLanes;

        ContinueDriving(wallRenderer);

        OnCompleted?.Invoke();
    }

    private IEnumerator WaitMove()
    {
        yield return _waitBeforeChangingLanes;

        OnMovement?.Invoke();

        _isColored = true;
    }

    private void ContinueDriving(Renderer renderer)
    {
        _colorCollisionHandler.TriggerContactEvent(GetComponent<Block>());

        ReturnMaterial(renderer, _material, _color);

        _pathMover.Move(_indicator.transform, _wall.Point);
    }

    private void SaveMaterial(Material material)
    {
        _material = material;
    }

    private void SaveColor(Color color)
    {
        _color = color;
    }

    private void ReturnMaterial(Renderer renderer, Material material, Color color)
    {
        renderer.material = material;
        renderer.material.color = color;
    }
}