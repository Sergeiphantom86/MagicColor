using UnityEngine;
using DG.Tweening;

public class FragmentAnimator : MonoBehaviour, IFragmentAnimator
{
    [SerializeField] private Sprite _sprite;

    public void ActivateFragment(Fragment fragment)
    {
        fragment.gameObject.SetActive(true);
        fragment.TurnOffTransparency();
        fragment.SetSprite(_sprite);
        AnimateAppearance(fragment.transform);
    }

    private void AnimateAppearance(Transform fragmentTransform)
    {
        Vector3 originalScale = fragmentTransform.localScale;
        fragmentTransform.localScale = Vector3.zero;
        fragmentTransform.DOScale(originalScale, 0.5f).SetEase(Ease.OutElastic);
    }
}