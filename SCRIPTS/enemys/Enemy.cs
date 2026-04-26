using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Transform player;
    protected Room myRoom;
    protected bool isActive = false;
    public bool isPaused = false; 

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.enabled = false; 
        }
    }

    public virtual void ActivateEnemy(Room room)
    {
        myRoom = room;
        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null) player = pObj.transform;
        
        if (gameObject.activeInHierarchy)
            StartCoroutine(SnapToNavMesh());
    }

    private IEnumerator SnapToNavMesh()
    {
        yield return new WaitForSeconds(0.15f); 
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        if (agent != null)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
            {
                agent.enabled = true;
                agent.Warp(hit.position);
                isActive = true;
            }
        }
    }

    protected virtual void Update()
    {
        if (!isActive || isPaused || player == null || agent == null || !agent.enabled) return;
        agent.SetDestination(player.position);
    }

    protected virtual void OnDestroy()
    {
        if (myRoom != null) myRoom.EnemyDied();
    }
}