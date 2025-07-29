public class SimpleTeleporter : Teleporter
{
    protected override void OnPlayerContact(Player player)
    {
        base.OnPlayerContact(player);

        StartCoroutine(TeleportPlayer(_defaultSpawnPoint.position));
    }
}