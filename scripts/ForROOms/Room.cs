// Room.cs
using UnityEngine;
using Assignment8;

public class Room : MonoBehaviour
{
    public Vector2Int gridPos;
    public bool isFinishRoom;

    [Header("Doors")]
    public Door doorUp;
    public Door doorDown;
    public Door doorLeft;
    public Door doorRight;

    [Header("Combat")]
    public GameObject gatesObject;

    private int enemiesAlive;
    private bool cleared;

    public void SetupDoors()
    {
        SetupDoor(doorUp, gridPos + Vector2Int.up);
        SetupDoor(doorDown, gridPos + Vector2Int.down);
        SetupDoor(doorLeft, gridPos + Vector2Int.left);
        SetupDoor(doorRight, gridPos + Vector2Int.right);
    }

    private void SetupDoor(Door door, Vector2Int neighbourPos)
    {
        if (door == null) return;

        door.parentRoom = this;

        if (LevelGenerator.Instance.spawnedRooms.TryGetValue(neighbourPos, out Room neighbour))
        {
            door.gameObject.SetActive(true);
            door.targetRoom = neighbour;
        }
        else
        {
            door.gameObject.SetActive(false);
        }
    }

    public Door GetDoor(Door.DoorDirection dir)
    {
        switch (dir)
        {
            case Door.DoorDirection.Up: return doorUp;
            case Door.DoorDirection.Down: return doorDown;
            case Door.DoorDirection.Left: return doorLeft;
            case Door.DoorDirection.Right: return doorRight;
            default: return null;
        }
    }

    public void OnEnter()
    {
        Enemy[] enemies = GetComponentsInChildren<Enemy>(true);

        enemiesAlive = 0;

        foreach (Enemy enemy in enemies)
        {
            if (!enemy.gameObject.activeSelf)
                enemy.gameObject.SetActive(true);

            enemy.ActivateEnemy(this);
            enemiesAlive++;
        }

        if (enemiesAlive > 0 && !isFinishRoom && !cleared)
            CloseDoors();
        else
            OpenDoors();
    }

    public void EnemyDied()
    {
        enemiesAlive--;

        if (enemiesAlive <= 0)
        {
            cleared = true;
            OpenDoors();
        }
    }

    public void CloseDoors()
    {
        if (gatesObject != null)
            gatesObject.SetActive(true);
    }

    public void OpenDoors()
    {
        if (gatesObject != null)
            gatesObject.SetActive(false);
    }
}