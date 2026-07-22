namespace PuzzleResources.Walls.WallEngineResources
{
    public interface IUnlockPolicy
    {
        bool TryUnlock();

        public void Use();
    }
}