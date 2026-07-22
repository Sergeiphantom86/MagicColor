using YG;

namespace Wallets
{
    public class CoinWallet : Wallet
    {
        protected override long LoadBalanceFromSave() =>
            YG2.saves.CurrentCoin;

        protected override void SaveBalanceToSave(long balance) =>
            YG2.saves.CurrentCoin = balance;
    }
}