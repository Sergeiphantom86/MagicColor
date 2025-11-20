using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public class MenuLoader : MonoBehaviour
{
    private const string Menu = nameof(Menu);
    private const string Tutorial = nameof(Tutorial);

    [SerializeField] private CoinWallet _coinWallet;
    [SerializeField] private CrystalWallet _crystalWallet;

    public void TargetScene(string scenName)
    {

        if (SceneLoader.Instance == null)
        {
            Debug.LogError("SceneLoader instance not found! Using default load.");
            SceneManager.LoadScene(Menu);
            return;
        }

        ReturnIndex();

        TryResetSprite();

        YG2.SaveProgress();

        SceneLoader.Instance.LoadSceneWithSplash(scenName);
    }

    public void SaveCurrency()
    {
        if (_coinWallet != null && _crystalWallet != null)
        {
            YG2.saves.SetCurrency(_coinWallet, _coinWallet.Balance);
            YG2.saves.SetCurrency(_crystalWallet, _crystalWallet.Balance);
        }
    }

    private void TryResetSprite()
    {
        if (SceneManager.GetActiveScene().name != Tutorial)
        {
            YG2.saves.ResetSprite();

            SaveCurrency();
        }
    }

    private void ReturnIndex()
    {
        if (gameObject.TryGetComponent(out Exit exit))
        {
            YG2.saves.SetQuestIndex(exit.GetIndex());
        }
    }
}