using UnityEngine;

namespace YG
{
    public partial class SavesYG
    {
        private Sprite _sprite;
        private static long _currentCoin;
        private static long _currentCrystal;
        private static int _questIndex;
        private bool _complete;
        private bool _isSimilar;

        public SavesYG()
        {
            MusicVolume = 0.8f;
            SoundVolume = 0.8f;
            CountStars = 0;
        }

        public float MusicVolume { get; private set; }

        public float SoundVolume { get; private set; }

        public int CountStars { get; private set; }

        public int QuestIndex => _questIndex;

        public long CurrentCoin => _currentCoin;

        public long CurrentCrystal => _currentCrystal;

        public Sprite Sprite => _sprite;

        public bool Complete => _complete;
        public bool IsSimilar => _isSimilar;

        public void SetAssembledPuzzle(bool complete)
        {
            _complete = complete;
        }

        public void SetSimilarity(bool isSimilar)
        {
            _isSimilar = isSimilar;
        }

        public void SetCurrentCoin(long amount)
        {
            if (amount < 0) return;

            _currentCoin += amount;
        }

        public void SetCurrentCrystal(long amount)
        {
            if (amount < 0) return;

            _currentCrystal += amount;
        }

        public void SetQuestIndex(int questInex)
        {
            if (questInex < 0) return;

            _questIndex = questInex;
        }

        public void SetCountStars(int count)
        {
            if (count < 0) return;

            CountStars = count;
        }

        public void SetVolume(string name, float volume)
        {
            if (nameof(MusicVolume) == name)
            {
                MusicVolume = volume;
            }
            else
            {
                SoundVolume = volume;
            }
        }

        public void ResetSprite()
        {
            _sprite = null;
        }

        public void SetSprite(Sprite sprite)
        {
            if (sprite == null) return;
            if (_sprite != null) return;

            _sprite = sprite;
        }
    }
}