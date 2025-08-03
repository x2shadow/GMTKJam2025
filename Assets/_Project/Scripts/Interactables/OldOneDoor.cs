using UnityEngine;

public class OldOneDoor : MonoBehaviour
{
    [Header("Ссылки")]
    public Transform doorLockedTarget; // Целевая позиция, куда поедет дверь
    public float moveSpeed = 2f;        // Скорость движения двери
    public AudioSource audioSource;
    public AudioClip doorCloseClip;

    private bool wasUsed = false;
    private bool isMoving = false;
    private bool isPlayerNearby = false;

    private void OnTriggerEnter(Collider other)
    {
        if (wasUsed) return;

        // Проверяем, игрок ли вошёл
        if (other.CompareTag("Player"))
        {
            Debug.Log("Вошел в триггер");
            audioSource.Play();
            wasUsed = true;
            isMoving = true; // Запускаем движение двери
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    private void Update()
    {
        if (isMoving && doorLockedTarget != null)
        {
            // Плавно перемещаем дверь к целевой позиции
            transform.position = Vector3.MoveTowards(transform.position, doorLockedTarget.position, moveSpeed * Time.deltaTime);

            // Остановить движение, если достигли позиции
            if (Vector3.Distance(transform.position, doorLockedTarget.position) < 0.01f)
            {
                isMoving = false;
            }
        }
    }
}

