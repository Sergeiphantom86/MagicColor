namespace Wallets.WalletEditor
{
    public interface IProcessTransacter
    {
        public bool ProcessTransaction(long amount, long balance);
    }
}