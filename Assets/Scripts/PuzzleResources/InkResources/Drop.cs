using PuzzleResources.Audio;
using UnityEngine;

namespace PuzzleResources.InkResources
{
    [RequireComponent(typeof(Voiceover))]

    public class Drop : ColorableObject
    {
        [SerializeField] private AudioClip _spawn;
        [SerializeField] private AudioClip _moving;

        private Voiceover _voiceover;

        private void Awake()
        {
            InitializeComponents();
            _voiceover = GetComponent<Voiceover>();
        }

        public void PlaySoundSpawn()
        {
            _voiceover.PlayOneShot(_spawn);
        }

        public void PlaySoundMoving()
        {
            _voiceover.PlayOneShot(_moving);
        }
    }
}