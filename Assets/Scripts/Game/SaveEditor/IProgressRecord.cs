using YG;
namespace Game.SaveEditor
{

public interface IProgressRecord
{
    SavesYG Saves { get; }

    public void SaveProgress();

    public void SetDefaultValues();
}
}