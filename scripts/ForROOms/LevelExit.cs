using UnityEngine;

namespace Assignment8 // Добавляем эту строку
{
    public class LevelExit : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (LevelGenerator.Instance != null)
                {
                    LevelGenerator.Instance.NextFloor();
                }
            }
        }
    }
} // И закрывающую скобку в конце