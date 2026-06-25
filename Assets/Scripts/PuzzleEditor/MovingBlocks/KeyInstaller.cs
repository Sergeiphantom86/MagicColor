using System.Collections.Generic;
using System.Linq;
using Menu.TutorialEditor;
using PuzzleEditor.MovingBlocks.BlockEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PuzzleEditor.MovingBlocks
{
    public class KeyInstaller : MonoBehaviour
    {
        [SerializeField]
        private int _desiredBlockCount = 3;

        private IActivatable _activatable;
        private bool _isPlaced;
        private bool _isTutorial;
        private Key _key;

        public void Initialized(Key key, bool isTutorial)
        {
            _key = key;
            _activatable = key;
            _isTutorial = isTutorial;

            if (_isTutorial == false)
            {
                _activatable.Deactivate();
            }
        }

        public void TryPlaceKey(List<IColorable> colorables)
        {
            if (CanPlace() == false)
                return;

            var blocks = colorables
                .OfType<Block>()
                .Where(block => block.IsRepainted == false)
                .OrderBy(_ => Random.value)
                .Take(_desiredBlockCount)
                .ToList();

            if (blocks.Count == 0)
                return;

            Block block = blocks[Random.Range(0, blocks.Count)];

            if (SceneManager.GetActiveScene().name == "Tutorial")
            {
                block.SetOutlineColor();
            }

            _key.transform.position = block.transform.position;

            _isPlaced = true;
        }

        private bool CanPlace()
        {
            return _isTutorial && _key != null && _isPlaced == false;
        }
    }
}