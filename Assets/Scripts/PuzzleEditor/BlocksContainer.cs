using System;
using System.Collections.Generic;
using System.Linq;
using PuzzleEditor.MovingBlocks.BlockEditor;
using PuzzleEditor.PoolEffects;
using PuzzleEditor.Spawners;
using UnityEngine;

namespace PuzzleEditor
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

        private List<Block> _blocks;
        private int _initialBlocksCount;
        private BlockSpawner _blockSpawner;
        private bool _isInitialize;
        private float _delayTime;

        public event Action EverythDestroyed;

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

        private void OnRegister(Block block)
        {
            _blocks.Add(block);

            block.Initialize(_effectImpact, _effectSmock, _effectDestruct, _soundDestruction, _soundDragg, _soundRaise, _matchSound);
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

        private void OnSubscribe(List<IColorable> colorableObjects)
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
                EverythDestroyed?.Invoke();
            }

            _blocks.Remove(block);

            block.Destroyed -= OnHandleBlockDestroyed;
        }
    }
}