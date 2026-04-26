using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int maxHealth = 100; // Та самая переменная, которой не хватало
    public int currentHealth;
    [Header("Current State")]
    public Room currentRoom;
    public Transform player;

    public float playerSpeed = 5;
    [Header("Camera Settings")]
    [SerializeField] private bool smoothCamera = true;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        currentHealth = maxHealth;
    }
    public void PickUpArtifact(ArtifactData data)
{
    // Применяем бонусы к игроку
    playerSpeed *= data.speedMultiplier;
    maxHealth += data.healthBonus;
    
    // Сохраняем прогресс автоматически
    SaveData currentSave = SaveManager.LoadGame();
    currentSave.collectedArtifactNames.Add(data.artifactName);
    SaveManager.SaveGame(currentSave);
}
    public void MoveToRoom(Room newRoom, Vector3 spawnPos)
    {
        if (newRoom == null || player == null) 
        {
            Debug.LogWarning("MoveToRoom: Отсутствует ссылка на комнату или игрока!");
            return;
        }

        // 1. Перемещаем игрока в новую точку
        player.transform.position = spawnPos;
        
        // 2. Обновляем текущую комнату
        // Мы НЕ выключаем старую комнату (SetActive), 
        // так как они теперь разнесены в пространстве.
        currentRoom = newRoom;

        // 3. Активируем логику новой комнаты (враги, ловушки и т.д.)
        currentRoom.OnEnter();

        // 4. Мгновенно перемещаем камеру (если она не привязана к игроку жестко)
        UpdateCamera(spawnPos);

        // 5. Обновление миникарты
        if (MinimapManager.Instance != null)
            MinimapManager.Instance.UpdateMap(currentRoom.gridPos);
            
        Debug.Log($"Переход в комнату: {currentRoom.gridPos}");
    }

    private void UpdateCamera(Vector3 targetPos)
    {
        // Если у тебя камера — отдельный объект, ей нужно мгновенно прыгнуть к игроку,
        // чтобы не было долгого "пролета" через пустоту.
        if (Camera.main != null)
        {
            Vector3 camPos = targetPos;
            camPos.z = -10f; // Сохраняем стандартную глубину камеры
            
            if (!smoothCamera)
            {
                Camera.main.transform.position = camPos;
            }
            // Если камера плавная (Cinemachine), она сама долетит, 
            // но лучше вызвать Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.OnTargetObjectWarped(...)
        }
    }

    public void WinGame()
    {
        Debug.Log("YOU WIN!");
        // Включаем UI победы, если он есть
        // UIManager.Instance.ShowWinScreen();
        Time.timeScale = 1f; 
    }
}