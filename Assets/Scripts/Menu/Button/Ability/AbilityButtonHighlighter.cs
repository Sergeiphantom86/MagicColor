using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button),typeof(Image))]
public class AbilityButton : MonoBehaviour
{
    [SerializeField] private Ability ability;
    [SerializeField] private Image _highlightImage;

    private Button _button;
    private Image _image;
    private BagAbilities _bag;
    private bool _isUsed;
    private Blocker _blocker;
    private IProgressSaver _progressSaver;

    public Button Button => _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _image = GetComponent<Image>();
        _bag = GetComponentInChildren<BagAbilities>();
        _blocker = GetComponentInChildren<Blocker>();
        _progressSaver = new ProgressSaver();

        _highlightImage.enabled = false;

        _image.sprite = ability.Icon;

        _blocker.gameObject.SetActive(false);
        _button.interactable = true;
    }

    private void Start()
    {
        if (_blocker != null && _progressSaver.Saves.IsUnlockAbilities == false)
        {
            _blocker.gameObject.SetActive(true);
            _button.interactable = false;
        }
        
        AbilitySelectionManager.Instance.OnSelection += Use;
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClick);
        AbilitySelectionManager.Instance.OnSelection -= Use;
    }

    private void OnClick()
    {
        if(_isUsed) return;
        
        if (_bag.TryApply() == false) return;
        
        _isUsed = true;

        AbilitySelectionManager.Instance.Select(this);
    }

    public Ability Ability => ability;

    public void SetHighlight(bool value)
    {
        _highlightImage.enabled = value;
        _isUsed = value;
    }

    private void Use()
    {
        _bag.Use();
    }
}