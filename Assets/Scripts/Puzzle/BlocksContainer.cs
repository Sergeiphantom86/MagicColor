using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BlocksContainer : MonoBehaviour, IBlocksContainer
{
    [SerializeField] private ImageAnalyzer _imageAnalyzer;

    private List<Block> _blocks;
    private int _initialBlocksCount;

    public List<Block> Blocks => _blocks;

    public event Action<string> StoppingTimer;
    public event Action BlockDestroyed;

    public int ActiveBlocksCount =>
        _blocks.Count(b => b != null && b.gameObject.activeSelf);

    private void Awake()
    {
        _blocks = new List<Block>();

        InitializeBlocks();
    }

    private void InitializeBlocks()
    {
        _blocks = GetComponentsInChildren<Block>(true).ToList();

        foreach (var block in _blocks)
        {
            block.OnDestroyed += HandleBlockDestroyed;
        }
    }

    private void OnEnable() =>
        _imageAnalyzer.CanPaint += SetQuantityBlocks;

    private void OnDisable() =>
        _imageAnalyzer.CanPaint -= SetQuantityBlocks;

    private void SetQuantityBlocks(List<Color> colors) =>
        _initialBlocksCount = colors.Count;

    private void HandleBlockDestroyed(Block block)
    {
        block.OnDestroyed -= HandleBlockDestroyed;

        _blocks.Remove(block);
        _initialBlocksCount--;

        if (_initialBlocksCount == 0)
        {
            StoppingTimer?.Invoke(_imageAnalyzer.GetNameSprite());
            BlockDestroyed?.Invoke();
        }
    }
}