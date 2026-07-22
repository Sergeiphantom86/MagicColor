using System;
using Wallets.WalletEconomy;

namespace PuzzleResources.Walls.WallEngineResources
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
            if (_bag.CanSpend(_price))
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