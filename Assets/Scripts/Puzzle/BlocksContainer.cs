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
    private BlockSpawner _blockSpawner;

    public Transform Transform => transform;

    public event Action BlockDestroyed;

    public int ActiveBlocksCount =>
        _blocks.Count(block => block != null && block.gameObject.activeSelf);

    private void Awake()
    {
        _blocks = new List<Block>();

        _blockSpawner = GetComponent<BlockSpawner>();
    }

    private void Start()
    {
        _blocks = _blockSpawner.SpawnedBlocks;
    }

    private void OnEnable() =>
       _imageAnalyzer.Spawn += Initialize;

    private void OnDisable() =>
        _imageAnalyzer.Spawn -= Initialize;

    private void Initialize(int colorsCount)
    {
        _initialBlocksCount = colorsCount;

        foreach (var block in _blocks)
        {
            block.Initialize(_destruction);

            block.OnDestroyed += HandleBlockDestroyed;

            if (block.TryGetComponent(out TouchDragInput touchDragInput))
            {
                touchDragInput.SetAudioClip(_dragg, _taking, _throwOff);
            }
        }
    }

    private void HandleBlockDestroyed(Block block)
    {
        _initialBlocksCount--;

        if (_initialBlocksCount == 0)
        {
            BlockDestroyed?.Invoke();
        }

        _blocks.Remove(block);

        block.OnDestroyed -= HandleBlockDestroyed;
    }
}