using System;
using System.Collections.Generic;
using UnityEngine;

public class Sorter : MonoBehaviour
{
    private PixelRenderer _renderer;
    private List<Fragment> _fragments;

    public List<Fragment> Fragments => _fragments;

    public event Action HasSorted;

    private void Awake()
    {
        _renderer = GetComponent<PixelRenderer>();
        _fragments = new List<Fragment>();
    }

    private void OnEnable()
    {
        _renderer.OnPixelsRendered += SortFragments;
    }

    private void OnDisable()
    {
        _renderer.OnPixelsRendered -= SortFragments;
    }

    private void SortFragments()
    {
        if (_renderer.Fragments == null || _renderer.Fragments.Count == 0)
            return;

        _renderer.Fragments.Sort(CompareFragments);

        _fragments = _renderer.Fragments;
        HasSorted?.Invoke();
    }

    private static int CompareFragments(Fragment first, Fragment second)
    {
        if (first == null || second == null)
            return 0;

        Vector3 positionA = first.transform.position;
        Vector3 positionB = second.transform.position;

        int diagonalComparison = CompareByDiagonal(positionA, positionB);
        if (diagonalComparison != 0)
            return diagonalComparison;

        int heightComparison = CompareByHeight(positionA, positionB);
        if (heightComparison != 0)
            return heightComparison;

        return CompareByX(positionA, positionB);
    }

    private static int CompareByDiagonal(Vector3 positionA, Vector3 positionB)
    {
        float diagonalValueA = positionA.x - positionA.y;
        float diagonalValueB = positionB.x - positionB.y;
        return diagonalValueB.CompareTo(diagonalValueA);
    }

    private static int CompareByHeight(Vector3 positionA, Vector3 positionB)
    {
        return positionA.y.CompareTo(positionB.y);
    }

    private static int CompareByX(Vector3 positionA, Vector3 positionB)
    {
        return positionB.x.CompareTo(positionA.x);
    }
}