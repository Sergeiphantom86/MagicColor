using UnityEngine;
using YG;

public class TransitionChooser : MonoBehaviour
{
    private const string Puzzle = nameof(Puzzle);
    private const string Tutorial = nameof(Tutorial);

    [SerializeField] private OfferPanel _offerPanel;

    private Quest _quest;
    private int _indexTransparent;

    private void Awake()
    {
        _indexTransparent = 2;
    }

    private void OnEnable()
    {
        _offerPanel.OnTutorial += TurnOnTutorial;
        _offerPanel.OnCancelled += SkipTutorial;
    }

    private void OnDisable()
    {
        _offerPanel.OnTutorial -= TurnOnTutorial;
        _offerPanel.OnCancelled -= SkipTutorial;
    }

    public void ChoosePuzzle(Quest quest)
    {
        SetQuest(quest);
        YG2.saves.SetTutorial(quest.Index);

        if (quest.Index == _indexTransparent)
        {
            YG2.saves.MakeTransparent(true);
        }

        if (quest.IsTutorial)
        {
            YG2.saves.SetSprite(_quest.Sprite);
            _offerPanel.gameObject.SetActive(true);
            return;
        }

        ConfigureTransition(Puzzle, _quest.Sprite);
    }

    private void SkipTutorial()
    {
        ConfigureTransition(Puzzle, _quest.Sprite);
    }

    private void TurnOnTutorial()
    {
        ConfigureTransition(Tutorial);
    }

    private void ConfigureTransition(string name, Sprite sprite = null)
    {
        YG2.saves.SetSprite(sprite);

        SceneLoader.Instance.LoadSceneWithSplash(name);
    }

    private void SetQuest(Quest quest)
    {
        if (quest == null)
        {
            Debug.LogError($"Quest = null {this}");
        }
        
        _quest = quest;
    }
}