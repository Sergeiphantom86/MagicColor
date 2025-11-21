using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkSpawner : MonoBehaviour
{
    [SerializeField] private Drop _inkDropPrefab;
    [SerializeField] private Ink _ink;
    [SerializeField] private AudioClip _spawn;
    [SerializeField] private AudioClip _moving;

    private int _quantity;
    private float _spawnDelay;
    private float _gridSpacing;
    private Voiceover _voiceover;
    private List<SmoothMoveToTarget> _smoothMovers;
    
    private WaitForSeconds _waitForSeconds;

    private void Awake()
    {
        _quantity = 4;
        _spawnDelay = 0.05f;
        _gridSpacing = 0.25f;
        _smoothMovers = new List<SmoothMoveToTarget>();
        _waitForSeconds = new WaitForSeconds(_spawnDelay);
        _voiceover = GetComponent<Voiceover>();
    }

    public void ActivateInkDrops(Color color, ParticleSystem particleSystem)
    {
        StartCoroutine(SpawnAndActivateRoutine(color, particleSystem));
    }

    private IEnumerator SpawnAndActivateRoutine(Color color, ParticleSystem particleSystem)
    {
        for (int i = 0; i < _quantity * _quantity; i++)
        {
            _voiceover.Play(_spawn);
            SpawnSingleInkDrop(GetRow(i), GetCol(i), color, particleSystem);
            yield return _waitForSeconds;
        }

        yield return StartCoroutine(ActivateDropsRoutine());

        particleSystem.gameObject.SetActive(false);
    }

    private IEnumerator ActivateDropsRoutine()
    {
        foreach (var mover in _smoothMovers)
        {
            if (mover != null && mover.isActiveAndEnabled)
            {
                mover.BeginMovement();
                _voiceover.Play(_moving);
            }

            yield return _waitForSeconds;
        }
    }

    private void SpawnSingleInkDrop(int row, int col, Color color, ParticleSystem particleSystem)
    {
        Vector3 spawnPosition = CalculateSpawnPosition(row, col);

        Drop inkDrop = Instantiate(_inkDropPrefab, spawnPosition, Quaternion.identity, _ink.transform);
        inkDrop.SetActive(true);
        particleSystem.gameObject.SetActive(true);
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

    private int GetRow(int index)
    {
        return index / _quantity;
    }

    private int GetCol(int index)
    {
        return index % _quantity;
    }

    private Vector3 CalculateSpawnPosition(int row, int col)
    {
        float x = transform.position.x + (col * _gridSpacing);
        float z = transform.position.z + (row * _gridSpacing);

        return new Vector3(x, transform.position.y, z);
    }
}