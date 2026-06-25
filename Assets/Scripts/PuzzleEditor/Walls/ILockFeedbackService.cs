public interface ILockFeedbackService
{
    public void InitializComponents(Lock @lock, Messager hint);

    void Play();
}