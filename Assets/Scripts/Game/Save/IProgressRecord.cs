using YG;

public interface IProgressRecord
{
    public void SaveProgress();
    public void SetDefaultValues();
    SavesYG Saves { get; }
}