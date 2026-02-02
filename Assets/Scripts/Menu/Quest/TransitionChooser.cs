using UnityEngine;
using YG;

public class TransitionChooser : MonoBehaviour
{
    private const string Puzzle = nameof(Puzzle);
    private const string Tutorial = nameof(Tutorial);

    [SerializeField] private OfferPanel _offerPanel;
    [SerializeField] private OfferPanel _offerPanelmobile;

    private int _indexTransparent;
    private Sprite _sprite;
    private ZoomChanger _zoomChanger;

    private void Awake()
    {
        _indexTransparent = 2;
        _zoomChanger = new ZoomChanger();
    }

    private void OnEnable()
    {
        _offerPanel.OnConsent += TurnOnTutorial;
        _offerPanelmobile.OnConsent += TurnOnTutorial;

        _offerPanel.OnCancelled += SkipTutorial;
        _offerPanelmobile.OnCancelled += SkipTutorial;
    }

    private void OnDisable()
    {
        _offerPanel.OnConsent -= TurnOnTutorial;
        _offerPanelmobile.OnConsent -= TurnOnTutorial;

        _offerPanel.OnCancelled -= SkipTutorial;
        _offerPanelmobile.OnCancelled -= SkipTutorial;
    }

    public void ChoosePuzzle(Quest quest)
    {
        _sprite = quest.Sprite;

        YG2.saves.SetTutorial(quest.Index);

        if (quest.Index == _indexTransparent)
        {
            YG2.saves.MakeTransparent(true);
        }

        if (quest.IsTutorial)
        {
            YG2.saves.ChangeTutorial(false);
            quest.SetTutorial(false);

            if (_zoomChanger.IsMobileWithTallScreen())
            {
                _offerPanelmobile.TurnOn();
            }
            else
            {
                _offerPanel.TurnOn();
            }

            return;
        }

        ConfigureTransition(Puzzle);
    }

    private void SkipTutorial()
    {
        ConfigureTransition(Puzzle);
    }

    private void TurnOnTutorial()
    {
        ConfigureTransition(Tutorial);
    }

    private void ConfigureTransition(string name)
    {
        if (_sprite != null)
        {
            YG2.saves.SetCurrentSprite(_sprite);
        }
        else
        {
            Debug.LogWarning($"Cached sprite is null when transitioning to {name}");
        }

        SceneLoader.Instance.LoadSceneWithSplash(name);
    }
}