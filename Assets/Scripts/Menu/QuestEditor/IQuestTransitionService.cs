using UnityEngine;

namespace Menu.QuestEditor
{
    public interface IQuestTransitionService
    {
        public TransitionResult ProcessQuest(Quest quest);

        public void SaveSprite(Sprite sprite);
    }
}