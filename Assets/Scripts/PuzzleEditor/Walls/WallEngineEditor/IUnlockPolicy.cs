namespace PuzzleEditor.Walls.WallEngineEditor
{
public interface IUnlockPolicy
{
    bool TryUnlock();

    public void Use();
}
}