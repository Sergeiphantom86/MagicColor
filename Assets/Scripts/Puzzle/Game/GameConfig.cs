using UnityEngine;

[CreateAssetMenu(menuName = "Game/GameConfig")]
public class GameConfig : ScriptableObject
{
    [System.Serializable]
    public class AnalyzerSettings
    {
        public bool IgnoreTransparent;
        [Range(0, 1)] public float IgnoredAlpha = 0.1f;
    }

    public AnalyzerSettings analyzerSettings;
}