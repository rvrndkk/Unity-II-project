// Door.cs
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public DoorDirection direction;

    [HideInInspector] public Room parentRoom;
    [HideInInspector] public Room targetRoom;

    public Transform spawnPoint;

    private bool isTransitioning;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning) return;
        if (!other.CompareTag("Player")) return;
        if (targetRoom == null) return;

        Door targetDoor = targetRoom.GetDoor(GetOpposite(direction));
        if (targetDoor == null || targetDoor.spawnPoint == null) return;

        isTransitioning = true;

        GameManager.Instance.MoveToRoom(
            targetRoom,
            targetDoor.spawnPoint.position
        );

        targetRoom.OnEnter();

        Invoke(nameof(ResetTransition), 0.5f);
    }

    private void ResetTransition()
    {
        isTransitioning = false;
    }

    private DoorDirection GetOpposite(DoorDirection dir)
    {
        switch (dir)
        {
            case DoorDirection.Up: return DoorDirection.Down;
            case DoorDirection.Down: return DoorDirection.Up;
            case DoorDirection.Left: return DoorDirection.Right;
            case DoorDirection.Right: return DoorDirection.Left;
            default: return DoorDirection.Down;
        }
    }
}