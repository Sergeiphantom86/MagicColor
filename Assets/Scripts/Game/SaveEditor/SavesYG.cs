using PuzzleEditor.RouletteEditor;
using PuzzleEditor.SoundEditor;
using UnityEngine;

namespace YG
{
    public partial class SavesYG
    {
        private const long MinCurrentValue = 0;
        private const int IndexUnblockingTutorial = 4;
        private const int IndexAbilityTutorial = 7;
        private const float StartingVolumeValue = 0.3f;

        [SerializeField] private int _indexUnblockingTutorial = IndexUnblockingTutorial;

        [field: SerializeField] public int Spins { get; set; }
        [field: SerializeField] public int Stars { get; set; }
        [field: SerializeField] public int QuestIndex { get; set; }
        [field: SerializeField] public long CurrentCoin { get; set; }
        [field: SerializeField] public long CurrentCrystal { get; set; }
        [field: SerializeField] public int QuantityAbilities { get; set; }
        [field: SerializeField] public int MaxReachedQuestIndex { get; set; }
        [field: SerializeField] public string CurrentLanguage { get; set; } = "ru";
        [field: SerializeField] public bool IsTutorialBasics { get; set; }
        [field: SerializeField] public bool IsUnblockingTutorial { get; set; }
        [field: SerializeField] public bool IsAbilityTutorial { get; set; }
        [field: SerializeField] public bool IsMenuTutorial { get; set; }
        [field: SerializeField] public bool IsTransparency { get; set; }
        [field: SerializeField] public bool IsUnlockKey { get; set; }
        [field: SerializeField] public bool IsUnlockAbilities { get; set; }
        [field: SerializeField] public bool IsAutomaticallyNewLevel { get; set; }

        public int CountQuest { get; set; }
        public int Reward { get; set; }

        private SpriteStorage _spriteStorage;
        private VolumeStorage _volumeStorage;

        public Sprite New => _spriteStorage?.New;
        public Sprite Current => _spriteStorage?.Current;
        public float MusicVolume => _volumeStorage?.MusicVolume ?? StartingVolumeValue;
        public float SoundVolume => _volumeStorage?.SoundVolume ?? StartingVolumeValue;

        public float MusicTime { get; set; }

        public int IndexSecondQuest => _indexUnblockingTutorial;
        public int ObstacleDeactivatIndex => IndexAbilityTutorial;

        public void SetCurrency(Currency currency, long balance)
        {
            if (currency == null)
            {
                Debug.LogError("SetCurrency: null currency");
                return;
            }

            if (balance == 0)
                return;

            if (currency is Coin)
            {
                CurrentCoin += TryGetBalance(balance);
            }
            else if (currency is Crystal)
            {
                CurrentCrystal += TryGetBalance(balance);
            }
        }

        private long TryGetBalance(long balance)
        {
            if (balance < MinCurrentValue)
            {
                Debug.LogWarning($"Balance {balance} -> {MinCurrentValue} (below min)");
                return MinCurrentValue;
            }

            return balance;
        }

        public void SetVolume(VolumeChanger volumeChanger, float volume)
        {
            InitializeVolumeStorage();
            _volumeStorage.SetVolume(volumeChanger, volume);
        }

        public void SetNew(Sprite sprite)
        {
            InitializeSpriteStorage();
            _spriteStorage.SetNew(sprite);
        }

        public void SetCurrent(Sprite sprite)
        {
            InitializeSpriteStorage();
            _spriteStorage.SetCurrent(sprite);
        }

        private void InitializeSpriteStorage()
        {
            _spriteStorage ??= new SpriteStorage();
        }

        private void InitializeVolumeStorage()
        {
            _volumeStorage ??= new VolumeStorage();
        }

        private void OnDefaultSaves()
        {
            Spins = 0;
            Stars = 0;
            Reward = 0;
            QuestIndex = 0;
            CurrentCoin = 0;
            CurrentCrystal = 0;
            QuantityAbilities = 0;
            MaxReachedQuestIndex = 0;
            CurrentLanguage = "ru";
            IsTutorialBasics = false;
            IsUnblockingTutorial = false;
            IsAbilityTutorial = false;
            IsMenuTutorial = false;
            IsTransparency = false;
            IsUnlockKey = false;
            IsUnlockAbilities = false;
            IsAutomaticallyNewLevel = false;
            CountQuest = 0;
        }
    }
}