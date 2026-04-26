using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class ShooterEnemy : Enemy
{
    public GameObject bulletPrefab;
    public float shootInterval = 2f;
    public float hoverDistance = 5f; // Дистанция, на которой он держится
    
    private float shootTimer;

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Поведение "мухи": если слишком близко — отлетает, если далеко — подходит
        if (distance > hoverDistance)
            agent.SetDestination(player.position);
        else
            agent.SetDestination(transform.position + (transform.position - player.position).normalized);

        // Стрельба
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            Shoot();
            shootTimer = 0;
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Vector2 direction = (player.position - transform.position).normalized;
        bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * 5f;
    }
}