using UnityEngine;
using UnityEngine.AI; // Обязательно для навигации

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // Настройки для 2D, чтобы враг не "вставал" вертикально
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null) player = pObj.transform;
    }

    void Update()
    {
        if (player != null && agent.enabled)
        {
            // Указываем цель для навигации
            agent.SetDestination(player.position);
        }
    }
}