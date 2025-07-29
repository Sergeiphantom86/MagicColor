using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Repainter : MonoBehaviour
{
    [SerializeField] private WallsContainer _wallsContainer;
    [SerializeField] private BlocksContainer _blocksContainer;

    private List<Color> _colors;
    private List<IColorable> _walls;
    private List<IColorable> _blocks;
    private ImageAnalyzer _imageAnalyzer;


    private void Awake()
    {
        _colors = new List<Color>();
        _walls = new List<IColorable>();
        _blocks = new List<IColorable>();

        _imageAnalyzer = GetComponent<ImageAnalyzer>();
    }

    private void Start()
    {
        InitializeColorables();
    }

    private void OnEnable()
    {
        _imageAnalyzer.CanPaint += UpdateSystem;
    }

    private void OnDisable()
    {
        _imageAnalyzer.CanPaint -= UpdateSystem;
    }

    private void InitializeColorables()
    {
        _walls = GetColorablesFromContainer(_wallsContainer.transform);
        _blocks = GetColorablesFromContainer(_blocksContainer.transform);
    }

    private List<IColorable> GetColorablesFromContainer(Transform container)
    {
        var list = new List<IColorable>();

        if (container == null)
        {
            Debug.LogWarning($"Контейнер {container?.name} пропал!", this);
            return list;
        }

        foreach (Transform child in container)
        {
            if (child.TryGetComponent(out IColorable colorable))
            {
                list.Add(colorable);
            }
        }

        return list;
    }

    private void UpdateSystem(List<Color> colors)
    {
        UpdateColors(colors);
        ReplaceColors(_walls);
        ReplaceColors(_blocks);
    }

    private void UpdateColors(List<Color> colors)
    {
        _colors.AddRange(colors);

        if (_colors.Count == 0)
        {
            Debug.LogWarning("В Color Analyzer нет доступных цветов!", this);
        }
    }

    private void ReplaceColors(List<IColorable> colorables)
    {
        if (ShouldRepaint(colorables) == false) return;

        var paintingData = PreparePaintingData(colorables);

        ExecutePainting(paintingData.Colors, paintingData.Walls);
    }

    private bool ShouldRepaint(List<IColorable> colorables)
    {
        return colorables.Count > 0 && _colors.Count > 0;
    }

    private (List<Color> Colors, List<IColorable> Walls) PreparePaintingData(List<IColorable> colorables)
    {
        return (
            Colors: ShuffleColors(_colors),
            Walls: SelectRandomColorables(colorables, _colors.Count)
        );
    }

    private List<Color> ShuffleColors(List<Color> colors)
    {
        return colors
            .OrderBy(_ => Guid.NewGuid())
            .ToList();
    }

    private List<IColorable> SelectRandomColorables(List<IColorable> colorables, int maxCount)
    {
        return colorables
            .OrderBy(_ => Guid.NewGuid())
            .Take(Mathf.Min(maxCount, colorables.Count))
            .ToList();
    }

    private void ExecutePainting(List<Color> colors, List<IColorable> colorables)
    {
        for (int i = 0; i < Mathf.Min(colors.Count, colorables.Count); i++)
        {
            colorables[i]?.SetColor(colors[i]);
        }
    }
}