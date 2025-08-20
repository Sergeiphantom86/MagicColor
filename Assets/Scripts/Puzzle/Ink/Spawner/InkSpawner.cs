using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _inkDropPrefab;
    [SerializeField] private Ink _ink;

    private List<SmoothMoveToTarget> _smoothMovers;
    private int _quantity;
    private float _gridSpacing;
    private float _spawnDelay;
    
    private WaitForSeconds _waitForSeconds;

    private void Awake()
    {
        _quantity = 4;
        _spawnDelay = 0.05f;
        _gridSpacing = 0.25f;
        _smoothMovers = new List<SmoothMoveToTarget>();
        _waitForSeconds = new WaitForSeconds(_spawnDelay);
    }

    public void ActivateInkDrops(Color color)
    {
        StartCoroutine(SpawnAndActivateRoutine(color));
    }

    private IEnumerator SpawnAndActivateRoutine(Color color)
    {
        for (int i = 0; i < _quantity * _quantity; i++)
        {
            SpawnSingleInkDrop(GetRow(i), GetCol(i), color);

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
            }

            yield return _waitForSeconds;
        }
    }

    private void SpawnSingleInkDrop(int row, int col, Color color)
    {
        Vector3 spawnPosition = CalculateSpawnPosition(row, col);

        GameObject inkDrop = Instantiate(_inkDropPrefab, spawnPosition, Quaternion.identity, _ink.transform);
        inkDrop.SetActive(true);

        TrySetColor(inkDrop, color);
        TryAddMover(inkDrop);
    }

    private void TrySetColor(GameObject inkDrop, Color color)
    {
        if (inkDrop.TryGetComponent(out ColorableObject colorable))
            colorable.SetColor(color);
    }

    private void TryAddMover(GameObject inkDrop)
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