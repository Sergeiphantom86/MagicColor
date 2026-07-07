public class UnityMethodOrderViolation
{
    public string AssetPath;
    public string Description;

    public string Id => $"{AssetPath}|{Description}";
}