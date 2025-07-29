using UnityEngine;
using System.Collections;

public class FootTrigger : MonoBehaviour
{
    [SerializeField] private FootstepSound footstepController;
    [SerializeField] private int footIndex;

    private bool[] _footPlaying = new bool[2];

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем поверхность и статус ноги
        if (other.TryGetComponent(out Terrain terrain) && _footPlaying[footIndex] == false)
        {
            //StartCoroutine(PlayFootstepWithCooldown(footIndex));
        }
    }

    //private IEnumerator PlayFootstepWithCooldown(int index)
    //{
    //    _footPlaying[index] = true;

    //    // Воспроизводим звук и получаем параметры
    //    (AudioClip clip, float pitch) = footstepController.PlayFootstepSound();

    //    if (clip != null)
    //    {
    //        // Рассчитываем длительность с учетом pitch
    //        float duration = clip.length / Mathf.Abs(pitch);

    //        // Ждем 110% от длительности звука
    //        yield return new WaitForSeconds(clip.length);
    //    }
    //    else
    //    {
    //        // Минимальная задержка если звука нет
    //        yield return new WaitForSeconds(0.1f);
    //    }

    //    // Разблокируем ногу
    //    _footPlaying[index] = false;
    //}
}