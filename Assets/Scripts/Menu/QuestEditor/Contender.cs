using UnityEngine;

namespace Menu.QuestEditor
{
    public class Contender : MonoBehaviour
    {
        public Transform[] AllChildren
        {
            get
            {
                int count = transform.childCount;
                Transform[] children = new Transform[count];
                for (int i = 0; i < count; i++)
                    children[i] = transform.GetChild(i);

                return children;
            }
        }
    }
}