using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlocksContainer : MonoBehaviour, IBlocksContainer
{
    [SerializeField] private ImageAnalyzer _imageAnalyzer;
    [SerializeField] private AudioClip _dragg;
    [SerializeField] private AudioClip _taking;
    [SerializeField] private AudioClip _throwOff;
    [SerializeField] private AudioClip _destruction;

    private List<Block> _blocks;
    private int _initialBlocksCount;

    public List<Block> Blocks => _blocks;
    public Transform Transform => transform;

    public event Action BlockDestroyed;

    public int ActiveBlocksCount =>
        _blocks.Count(b => b != null && b.gameObject.activeSelf);

    private void Awake()
    {
        _blocks = new List<Block>();

        _blocks = GetComponentsInChildren<Block>(true).ToList();

        if (ConfirmQuantities(_blocks, "блоков") == false) return;

        Initialize();
    }

    private void OnEnable() =>
       _imageAnalyzer.CanPaint += SetQuantityBlocks;

    private void OnDisable() =>
        _imageAnalyzer.CanPaint -= SetQuantityBlocks;

    private void Initialize()
    {
        foreach (var block in _blocks)
        {
            AddComponents(block);
        }
    }

    private void SetQuantityBlocks(List<Color> colors)
    {
        if (ConfirmQuantities(colors, "цветов") == false) return;

        _initialBlocksCount = colors.Count;
    }

    private bool ConfirmQuantities<T>(ICollection<T> collection, string collectionName)
    {
        if (collection == null || collection.Count <= 0)
        {
            Debug.Log($"Количество {collectionName} = 0 {this}");
            return false;
        }

        return true;
    }

    private void AddComponents(Block block)
    {
        block.Initialize(_destruction);

        block.OnDestroyed += HandleBlockDestroyed;

        if (block.TryGetComponent(out TouchDragInput touchDragInput))
        {
            touchDragInput.SetAudioClip(_dragg, _taking, _throwOff);
        }
    }

    private void HandleBlockDestroyed(Block block)
    {
        block.OnDestroyed -= HandleBlockDestroyed;

        _blocks.Remove(block);
        _initialBlocksCount--;

        if (_initialBlocksCount == 0)
        {
            BlockDestroyed?.Invoke();
        }
    }
}