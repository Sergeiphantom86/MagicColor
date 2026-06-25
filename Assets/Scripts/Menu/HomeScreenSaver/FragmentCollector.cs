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

        public event Action<List<Fragment>> OnPixelsRendered;

        private void Awake()
        {
            _textureInitializer = GetComponent<TextureInitializer>();
        }

        private void OnEnable()
        {
            if (_textureInitializer != null)
            {
                _textureInitializer.OnInitialize += Collect;
            }
        }

        private void OnDisable()
        {
            if (_textureInitializer != null)
            {
                _textureInitializer.OnInitialize -= Collect;
            }
        }

        private void Collect(int count)
        {
            OnPixelsRendered.Invoke(_textureInitializer.FragmentsList);
        }
    }
}