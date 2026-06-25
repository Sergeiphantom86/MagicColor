using UnityEngine;

public interface IQuestTransitionService
{
    public TransitionResult ProcessQuest(Quest quest);

    public void SaveSprite(Sprite sprite);
}