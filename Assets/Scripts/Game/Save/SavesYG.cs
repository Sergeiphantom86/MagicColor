using UnityEngine;

namespace YG
{
    public partial class SavesYG
    {
        private Sprite _sprite;
        private bool _isSimilar;
        private bool _isTutorial;
        private int _indexUnblocking;
        private int _currentValue;
        private bool _isTransparency;
        private static bool _isFirstTutorial;
        private static bool _isSecondTutorial;
        private static int _questIndex;
        private static long _currentCoin;
        private static long _currentCrystal;

        public SavesYG()
        {
            _indexUnblocking = 7;
            _questIndex = 0;
            _currentCoin = 0;
            _currentCrystal = 0;
            _isFirstTutorial = true;
            _isSecondTutorial = true;
            _isTransparency = false;
            _isSimilar = false;
            _isTutorial = false;
            CountStars = 0;
            MusicVolume = 0.8f;
            SoundVolume = 0.8f;
            MusicPlaybackTime = 0;
        }

        public float MusicVolume { get; private set; }

        public float SoundVolume { get; private set; }

        public int CountStars { get; private set; }

        public int QuestIndex => _questIndex;

        public float MusicPlaybackTime { get; private set; }

        public long CurrentCoin => _currentCoin;

        public long CurrentCrystal => _currentCrystal;

        public Sprite Sprite => _sprite;

        public bool IsTutorial => _isTutorial;

        public bool IsFirstTutorial => _isFirstTutorial;

        public bool IsSecondTutorial => _isSecondTutorial;

        public bool IsTransparency => _isTransparency;

        public bool IsSimilar => _isSimilar;

        public int CurrentValue => _currentValue;

        public int IndexUnblocking => _indexUnblocking;

        public void SetTutorial(int index)
        {
            if (index == _indexUnblocking)
            {
                _isTutorial = true;
            }
        }

        public void SetTutorial(bool isTutorial)
        {
            if (_isFirstTutorial)
            {
                _isFirstTutorial = isTutorial;
                return;
            }

            _isSecondTutorial = isTutorial;
        }

        public void SetCurrentValue(int value)
        {
            _currentValue = value;
        }

        public void MakeTransparent(bool isTransparency)
        {
            _isTransparency = isTransparency;
        }

        public void SetSimilarity(bool isSimilar)
        {
            _isSimilar = isSimilar;
        }

        public void SetMusicPlaybackTime(float time)
        {
            MusicPlaybackTime = time;
        }

        public void SetCurrency(Wallet wallet, long amount)
        {
            if (amount <= 0) return;

            if (wallet is CoinWallet)
                _currentCoin += amount;
            else if (wallet is CrystalWallet)
                _currentCrystal += amount;
        }

        public void SetQuestIndex(int questInex)
        {
            _questIndex = questInex;
        }

        public void SetCountStars(int count)
        {
            if (count < 0) return;

            CountStars = count;
        }

        public void SetVolume(VolumeChanger volumeChanger, float volume)
        {
            if (volumeChanger is MusicVolumeController)
                MusicVolume = volume;
            else
                SoundVolume = volume;
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