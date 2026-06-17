using System.Collections;
using UnityEngine;

[RequireComponent(typeof(DropPooler))]
public class InkSpawner : BaseSpawner<Drop>
{
    [SerializeField] private Ink _ink;

    private float _delay;
    private int _quantity;
    private Coroutine _spawnRoutine;
    private Vector3 _spawnPosition;
    private WaitForSeconds _waitForSeconds;

    protected override void Awake()
    {
        base.Awake();

        _delay = 0.1f;
        _quantity = 10;
        _waitForSeconds = new WaitForSeconds(_delay);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void ActivateInkDrops(Color color, float duration)
    {
        if (color == null)
        {
            Debug.LogError($"{nameof(ActivateInkDrops)}: Color == null!", this);
        }

        if (duration < 0)
        {
            Debug.LogError($"{nameof(ActivateInkDrops)}: Duration < 0!", this);
        }

        if (_spawnRoutine != null)
            StopCoroutine(_spawnRoutine);

        _spawnRoutine = StartCoroutine(SpawnAndActivateRoutine(color, duration));
    }

    private IEnumerator SpawnAndActivateRoutine(Color color, float duration)
    {
        yield return new WaitForSeconds(duration);

        for (int i = 0; i < _quantity; i++)
        {
            yield return _waitForSeconds;

            SpawnSingleInkDrop(color);
        }
    }

    private void SpawnSingleInkDrop(Color color)
    {
        _spawnPosition = transform.position;

        Drop inkDrop = SpawnObject(_spawnPosition, _ink.transform);

        TrySetColor(inkDrop, color);

        if (inkDrop.TryGetComponent(out IDropAnimation animator))
        {
            animator.Play(_spawnPosition);
            inkDrop.PlaySoundSpawn();
        }
    }

    private void TrySetColor(Drop inkDrop, Color color)
    {
        if (inkDrop.TryGetComponent(out IColorable colorable))
            colorable.SetColor(color);
    }
}