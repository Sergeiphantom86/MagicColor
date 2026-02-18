public interface ILockFeedbackService
{
    public void InitializComponents(Lock @lock, HintKey hint);

    void Play();
}