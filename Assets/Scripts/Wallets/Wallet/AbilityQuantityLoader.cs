using UnityEngine;

public class AbilityQuantityLoader : MonoBehaviour
{
    [SerializeField] private PurchaseButton _purchaseButton;

    private int _balance;
    private BagAbilities _bagAbilities;
    private IProgressSaver _progressSaver;

    private void Awake()
    {
        _progressSaver = new ProgressSaver();
        _bagAbilities = GetComponent<BagAbilities>();

        if (_progressSaver == null)
        {
            Debug.LogError("ProgressSaver == null");
        }

        if (_bagAbilities == null)
        {
            Debug.LogError("BagAbilities == null");
        }

        UpdateBalance(GetBalance());
    }

    private void Start()
    {
        _bagAbilities.Add(GetBalance());
    }

    private void OnEnable()
    {
        _bagAbilities.OnBagChanged += UpdateBalance;
        _purchaseButton.OnClick += Add;
    }

    private void OnDisable()
    {
        _bagAbilities.OnBagChanged -= UpdateBalance;
        _purchaseButton.OnClick -= Add; 
    }

    private void OnDestroy()
    {
        SaveToFile();
    }

    private int GetBalance()
    {
        return _progressSaver.Saves.QuantityAbilities;
    }

    private void UpdateBalance(int balance)
    {
        _balance = balance;
    }

    private void SaveToFile()
    {
        _progressSaver.SetQuantityAbilities(_balance);
    }

    private void Add()
    {
        if (_bagAbilities != null)
            _bagAbilities.Add();
    }
}