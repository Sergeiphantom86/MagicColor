public interface ICurrencyProgress
{
    public void SetCurrency(Currency currency, long balance);

    public void SetReward(int reward);

    public void SetCountStars(int count);

    public void SaveSpinsCount(int spins);

    public void SetQuantityAbilities(int quantityAbilities);

    public void SaveBalanceAfterPurchase(long balans);
}