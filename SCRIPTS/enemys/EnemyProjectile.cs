using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 5f;

    void Start()
    {
        // Пуля сама удалится через 5 секунд, если ни во что не попадет
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            Destroy(gameObject); // Исчезает при попадании в игрока
        }
        else if (collision.CompareTag("Wall")) 
        {
            Destroy(gameObject); // Исчезает при ударе о стену
        }
    }
}