using System;
using Wallets.WalletEditor;
namespace PuzzleEditor.Walls.WallEngineEditor
{

public class BagUnlockPolicy : IUnlockPolicy
{
    private readonly BagKey _bag;
    private readonly int _price;

    public BagUnlockPolicy(BagKey bag, int price)
    {
        _bag = bag != null ? bag : throw new ArgumentNullException(nameof(bag));
        _price = price;
    }

    public bool TryUnlock()
    {
        if (_bag.TryApply(_price))
        {
            return true;
        }

        return false;
    }

    public void Use()
    {
        _bag.Use();
    }
}
}