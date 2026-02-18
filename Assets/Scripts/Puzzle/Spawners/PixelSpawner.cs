using UnityEngine;
using System.Collections.Generic;

public class PixelSpawner
{
    private readonly PixelPool _pixelPool;
    private readonly Transform _parent;
    private readonly ColorPrecision _precision;
    private readonly bool _saveCollections;

    private readonly List<Fragment> _fragmentsList;
    private readonly Dictionary<Color, Queue<Fragment>> _fragments;

    public IReadOnlyList<Fragment> FragmentsList => _fragmentsList;
    public IReadOnlyDictionary<Color, Queue<Fragment>> Fragments => _fragments;

    public PixelSpawner(
        PixelPool pixelPool,
        Transform parent,
        bool saveCollections)
    {
        _pixelPool = pixelPool;
        _parent = parent;
        _saveCollections = saveCollections;

        _precision = new ColorPrecision();
        _fragmentsList = new List<Fragment>();
        _fragments = new Dictionary<Color, Queue<Fragment>>();
    }

    public void Spawn(int x, int y, Color color, Vector2 pivot, int pixelSize)
    {
        if (!_pixelPool.Pool.Get().TryGetComponent(out Fragment fragment))
            return;

        color = _precision.Reduce(color);

        fragment.transform.SetParent(_parent);
        fragment.transform.SetLocalPositionAndRotation(
            new Vector3((x - pivot.x) * pixelSize, (y - pivot.y) * pixelSize, 0),
            Quaternion.identity);

        fragment.transform.localScale = Vector3.one;
        fragment.SetColor(color);

        if (_saveCollections)
        {
            if (!_fragments.ContainsKey(color))
                _fragments[color] = new Queue<Fragment>();

            _fragments[color].Enqueue(fragment);
            fragment.TurnOnTransparency();
        }
        else
        {
            _fragmentsList.Add(fragment);
        }
    }

    public void Clear()
    {
        _pixelPool.ReturnAllFragments(_fragmentsList);
        _fragmentsList.Clear();
        _fragments.Clear();
    }
}
