using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class Repainter : MonoBehaviour
{
    [SerializeField] private WallsContainer _wallsContainer;
    [SerializeField] private BlocksContainer _blocksContainer;

    private List<Color> _colors;
    private List<IColorable> _walls;
    private List<IColorable> _blocks;
    private IBlocksContainer _iBlocksContainer;
    private ImageAnalyzer _imageAnalyzer;

    public event Action<List<IColorable>> OnRecoloredWalls;
    public event Action<List<IColorable>> OnRecoloredBlock;

    private void Awake()
    {
        _colors = new List<Color>();
        _walls = new List<IColorable>();
        _blocks = new List<IColorable>();
        _imageAnalyzer = GetComponent<ImageAnalyzer>();
        _iBlocksContainer = _blocksContainer;
    }

    private void OnEnable()
    {
        _imageAnalyzer.CanPaint += UpdateSystem;
    }

    private void OnDisable()
    {
        _imageAnalyzer.CanPaint -= UpdateSystem;
    }

    private List<IColorable> GetColorablesFromContainer(Transform container)
    {
        var list = new List<IColorable>();

        if (container == null)
        {
            Debug.LogWarning($"Контейнер {container.name} пропал!", this);
            return list;
        }

        return GetColorables(container, list);
    }

    private List<IColorable> GetColorables(Transform container, List<IColorable> IColorables)
    {
        foreach (Transform child in container)
        {
            if (child.TryGetComponent(out IColorable colorable))
            {
                IColorables.Add(colorable);
            }
        }

        return IColorables;
    }

    private void UpdateSystem(List<Color> colors)
    {
        _walls = GetColorablesFromContainer(_wallsContainer.transform);
        _blocks = GetColorablesFromContainer(_iBlocksContainer.Transform);

        if (_walls.Count < 0)
        {
            Debug.LogError($"Количество Walls = {_walls.Count} {this}");
            return;
        }

        if (_blocks.Count < 0)
        {
            Debug.LogError($"Количество Blocks = {_blocks.Count} {this}");
            return;
        }
       
        UpdateColors(colors);

        ReplaceColors(_walls);
        OnRecoloredWalls?.Invoke(_walls);

        ReplaceColors(_blocks);
        OnRecoloredBlock?.Invoke(_blocks);
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

        var (Colors, Walls) = PreparePaintingData(colorables);

        ExecutePainting(Colors, Walls);
    }

    private bool ShouldRepaint(List<IColorable> colorables)
    {
        return colorables.Count > 0 && _colors.Count > 0;
    }

    private (List<Color> Colors, List<IColorable> Walls) PreparePaintingData(List<IColorable> colorables)
    {
        return (
            Colors: ShuffleColors(_colors),
            Walls: SelectRandomColorables(colorables, _colors.Count));
    }

    private List<Color> ShuffleColors(List<Color> colors)
    {
        return colors.OrderBy(_ => Guid.NewGuid()).ToList();
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
            colorables[i]?.InstallRepainted();
            colorables[i]?.SetColor(colors[i]);
        }
    }
}