using UnityEngine;

namespace Fireworks
{
    [CreateAssetMenu(fileName = "FireworksSoundPack", menuName = "Audio/Fireworks Sound Pack")]

    public class FireworksSoundPack : ScriptableObject
    {
        public AudioClip ExplosionSound;

        public AudioClip SparkleSound;

        public AudioClip GlowSound;
    }
}