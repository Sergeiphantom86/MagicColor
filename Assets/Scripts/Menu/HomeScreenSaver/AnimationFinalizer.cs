using PuzzleEditor;
using UnityEngine;

namespace Menu.HomeScreenSaver
{
    [RequireComponent(typeof(Viewer), typeof(Agitator), typeof(TextureInitializer))]

    public class AnimationFinalizer : MonoBehaviour
    {
        private float _delay;
        private Viewer _viewer;
        private Agitator _animator;
        private TextureInitializer _textureInitializer;

        private void Awake()
        {
            _delay = 0.5f;
            _viewer = GetComponent<Viewer>();
            _animator = GetComponent<Agitator>();
            _textureInitializer = GetComponent<TextureInitializer>();
        }

        private void OnEnable()
        {
            _animator.Exploded += OnStartNewAnimation;
        }

        private void OnDisable()
        {
            _animator.Exploded -= OnStartNewAnimation;
        }

        private void OnStartNewAnimation()
        {
            _textureInitializer.ClearAllFragments();

            this.SafeDelayedCall(
            _delay,
            () =>
            {
                if (_viewer != null && isActiveAndEnabled)
                {
                    _viewer.ShowNextSprite();
                }
            }
            );
        }
    }
}