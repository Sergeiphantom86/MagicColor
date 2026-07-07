namespace PuzzleEditor.Walls.WallEngineResource
{
    public interface IUnlockPolicy
    {
        bool TryUnlock();

        public void Use();
    }
}