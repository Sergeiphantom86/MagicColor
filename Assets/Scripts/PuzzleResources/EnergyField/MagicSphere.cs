using System.Collections;
using PuzzleResources.MovingBlocks.GridLogic;
using PuzzleResources.PoolEffects;
using UnityEngine;

namespace PuzzleResources.EnergyField
{
    public class MagicSphere : MonoBehaviour, IGridOccupant
    {
        [SerializeField] private Effecter _electricDischarge;
        [SerializeField] private Effecter _explosionEffect;
        [SerializeField] private Vector2Int _sizeInCells;

        private Vector2Int _gridPosition;
        private ParticleSystem _particleSystem;
        private Explosion _explosion;
        private float _scaleMultiplier;

        public Vector2Int SizeInCells => _sizeInCells;

        public GameObject GameObject => gameObject;

        public Vector2Int GridPosition => _gridPosition;

        private void Awake()
        {
            _scaleMultiplier = 2;
            _explosion = GetComponent<Explosion>();
        }

        private void Start()
        {
            GetEffect(_electricDischarge, _scaleMultiplier);
        }

        public void SetGridPosition(Vector2Int origin)
        {
            _gridPosition = origin;
        }

        public void EnableEndEffect()
        {
            StartCoroutine(WaitForParticles());
        }

        private IEnumerator WaitForParticles()
        {
            ParticleSystem particleSystem = GetEffect(_explosionEffect, 1);
            _explosion.Explode();
            yield return new WaitWhile(() => particleSystem.IsAlive(true));

            TurnOff();
        }

        private void TurnOff()
        {
            gameObject.SetActive(false);
        }

        private ParticleSystem GetEffect(Effecter effecter, float scaleMultiplier)
        {
            _particleSystem = effecter.CreatePooledItem();
            _particleSystem.transform.SetParent(transform);
            _particleSystem.transform.localPosition = Vector3.zero;
            _particleSystem.transform.localScale = Vector3.one / scaleMultiplier;
            _particleSystem.gameObject.SetActive(true);

            return _particleSystem;
        }
    }
}