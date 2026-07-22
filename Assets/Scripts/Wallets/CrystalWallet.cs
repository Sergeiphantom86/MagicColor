using YG;

namespace Wallets
{
    public class CrystalWallet : Wallet
    {
        private const float UpdateDelay = 1.5f;

        protected override long LoadBalanceFromSave() =>
            YG2.saves.CurrentCrystal;

        protected override void SaveBalanceToSave(long balance) =>
            YG2.saves.CurrentCrystal = balance;

        protected override float GetLoadDelay() =>
            UpdateDelay;
    }
}