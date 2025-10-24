using System.Collections;
using UnityEngine;

public class SequentialSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Placeholder _objectToSpawn;
    [SerializeField] private Pen _pen;

    private EndPoint _endPoint;
    private float _nextSpawnYPosition;
    private float _defaultDuration;
    private Transform _transform;
    private GrowUpwards _growUpwards;

    private void Awake()
    {
        _defaultDuration = 1;
        _transform = transform;
        _nextSpawnYPosition = 0f;
    }

    public void SpawnObject(Color color)
    {
        if (_objectToSpawn == null)
        {
            Debug.LogError("Object to spawn is not assigned!", this);
            return;
        }

        Placeholder newObject = Instantiate(_objectToSpawn, _transform);

        newObject.transform.localPosition = new Vector3(0, _nextSpawnYPosition, 0);
        newObject.SetActive(true);

        if (newObject.TryGetComponent(out GrowUpwards growUpwards))
        {
            growUpwards.FillPen(color, newObject);
        }

        if (newObject.TryGetComponent(out IColorable colorable))
        {
            colorable.SetColor(color);
            colorable.SetAlpha(color, 1);
        }

        StartCoroutine(FindAndUpdatePointsInSpawnedObject(newObject));
    }

    private IEnumerator FindAndUpdatePointsInSpawnedObject(Placeholder spawnedObject)
    {
        _growUpwards = spawnedObject.GetComponent<GrowUpwards>();

        yield return new WaitForSeconds(GetDelayTime());

        EndPoint newEndPoint = spawnedObject.GetComponentInChildren<EndPoint>();

        if (newEndPoint != null) _endPoint = newEndPoint;

        if (_endPoint != null )
        {
            Vector3 endPointWorldPos = _endPoint.transform.position;
            Vector3 endPointLocalPos = _transform.InverseTransformPoint(endPointWorldPos);

            _nextSpawnYPosition = endPointLocalPos.y;
        }
        else
        {
            Debug.LogError("Не удалось найти StartPoint или EndPoint в созданном объекте!");
        }
    }

    private float GetDelayTime()
    {
        return _growUpwards != null ? _growUpwards.GetDuration() : _defaultDuration;
    }
}