using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

[RequireComponent(typeof(Button))]
public class SceneLoadButton : MonoBehaviour
{
    private const string Menu = nameof(Menu);

    [SerializeField] private CoinWallet _coin;
    [SerializeField] private CrystalWallet _crystal;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(LoadTargetScene);
    }

    private void LoadTargetScene()
    {
        YG2.saves.SetAssembledPuzzle(false);

        if (SceneLoader.Instance == null)
        {
            Debug.LogError("SceneLoader instance not found! Using default load.");
            SceneManager.LoadScene(Menu);
            return;
        }

        if (_coin != null && _crystal != null)
        {
            YG2.saves.SetCurrentCoin(_coin.Balance);
            YG2.saves.SetCurrentCrystal(_crystal.Balance);
            YG2.saves.SetAssembledPuzzle(true);
            YG2.SaveProgress();
        }

        YG2.saves.ResetSprite();
        SceneLoader.Instance.LoadSceneWithSplash(Menu);
    }

    private void OnDestroy()
    {
        if (_button != null)
            _button.onClick.RemoveListener(LoadTargetScene);
    }
}