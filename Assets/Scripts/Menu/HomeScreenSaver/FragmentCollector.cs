using System;
using System.Collections.Generic;
using PuzzleEditor;
using UnityEngine;

namespace Menu.HomeScreenSaver
{
    [RequireComponent(typeof(TextureInitializer))]
    public class FragmentCollector : MonoBehaviour
    {
        private TextureInitializer _textureInitializer;

        public event Action<List<Fragment>> PixelsRendered;

        private void Awake()
        {
            _textureInitializer = GetComponent<TextureInitializer>();
        }

        private void OnEnable()
        {
            if (_textureInitializer != null)
            {
                _textureInitializer.Initialize += OnCollect;
            }
        }

        private void OnDisable()
        {
            if (_textureInitializer != null)
            {
                _textureInitializer.Initialize -= OnCollect;
            }
        }

        private void OnCollect(int count)
        {
            PixelsRendered.Invoke(_textureInitializer.FragmentsList);
        }
    }
}