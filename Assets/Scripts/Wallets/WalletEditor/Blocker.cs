using PuzzleEditor;
using UnityEngine;
namespace Wallets.WalletEditor
{

public class Blocker : MonoBehaviour, IActivatable
{
    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
}