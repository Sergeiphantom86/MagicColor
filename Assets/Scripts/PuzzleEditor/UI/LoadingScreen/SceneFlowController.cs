using Menu.TutorialEditor;
using PuzzleEditor.RouletteEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

namespace PuzzleEditor.UI.LoadingScreen
{
    public class SceneFlowController : MonoBehaviour
    {
        private const string Puzzle = nameof(Puzzle);
        private const string Roulette = nameof(Roulette);
        private const string Tutorial = nameof(Tutorial);

        [SerializeField] private MenuLoader _menuLoader;
        [SerializeField] private TextureInitializer _textureInitializer;
        [SerializeField] private TutorialPuzzle1 _tutorialPuzzle;

        private string _sceneName;
        private AdRewardController _adRewardController;

        private void Awake()
        {
            _sceneName = SceneManager.GetActiveScene().name;
            _adRewardController = GetComponent<AdRewardController>();

            if (_textureInitializer == null)
                Debug.LogError($"[SceneFlowController] TextureInitializer is missing on object '{gameObject.name}'", this);

            if (_menuLoader == null)
                Debug.LogError($"[SceneFlowController] MenuLoader is missing on object '{gameObject.name}'", this);

            if (_adRewardController == null)
                Debug.LogWarning($"[SceneFlowController] AdRewardController not found on object '{gameObject.name}'", this);
        }

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            Sprite sprite = TryGetSprite(YG2.saves.Current);

            if (sprite == null)
            {
                Debug.LogError($"Sprite is null on object '{gameObject.name}'", this);
                return;
            }

            if (sprite.texture == null)
            {
                Debug.LogError($"Sprite.texture is null on object '{gameObject.name}'", this);
                return;
            }

            _textureInitializer.SpawnPixelsFromTexture(sprite.texture);
        }

        public void LoadNext()
        {
            if (_sceneName != Tutorial)
            {
                _adRewardController.ShowRewardAd(LoadRoulette);
                YG2.SaveProgress();

                return;
            }

            _menuLoader.TargetScene(Puzzle);
        }

        private void LoadRoulette()
        {
            _menuLoader.TargetScene(Roulette);
        }

        private Sprite TryGetSprite(Sprite sprite)
        {
            return _tutorialPuzzle != null 
                ? _tutorialPuzzle.Sprite 
                : sprite;
        }
    }
}