using TMPro;
using UnityEngine;

public class TextSwitcher : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _notificationText;
    private Rewards _awardText;

    private void Awake()
    {
        _awardText = GetComponentInChildren<Rewards>(true);
    }

    public void TurnOffDesiredOne(bool isOn)
    {
        if (isOn == false)
        {
            _notificationText.gameObject.SetActive(false);
            _awardText.gameObject.SetActive(true);
            return;
        }

        _awardText.gameObject.SetActive(false);
        _notificationText.gameObject.SetActive(true);
    }
}