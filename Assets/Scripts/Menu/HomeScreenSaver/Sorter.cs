using System;
using System.Collections.Generic;
using PuzzleResources;
using UnityEngine;

namespace Menu.HomeScreenSaver
{
    [RequireComponent(typeof(FragmentCollector))]

    public class Sorter : MonoBehaviour
    {
        private FragmentCollector _fragmentCollector;
        private List<Fragment> _fragments;

        public event Action HasSorted;

        public List<Fragment> Fragments => _fragments;

        private void Awake()
        {
            _fragmentCollector = GetComponent<FragmentCollector>();
            _fragments = new List<Fragment>();
        }

        private void OnEnable()
        {
            _fragmentCollector.PixelsRendered += OnSortFragments;
        }

        private void OnDisable()
        {
            _fragmentCollector.PixelsRendered -= OnSortFragments;
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

        private void OnSortFragments(List<Fragment> fragments)
        {
            if (fragments == null || fragments.Count == 0)
            return;

            fragments.Sort(CompareFragments);

            _fragments = fragments;
            HasSorted?.Invoke();
        }
    }
}