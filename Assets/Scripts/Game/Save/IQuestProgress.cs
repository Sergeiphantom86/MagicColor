public interface IQuestProgress
{
    public void SetQuestIndex(int questIndex);
    public void SetIndexExit();
    public bool TryEnableFollowingQuest(int indexCurrentQuest);
    public void SetCountQuest(int countQuest);
}