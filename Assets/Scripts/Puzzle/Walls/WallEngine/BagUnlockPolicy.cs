public class BagUnlockPolicy : IUnlockPolicy
{
    private readonly Bag _bag;
    private readonly int _price;

    public BagUnlockPolicy(Bag bag, int price)
    {
        _bag = bag;
        _price = price;
    }

    public bool TryUnlock()
    {
        return _bag.SpendFunds(_price);
    }
}