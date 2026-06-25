using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PuzzleEditor.MovingBlocks.BlockEditor;
using PuzzleEditor.Walls;
using UnityEngine;

namespace PuzzleEditor
{
    public class Repainter : MonoBehaviour
    {
        [SerializeField]
        private PuzzlesIdentifier _puzzlesIdentifier;

        [SerializeField]
        private BlocksContainer _blocksContainer;

        private List<Color> _colors;
        private List<IColorable> _walls;
        private List<IColorable> _blocks;
        private IBlocksContainer _iBlocksContainer;
        private TextureInitializer _textureInitializer;
        private WaitForSeconds _waitForSeconds;
        private float _delay;

        public event Action<List<IColorable>> OnRecoloredWalls;

        public event Action<List<IColorable>> OnRecoloredBlock;

        private void Awake()
        {
            _delay = 0.005f;
            _colors = new List<Color>();
            _walls = new List<IColorable>();
            _blocks = new List<IColorable>();
            _waitForSeconds = new WaitForSeconds(_delay);
            _iBlocksContainer = _blocksContainer;
            _textureInitializer = GetComponent<TextureInitializer>();
        }

        private void OnEnable()
        {
            _textureInitializer.CanPaint += UpdateSystem;
        }

        private void OnDisable()
        {
            _textureInitializer.CanPaint -= UpdateSystem;
        }

        private List<IColorable> GetColorablesFromContainer(Transform container)
        {
            var list = new List<IColorable>();

            if (container == null)
            {
                Debug.LogWarning($"��������� {container.name} ������!", this);
                return list;
            }

            return GetColorables(container, list);
        }

        private List<IColorable> GetColorables(Transform container, List<IColorable> colorables)
        {
            foreach (Transform child in container)
            {
                if (child.TryGetComponent(out IColorable colorable))
                {
                    colorables.Add(colorable);
                }
            }

            return colorables;
        }

        private void UpdateSystem(List<Color> colors)
        {
            StartCoroutine(Wait(colors));
        }

        private void UpdateColors(List<Color> colors)
        {
            _colors.AddRange(colors);

            if (_colors.Count == 0)
            {
                Debug.LogWarning("� Color Analyzer ��� ��������� ������!", this);
            }
        }

        private void ReplaceColors(List<IColorable> colorables)
        {
            if (ShouldRepaint(colorables) == false)
                return;

            var (colors, objects) = PreparePaintingData(colorables);

            ExecutePainting(colors, objects);
        }

        private bool ShouldRepaint(List<IColorable> colorables)
        {
            return colorables.Count > 0 && _colors.Count > 0;
        }

        private (List<Color> Colors, List<IColorable> Objects) PreparePaintingData(
            List<IColorable> colorables
        )
        {
            return (
                Colors: ShuffleColors(_colors),
                Objects: SelectRandomColorables(colorables, _colors.Count)
            );
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

                if (colorables[i] is Block block)
                {
                    block.Subscribe();
                }
            }
        }

        private IEnumerator Wait(List<Color> colors)
        {
            yield return _waitForSeconds;

            _walls = GetColorablesFromContainer(_puzzlesIdentifier.CurrentContainer.transform);

            _blocks = GetColorablesFromContainer(_iBlocksContainer.Transform);

            if (_blocks.Count == 0)
            {
                Debug.LogError("AssignOriginal");
            }

            UpdateColors(colors);

            ReplaceColors(_walls);
            OnRecoloredWalls?.Invoke(_walls);

            ReplaceColors(_blocks);
            OnRecoloredBlock?.Invoke(_blocks);
        }
    }
}