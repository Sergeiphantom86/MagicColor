using Menu.Tutorials;
using UnityEngine;
using YG;

namespace Menu.GameLogicQuests
{
    public class TransitionChooser : MonoBehaviour
    {
        private const string Puzzle = nameof(Puzzle);
        private const string Tutorial = nameof(Tutorial);

        [SerializeField] private OfferPanel _offerPanel;
        [SerializeField] private OfferPanel _offerPanelMobile;

        private IQuestTransitionService _transitionService;
        private Sprite _cachedSprite;

        private void OnEnable()
        {
            _offerPanel.Consented += OnLoadTutorial;
            _offerPanelMobile.Consented += OnLoadTutorial;

            _offerPanel.Cancelled += OnLoadPuzzle;
            _offerPanelMobile.Cancelled += OnLoadPuzzle;
        }

        private void OnDisable()
        {
            _offerPanel.Consented -= OnLoadTutorial;
            _offerPanelMobile.Consented -= OnLoadTutorial;

            _offerPanel.Cancelled -= OnLoadPuzzle;
            _offerPanelMobile.Cancelled -= OnLoadPuzzle;
        }

        public void Initialize(ZoomChanger zoomChanger)
        {
            _transitionService = new QuestTransitionService(zoomChanger);
        }

        public void ChoosePuzzle(Quest quest)
        {
            _cachedSprite = quest.Sprite;

            var result = _transitionService.ProcessQuest(quest);

            if (result.ShowOffer)
            {
                if (result.UseMobilePanel)
                    _offerPanelMobile.TurnOn();
                else
                    _offerPanel.TurnOn();

                return;
            }

            LoadScene(result.SceneName);
        }

        private void LoadScene(string sceneName)
        {
            _transitionService.SaveSprite(_cachedSprite);

            if (YG2.saves.SceneLoader == null)
            {
                Debug.LogError("SceneLoader instance not found! Using default load.");
                return;
            }

            YG2.saves.SceneLoader.LoadSceneAsyncWithSplash(sceneName);
        }

        private void OnLoadPuzzle() =>
           LoadScene(Puzzle);

        private void OnLoadTutorial() =>
            LoadScene(Tutorial);
    }
}