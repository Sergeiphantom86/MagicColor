using System;
using UnityEngine;

public class AbilitySelectionManager : MonoBehaviour
{
    public static AbilitySelectionManager Instance;

    private Ability currentAbility;
    private AbilityButton currentButton;

    public bool HasSelection => currentAbility != null;

    public event Action OnSelection;

    private void Awake()
    {
        Instance = this;
    }

    public void Select(AbilityButton button)
    {
        if (currentButton != null)
            currentButton.SetHighlight(false);

        currentButton = button;
        currentAbility = button.Ability;

        currentButton.SetHighlight(true);
    }

    public Ability GetSelectedAbility()
    {
        return currentAbility;
    }

    public void ClearSelection()
    {
        if (currentButton == null) return;

        currentButton.SetHighlight(false);
        currentButton = null;
        currentAbility = null;
    }

    public void Use()
    {
        OnSelection?.Invoke();
    }
}