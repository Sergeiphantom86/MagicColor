using System;
using System.Collections.Generic;
using System.Linq;
using PuzzleResources.MovingBlocks.GridLogic;
using PuzzleResources.ColoringObjects;
using PuzzleResources.MovingBlocks;
using PuzzleResources.PoolEffects;
using PuzzleResources.Spawners;
using UnityEngine;

namespace PuzzleResources
{
    public class BlocksContainer : MonoBehaviour, IBlocksContainer
    {
        [SerializeField] private TextureInitializer _textureInitializer;
        [SerializeField] private Repainter _repainter;
        [SerializeField] private AudioClip _soundDragg;
        [SerializeField] private AudioClip _soundRaise;
        [SerializeField] private AudioClip _matchSound;
        [SerializeField] private AudioClip _soundDestruction;
        [SerializeField] private Effecter _effectImpact;
        [SerializeField] private Effecter _effectDestruct;
        [SerializeField] private Effecter _effectSmock;
        [SerializeField] private GridSystem _gridSystem;

        private List<Block> _blocks;
        private int _initialBlocksCount;
        private BlockSpawner _blockSpawner;
        private bool _isInitialize;
        private float _delayTime;

        public event Action EverythingDestroyed;

        public event Action Destroyed;

        public Transform Transform => transform;

        public float DelayTime => _delayTime;

        public int ActiveBlocksCount =>
        _blocks.Count(block => block != null && block.gameObject.activeSelf);

        private void Awake()
        {
            _blocks = new List<Block>();
            _blockSpawner = GetComponent<BlockSpawner>();
        }

        private void OnEnable()
        {
            _repainter.RecoloredBlock += OnSubscribe;
            _blockSpawner.BlockSpawned += OnRegister;
        }

        private void OnDisable()
        {
            _repainter.RecoloredBlock -= OnSubscribe;
            _blockSpawner.BlockSpawned -= OnRegister;
        }

        private void CalculateStartTimeGame(Block block)
        {
            if (_isInitialize == false)
            {
                _isInitialize = true;

                if (block.TryGetComponent(out SpawnDropAnimation spawnDropAnimation))
                {
                    _delayTime = spawnDropAnimation.Duration * _blocks.Count;
                }
            }
        }

        private void OnSubscribe(List<IColorable> _)
        {
            foreach (var block in _blocks)
            {
                CalculateStartTimeGame(block);

                if (block.IsRepainted)
                {
                    block.Destroyed += OnHandleBlockDestroyed;
                    _initialBlocksCount++;
                }
            }
        }

        private void OnHandleBlockDestroyed(Block block)
        {
            _initialBlocksCount--;

            Destroyed?.Invoke();

            if (_initialBlocksCount == 0)
            {
                EverythingDestroyed?.Invoke();
            }

            _blocks.Remove(block);

            block.Destroyed -= OnHandleBlockDestroyed;
        }

        private void OnRegister(Block block)
        {
            _blocks.Add(block);

            block.Initialize(
                _effectImpact,
                _effectSmock,
                _effectDestruct,
                _soundDestruction,
                _soundDragg,
                _soundRaise,
                _matchSound,
                _gridSystem);
        }
    }
}