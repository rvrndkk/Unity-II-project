using UnityEngine;

public class ChaserEnemy : Enemy
{
    void Update()
    {
        if (isPaused || player == null || agent == null) return;

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        }
    }
}