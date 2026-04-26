using UnityEngine;

public class FinishRoom : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            GameManager.Instance.WinGame();
        }
    }
}