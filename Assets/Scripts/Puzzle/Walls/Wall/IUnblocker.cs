public interface IUnblocker
{
    public void Unblock();
    public bool IsBlocked {  get; }
}