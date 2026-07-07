using UnityEngine;

namespace Game.SaveEditor
{
    public class SpriteStorage
    {
        private Sprite _new;
        private Sprite _current;

        public Sprite New => _new;
        public Sprite Current => _current;

        public void SetNew(Sprite sprite)
        {
            if (sprite == null)
            {
                Debug.LogWarning("SpriteStorage: null sprite assignment ignored");
                return;
            }

            if (_new == sprite) return;

            _new = sprite;
        }

        public void SetCurrent(Sprite sprite)
        {
            if (sprite == null)
            {
                Debug.LogWarning("SpriteStorage: attempt to set a null sprite");
                return;
            }

            if (_current == sprite) return;

            _current = sprite;
        }
    }
}