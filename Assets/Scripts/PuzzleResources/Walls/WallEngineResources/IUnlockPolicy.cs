namespace PuzzleResources.Walls.WallEngineResource
{
    public interface IUnlockPolicy
    {
        bool TryUnlock();

        public void Use();
    }
}