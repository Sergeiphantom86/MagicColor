namespace Game.SaveEditor
{
    public interface IQuestProgress
    {
        public void SetQuestIndex(int questIndex);

        public void SetMaxReachedQuestIndex();

        public bool TryEnableFollowingQuest(int indexCurrentQuest);

        public void SetCountQuest(int countQuest);
    }
}