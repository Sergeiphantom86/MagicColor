using UnityEngine;

namespace Menu
{
    public class Background : MonoBehaviour
    {
        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}