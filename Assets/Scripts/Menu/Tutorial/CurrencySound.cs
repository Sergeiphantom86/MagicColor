using UnityEngine;

public class CurrencySound : MonoBehaviour
{
    [SerializeField] AudioClip _audioClip;

    private Voiceover _voiceover;

    private void Awake()
    {
        _voiceover = GetComponent<Voiceover>();
    }

    private void OnEnable()
    {
        _voiceover.Play(_audioClip);
    }
}