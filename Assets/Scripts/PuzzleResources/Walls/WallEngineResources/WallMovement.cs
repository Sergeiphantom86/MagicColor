using DG.Tweening;
using PuzzleResources.Walls.WallResources;
using UnityEngine;

namespace PuzzleResources.Walls.WallEngineResources
{
    public class WallMovement : MonoBehaviour
    {
        [SerializeField] private float _moveDuration = 0.3f;
        [SerializeField] private float _partialOpenPercent = 0.3f;
        [SerializeField] private float _fullOpenPercent = 0.9f;

        private bool _isMoving;
        private Renderer _renderer;
        private Vector3 _startPosition;
        private Wall _wall;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _wall = GetComponent<Wall>();

            CacheStartPosition();
        }

        public void CacheStartPosition()
        {
            _startPosition = transform.position;
        }

        public void Push()
        {
            if (_isMoving)
            return;

            float distance = GetOpenDistance();
            _isMoving = true;

            transform
            .DOMove(_startPosition + Vector3.down * distance, _moveDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(ReturnToStart);
        }

        private float GetOpenDistance()
        {
            float height = _renderer.bounds.size.y;

            return _wall.IsBlocked ? height * _partialOpenPercent : height * _fullOpenPercent;
        }

        private void ReturnToStart()
        {
            transform
            .DOMove(_startPosition, _moveDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => _isMoving = false);
        }
    }
}