using Game.LoadingScreen;
using Menu.TutorialEditor;
using UnityEngine;

namespace Menu.QuestEditor
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
            _offerPanel.Consent += LoadTutorial;
            _offerPanelMobile.Consent += LoadTutorial;

            _offerPanel.Cancelled += LoadPuzzle;
            _offerPanelMobile.Cancelled += LoadPuzzle;
        }

        private void OnDisable()
        {
            _offerPanel.Consent -= LoadTutorial;
            _offerPanelMobile.Consent -= LoadTutorial;

            _offerPanel.Cancelled -= LoadPuzzle;
            _offerPanelMobile.Cancelled -= LoadPuzzle;
        }

        public void Initialize(ZoomChanger zoomChanger)
        {
            _transitionService = new QuestTransitionService(zoomChanger);
        }

        public void ChoosePuzzle(Quest quest, bool isAutomaticTransition)
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

            LoadScene(result.SceneName, isAutomaticTransition);
        }

        private void LoadPuzzle() => 
            LoadScene(Puzzle);

        private void LoadTutorial() => 
            LoadScene(Tutorial);

        private void LoadScene(string sceneName, bool isAutomaticTransition = false)
        {
            float extraTime = 0;

            if (isAutomaticTransition)
            {
                extraTime = 0.2f;
            }

            _transitionService.SaveSprite(_cachedSprite);

            SceneLoader.Instance.LoadSceneAsyncWithSplash(sceneName, extraTime);
        }
    }
}