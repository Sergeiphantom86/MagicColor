using System;
using System.Collections.Generic;
using PuzzleResources;
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
                _textureInitializer.Initialized += OnCollect;
            }
        }

        private void OnDisable()
        {
            if (_textureInitializer != null)
            {
                _textureInitializer.Initialized -= OnCollect;
            }
        }

        private void OnCollect()
        {
            PixelsRendered.Invoke(_textureInitializer.FragmentsList);
        }
    }
}