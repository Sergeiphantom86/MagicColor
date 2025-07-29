using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField] private Sprite _npcSprite;

    private void Start()
    {
        //if (EntryPoint.Instance.IsQuestCompleted(_npcSprite.name))
        //{
        //    Debug.Log("Квест у этого NPC уже выполнен!");
        //    // Показать соответствующий диалог/действия
        //}
        //else
        //{
        //    Debug.Log("Квест еще не выполнен");
        //    // Предложить новый квест
        //}
    }
}