using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 2f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
            {
            Room room = FindObjectOfType<Room>();
            room.EnemyDied();

            Destroy(other.gameObject);
            Destroy(gameObject);
            }
    }
}