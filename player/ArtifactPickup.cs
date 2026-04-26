using UnityEngine;

public class ArtifactPickup : MonoBehaviour
{
    public ArtifactData artifactData; // Назначь нужный артефакт в Инспекторе

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что объект столкновения — игрок
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.AddArtifact(artifactData); // Добавляем бонусы
                Destroy(gameObject); // Убираем предмет с уровня
            }
        }
    }
}