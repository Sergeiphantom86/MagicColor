public class WallBlockProcessor
{
    private readonly Wall _wall;

    public WallBlockProcessor(Wall wall)
    {
        _wall = wall;
    }

    public void UnblockWall()
    {
        _wall.Unblock();
    }
}