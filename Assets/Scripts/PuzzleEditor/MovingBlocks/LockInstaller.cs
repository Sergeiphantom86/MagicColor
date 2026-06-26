using System.Collections.Generic;
using System.Linq;
using PuzzleEditor.LockEditor;
using PuzzleEditor.Walls.WallEditor;
using UnityEngine;

namespace PuzzleEditor.MovingBlocks
{
    public class LockInstaller : MonoBehaviour
    {
        [SerializeField] private int _additionalTurnY = 90;
        [SerializeField] private int _additionalTurnZ = 90;

        private Lock _lock;
        private bool _isPlaced;
        private bool _isTutorial;

        public void Initialized(Lock @lock, bool isTutorial)
        {
            _lock = @lock;
            _isTutorial = isTutorial;

            if (_isTutorial == false)
            {
                _lock.gameObject.SetActive(false);
            }
        }

        public void TryPlaceLock(List<IColorable> colorables)
        {
            if (CanPlace() == false)
            return;

            Wall wall = colorables
            .OfType<Wall>()
            .FirstOrDefault(wall => wall.IsRepainted && wall.CenterFence != null);

            if (wall == null)
            return;

            PlaceOnWall(wall);
        }

        private void PlaceOnWall(Wall wall)
        {
            _lock.transform.position = wall.CenterFence.position;

            AdjustLockRotation(wall);

            wall.Block();

            _isPlaced = true;

            _lock.SetUsed(_isPlaced);
        }

        private void AdjustLockRotation(Wall wall)
        {
            Quaternion wallRotation = Quaternion.Euler(0f, wall.GetAngleY(), 0f);

            Quaternion perpendicularRotation = GetPerpendicularRotation(wallRotation);

            _lock.SetAngle(perpendicularRotation.eulerAngles);
        }

        private Quaternion GetPerpendicularRotation(Quaternion wallRotation)
        {
            return wallRotation * Quaternion.Euler(0, _additionalTurnY, _additionalTurnZ);
        }

        private bool CanPlace()
        {
            return _isTutorial && _lock != null && _isPlaced == false && _lock.IsUsed == false;
        }
    }
}