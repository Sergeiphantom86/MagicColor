namespace PuzzleEditor.Walls.WallResources
{
    public interface IUnblocker
    {
        public bool IsBlocked { get; }
        public void Unblock();
    }
}