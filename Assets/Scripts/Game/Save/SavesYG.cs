using UnityEngine;

namespace YG
{
    public partial class SavesYG
    {
        private const int MinIndexValue = 0;
        private const long MinCurrentValue = 0;
        private const int IndexSecondTutorial = 2;

        [SerializeField] private int _indexSecondQuest = IndexSecondTutorial;
        [SerializeField] private int _questIndex;
        [SerializeField] private int _maxReachedQuestIndex;
        [SerializeField] private long _currentCoin;
        [SerializeField] private long _currentCrystal;
        [SerializeField] private bool _isFirstTutorial = true;
        [SerializeField] private bool _isSecondTutorial = true;
        [SerializeField] private bool _isMenuTutorial;
        [SerializeField] private bool _isTransparency;
        [SerializeField] private bool _isTutorial;
        [SerializeField] private bool _isAutomaticallyNewLevel;
        [SerializeField] private int _stars;
        [SerializeField] private float _musicVolume = 0.3f;
        [SerializeField] private float _soundVolume = 0.3f;
        [SerializeField] private int _spins;

        private Sprite _newSprite;
        private Sprite _currentSprite;
        private int _countQuest;
        private int _reward;

        public bool IsAutomaticallyNewLevel => _isAutomaticallyNewLevel;
        public bool IsFirstTutorial => _isFirstTutorial;
        public bool IsSecondTutorial => _isSecondTutorial;
        public bool IsMenuTutorial => _isMenuTutorial;
        public int IndexSecondQuest => _indexSecondQuest;
        public long CurrentCrystal => _currentCrystal;
        public bool IsTransparency => _isTransparency;
        public float MusicVolume => _musicVolume;
        public float SoundVolume => _soundVolume;
        public float MusicTime { get; private set; }
        public long CurrentCoin => _currentCoin;
        public bool IsTutorial => _isTutorial;
        public int QuestIndex => _questIndex;
        public int CountStars => _stars;
        public int Reward => _reward;
        public int Spins => _spins;
        public Sprite NewSprite => _newSprite;
        public Sprite CurrentSprite => _currentSprite;

        public void SetCountQuest(int countQuest)
        {
            _countQuest = countQuest;
        }

        public void SetMusicTime(float time)
        {
            MusicTime = time;
        }

        public bool TryEnableFollowingQuest(int indexCurrentQuest)
        {
            return indexCurrentQuest >= _countQuest;
        }

        public void SetNewSprite(Sprite sprite)
        {
            _newSprite = sprite;
        }

        public void SetCurrentSprite(Sprite sprite)
        {
            _currentSprite = sprite;
        }

        public void SaveSpinsCount(int spins)
        {
            _spins = 0;
            
            _spins += spins;
        }

        public void SetTutorial(int index)
        {
            if (index < MinIndexValue)
            {
                Debug.LogWarning($"SetTutorial: передан отрицательный индекс: {index}");
                return;
            }

            if (index == _indexSecondQuest)
            {
                _isTutorial = true;
            }
        }

        public void DisableTutorialMenu()
        {
            _isMenuTutorial = true;
        }

        public void ChangeTutorial(bool isTutorial)
        {
            if (_isFirstTutorial)
            {
                _isFirstTutorial = isTutorial;
                return;
            }

            _isSecondTutorial = isTutorial;
        }

        public void SetReward(int reward)
        {
            if (reward > 0)
            {
                _reward = reward;
            }
        }

        public void MakeTransparent(bool isTransparency)
        {
            _isTransparency = isTransparency;
        }

        public void SetCurrency(Currency currency, long balance)
        {
            if (currency == null)
            {
                Debug.LogError("SetCurrency: передан null currency");
                return;
            }
            
            if (balance == 0) return;

            if (currency is Coin)
            {
                _currentCoin += TryGetBalance(balance);
            }
            else if (currency is Crystal)
            {
                _currentCrystal += TryGetBalance(balance);
            }
        }

        private long TryGetBalance(long balance)
        {
            if (balance < MinCurrentValue)
            {
                Debug.LogWarning($"SetCurrency: попытка установить отрицательный баланс для Coin. Текущий: {_currentCoin}, изменение: {balance}");
                return MinCurrentValue;
            }

            return balance;
        }

        public void SetQuestIndex(int questIndex)
        {
            _questIndex = questIndex;
        }

        public int SetIndexExit()
        {
            if (_questIndex > _maxReachedQuestIndex)
            {
                _maxReachedQuestIndex = _questIndex;
            }

            if (_questIndex == _maxReachedQuestIndex)
            {
                _questIndex -= 1;
            }

            return _maxReachedQuestIndex;
        }

        public void SetCountStars(int count)
        {
            if (count < 0)
            {
                Debug.LogWarning($"SetCountStars: переданное количество звезд отрицательно: {count}");
                _stars = MinIndexValue;
                return;
            }

            _stars = count;
        }

        public void SetVolume(VolumeChanger volumeChanger, float volume)
        {
            if (float.IsNaN(volume) || float.IsInfinity(volume))
            {
                Debug.LogError($"SetVolume: передан некорректный volume: {volume}");
                return;
            }

            if (ValidateAllData(volumeChanger) && volumeChanger is MusicVolumeController)
                _musicVolume = volume;
            else if (ValidateAllData(volumeChanger) &&volumeChanger is VolumeSoundsController)
                _soundVolume = volume;
        }

        private bool ValidateAllData(VolumeChanger volumeChanger)
        {
            if (volumeChanger == null)
            {
                Debug.LogError("GetVolume: передан null VolumeChanger. Возвращено значение по умолчанию: 0");
                return false;
            }

            return true;
        }

        public void SetAutomaticTransition(bool isAutomaticallyNewLevel)
        {
            _isAutomaticallyNewLevel = isAutomaticallyNewLevel;
        }

        private void OnDefaultSaves()
        {
            _spins = 0;
            _stars = 0;
            _reward = 0;
            _questIndex = 0;
            _currentCoin = 0;
            _currentCrystal = 0;
            _soundVolume = 0.3f;
            _musicVolume = 0.3f;
            _maxReachedQuestIndex = 0;
            _isFirstTutorial = true;
            _isSecondTutorial = true;
            _isMenuTutorial = false;
            _isTransparency = false;
            _isTutorial = false;
            _indexSecondQuest = IndexSecondTutorial;
        }
    }
}