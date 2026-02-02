using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DropPooler), typeof(Voiceover))]
public class InkSpawner : BaseSpawner<Drop>
{
    [SerializeField] private Ink _ink;
    [SerializeField] private AudioClip _spawn;
    [SerializeField] private AudioClip _moving;

    private int _quantity;
    private float _spawnDelay;
    private float _gridSpacing;
    private Voiceover _voiceover;
    private Coroutine _spawnRoutine;
    private List<SmoothMoveToTarget> _smoothMovers;

    private WaitForSeconds _waitForSeconds;

    protected override void Awake()
    {
        base.Awake();
        _quantity = 4;
        _spawnDelay = 0.06f;
        _gridSpacing = 0.25f;
        _voiceover = GetComponent<Voiceover>();
        _smoothMovers = new List<SmoothMoveToTarget>();
        _waitForSeconds = new WaitForSeconds(_spawnDelay);
    }

    private void OnDisable()
    {
        _smoothMovers.Clear();
        StopAllCoroutines();
    }

    public void ActivateInkDrops(Color color, float scale)
    {
        _gridSpacing = scale;

        if (_spawnRoutine != null)
            StopCoroutine(_spawnRoutine);

        _smoothMovers.Clear();
        _spawnRoutine = StartCoroutine(SpawnAndActivateRoutine(color));
    }


    private IEnumerator SpawnAndActivateRoutine(Color color)
    {
        for (int i = 0; i < _quantity * _quantity; i++)
        {
            _voiceover.Play(_spawn);

            int row = i / _quantity;
            int col = i % _quantity;

            SpawnSingleInkDrop(row, col, color);

            yield return _waitForSeconds;
        }

        yield return StartCoroutine(ActivateDropsRoutine());
    }

    private IEnumerator ActivateDropsRoutine()
    {
        foreach (var mover in _smoothMovers)
        {
            if (mover != null && mover.isActiveAndEnabled)
            {
                mover.BeginMovement();
                _voiceover.Play(_moving);

                yield return _waitForSeconds;
            }
        }
    }

    private void SpawnSingleInkDrop(int row, int col, Color color)
    {
        Vector3 spawnPosition = new (
            transform.position.x + col * _gridSpacing,
            transform.position.y,
            transform.position.z + row * _gridSpacing
        );

        Drop inkDrop = SpawnObject(spawnPosition, _ink.transform);
        TrySetColor(inkDrop, color);
        TryAddMover(inkDrop);
    }

    private void TrySetColor(Drop inkDrop, Color color)
    {
        if (inkDrop.TryGetComponent(out IColorable colorable))
            colorable.SetColor(color);
    }

    private void TryAddMover(Drop inkDrop)
    {
        if (inkDrop.TryGetComponent(out SmoothMoveToTarget mover))
            _smoothMovers.Add(mover);
    }
}