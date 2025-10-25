using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Placeholder _objectToSpawn;

    private float _nextSpawnYPosition;
    private float _defaultDuration;
    private Transform _transform;
    private Placeholder _placeholder;
    private List<Placeholder> _placeholders;

    private void Awake()
    {
        _defaultDuration = 1;
        _transform = transform;
        _nextSpawnYPosition = 0f;
        _placeholders = new List<Placeholder>();
    }

    public void SpawnObject(Color color)
    {
        if (_objectToSpawn == null)
        {
            Debug.LogError("Object to spawn is not assigned!", this);
            return;
        }

        if (color == null)
        {
            Debug.LogError("Ñolor for the appearance is null!", this);
            return;
        }

        StartCoroutine(UpdatePointInCreatedObject(GetPlaceholder(), color));
    }

    public void Reduce()
    {
        if (_placeholder == null)
        {
            _placeholder = _placeholders[0];
            Debug.Log("If");
            _placeholders.RemoveAt(0);
        }

        _placeholder.ReduceSize();
        Debug.Log("ReduceSize");
    }

    private IEnumerator UpdatePointInCreatedObject(Placeholder spawnedObject, Color color)
    {
        if (spawnedObject != null)
        {
            spawnedObject.ShowFillings(color, _nextSpawnYPosition);
        }

        yield return new WaitForSeconds(GetDelayTime(spawnedObject.Duration));

        AssignNextSpawnPosition(spawnedObject.PositionEndPoint);
    }

    private float GetDelayTime(float duration)
    {
        return duration <= 0  ? duration : _defaultDuration;
    }

    private void AssignNextSpawnPosition(Vector3 position)
    {
        _nextSpawnYPosition = _transform.InverseTransformPoint(position).y;
    }

    private Placeholder GetPlaceholder()
    {
        Placeholder placeholder = Instantiate(_objectToSpawn, _transform);

        _placeholders.Add(placeholder);
        
        return placeholder;
    }
}