using PuzzleEditor.RouletteEditor;
using PuzzleEditor.SoundEditor;
using UnityEngine;

namespace YG
{
    public partial class SavesYG 
    {
        private const int MinIndexValue = 0;
        private const long MinCurrentValue = 0;
        private const int IndexUnblockingTutorial = 4;
        private const int IndexAbilityTutorial = 7;

        [SerializeField] private int _indexUnblockingTutorial = IndexUnblockingTutorial;

        [SerializeField] private int _questIndex;

        [SerializeField] private int _maxReachedQuestIndex;

        [SerializeField] private int _quantityAbilities;

        [SerializeField] private long _currentCoin;

        [SerializeField] private string _currentLanguage = "ru";

        [SerializeField] private long _currentCrystal;

        [SerializeField] private bool _isTutorialBasics;

        [SerializeField] private bool _isUnblockingTutorial;

        [SerializeField] private bool _isAbilityTutorial;

        [SerializeField] private bool _isUnlockAbilities;

        [SerializeField] private bool _isMenuTutorial;

        [SerializeField] private bool _isTransparency;

        [SerializeField] private bool _isUnlockKey;

        [SerializeField] private int _stars;

        [SerializeField] private float _musicVolume = 0.3f;

        [SerializeField] private float _soundVolume = 0.3f;

        [SerializeField] private int _spins;

        private int _countQuest;
        private int _reward;

        public int MaxReachedQuestIndex => _maxReachedQuestIndex;

        public bool IsTutorialBasics => _isTutorialBasics;

        public bool IsUnblockingTutorial => _isUnblockingTutorial;

        public bool IsAbilityTutorial => _isAbilityTutorial;

        public bool IsMenuTutorial => _isMenuTutorial;

        public bool IsUnlockAbilities => _isUnlockAbilities;

        public string CurrentLanguage => _currentLanguage;

        public int IndexSecondQuest => _indexUnblockingTutorial;

        public int ObstacleDeactivatIndex => IndexAbilityTutorial;

        public int QuantityAbilities => _quantityAbilities;

        public long CurrentCrystal => _currentCrystal;

        public bool IsTransparency => _isTransparency;

        public float MusicVolume => _musicVolume;

        public float SoundVolume => _soundVolume;

        public float MusicTime { get; private set; }

        public int IndexPuzzle { get; set; }

        public long CurrentCoin => _currentCoin;

        public bool IsUnlockKey => _isUnlockKey;

        public int QuestIndex => _questIndex;

        public int CountStars => _stars;

        public int Reward => _reward;

        public int Spins => _spins;

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

        public void SaveSpinsCount(int spins)
        {
            _spins = spins;
        }

        public void SetQuantityAbilities(int quantityAbilities)
        {
            _quantityAbilities = quantityAbilities;
        }

        public void SetTutorial(int index)
        {
            if (index < MinIndexValue)
            {
                Debug.LogWarning($"SetTutorial: ������� ������������� ������: {index}");
                return;
            }

            if (index >= _indexUnblockingTutorial)
            {
                _isUnlockKey = true;
            }

            if (index >= IndexAbilityTutorial)
            {
                ObstacleSwitch(true);
            }
        }

        public void ObstacleSwitch(bool isOn)
        {
            _isUnlockAbilities = isOn;
        }

        public void DisableTutorialMenu()
        {
            _isMenuTutorial = true;
        }

        public void ChangeTutorial(bool isTutorial)
        {
            if (_isTutorialBasics)
            {
                _isTutorialBasics = isTutorial;
                return;
            }

            _isUnblockingTutorial = isTutorial;
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
                Debug.LogError("SetCurrency: ������� null currency");
                return;
            }

            if (balance == 0) 
                return;

            if (currency is Coin)
            {
                _currentCoin += TryGetBalance(balance);
            }
            else if (currency is Crystal)
            {
                _currentCrystal += TryGetBalance(balance);
            }
        }

        public void SaveBalanceAfterPurchase(long balance)
        {
            _currentCoin = balance;
        }

        private long TryGetBalance(long balance)
        {
            if (balance < MinCurrentValue)
            {
                Debug.LogWarning($"SetCurrency: ������� ���������� ������������� ������ ��� Coin. �������: {_currentCoin}, ���������: {balance}");
                return MinCurrentValue;
            }

            return balance;
        }

        public void SetTutorialBasics()
        {
            _isTutorialBasics = true;
        }

        public void SetAbilityTutorial()
        {
            _isAbilityTutorial = true;
        }

        public void SetUnblockingTutorial()
        {
            _isUnblockingTutorial = true;
        }

        public void SetQuestIndex(int questIndex)
        {
            _questIndex = questIndex;
        }

        public void SetCurrentLanguage(string language)
        {
            _currentLanguage = language;
        }

        public void SetMaxReachedQuestIndex()
        {
            if (_questIndex >= _maxReachedQuestIndex)
            {
                _maxReachedQuestIndex++;
            }
        }

        public void SetCountStars(int count)
        {
            if (count < 0)
            {
                Debug.LogWarning($"SetCountStars: ���������� ���������� ����� ������������: {count}");
                _stars = MinIndexValue;
                return;
            }

            _stars = count;
        }

        public void SetVolume(VolumeChanger volumeChanger, float volume)
        {
            if (float.IsNaN(volume) || float.IsInfinity(volume))
            {
                Debug.LogError($"SetVolume: ������� ������������ volume: {volume}");
                return;
            }

            if (ValidateAllData(volumeChanger) && volumeChanger is MusicVolumeController)
                _musicVolume = volume;
            else if (ValidateAllData(volumeChanger) && volumeChanger is VolumeSoundsController)
                _soundVolume = volume;
        }

        private bool ValidateAllData(VolumeChanger volumeChanger)
        {
            if (volumeChanger == null)
            {
                Debug.LogError("GetVolume: ������� null VolumeChanger. ���������� �������� �� ���������: 0");
                return false;
            }

            return true;
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
            _quantityAbilities = 0;
            _maxReachedQuestIndex = 0;
            _currentLanguage = "ru";
            _isTutorialBasics = false;
            _isUnblockingTutorial = false;
            _isAbilityTutorial = false;
            _isMenuTutorial = false;
            _isTransparency = false;
            _isUnlockKey = false;
            _isUnlockAbilities = false;
            _indexUnblockingTutorial = IndexUnblockingTutorial;
        }
    }
}