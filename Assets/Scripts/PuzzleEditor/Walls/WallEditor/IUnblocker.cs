namespace PuzzleEditor.Walls.WallEditor
{
public interface IUnblocker
{
    public bool IsBlocked { get; }

    public void Unblock();
}
}