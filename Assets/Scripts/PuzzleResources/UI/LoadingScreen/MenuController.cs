using System.Collections;
using Fireworks;
using UnityEngine;

namespace PuzzleResources.UI.LoadingScreen
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private MenuButtons _menuButtons;
        [SerializeField] private PanelFader _panelFader;
        [SerializeField] private FireworksController _fireworks;

        private SceneFlowController _sceneFlow;
        private PuzzleFlowController _puzzleFlow;

        private void Awake()
        {
            _sceneFlow = GetComponent<SceneFlowController>();
            _puzzleFlow = GetComponent<PuzzleFlowController>();

            if (_menuButtons == null)
            {
                Debug.LogError("MenuButtons == null");
            }

            if (_panelFader == null)
            {
                Debug.LogError("PanelFader == null");
            }

            if (_fireworks == null)
            {
                Debug.LogError("FireworksController == null");
            }

            if (_sceneFlow == null)
            {
                Debug.LogError("SceneFlowController == null");
            }

            if (_puzzleFlow == null)
            {
                Debug.LogError("PuzzleFlowController == null");
            }

            _menuButtons.Initialize(OnAnyButton, OnResumeClicked);
        }

        private IEnumerator Start()
        {
            yield return new WaitForFixedUpdate();

            _panelFader.FadeOut();
        }

        private void OnEnable()
        {
            if (_puzzleFlow == null)
                return;

            _puzzleFlow.PuzzleCompleted += OnPuzzleCompleted;
        }

        private void OnDisable()
        {
            if (_puzzleFlow == null)
                return;

            _puzzleFlow.PuzzleCompleted -= OnPuzzleCompleted;
        }

        private void OnAnyButton() { }

        private void OnPuzzleCompleted()
        {
            _menuButtons.ShowResumeButton();
        }

        private void OnResumeClicked()
        {
            _fireworks.Stop();

            _panelFader.FadeIn(() =>
            {
                _sceneFlow.LoadNext();
            });
        }
    }
}