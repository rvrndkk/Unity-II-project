using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Movement & Combat")]
    public float speed = 5f;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public float fireRate = 0.3f;

    private float nextFireTime;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 shootInput;
    private PlayerInputActions inputActions;

    [Header("Artifacts")]
    // Список объектов данных для расчета бонусов
    public List<ArtifactData> collectedArtifacts = new List<ArtifactData>();

    // Геттеры: автоматически пересчитывают статы при каждом обращении
    public float CurrentSpeed => speed * GetTotalSpeedBonus();
    public float CurrentFireRate => fireRate * GetTotalFireRateBonus();

    void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Чтобы физика не засыпала и переходы между комнатами работали четко
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        
        LoadSavedProgress();
    }

    void Update()
    {
        // Чтение ввода из New Input System
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        shootInput = inputActions.Player.Shoot.ReadValue<Vector2>();

        // Логика стрельбы с учетом бонусов скорострельности
        if (shootInput != Vector2.zero && Time.time > nextFireTime)
        {
            Shoot(shootInput);
            nextFireTime = Time.time + CurrentFireRate;
        }
    }

    void FixedUpdate()
    {
        // Движение через Rigidbody с учетом бонусов скорости
        rb.linearVelocity = moveInput * CurrentSpeed;
    }

    // --- Логика Бонусов ---

    float GetTotalSpeedBonus() 
    {
        float bonus = 1f;
        foreach(var a in collectedArtifacts) bonus *= a.speedMultiplier;
        return bonus;
    }

    float GetTotalFireRateBonus() 
    {
        float bonus = 1f;
        foreach(var a in collectedArtifacts) bonus *= a.fireRateMultiplier;
        return bonus;
    }

    // --- Подбор и Сохранение ---

    public void AddArtifact(ArtifactData artifact) 
    {
        if (artifact == null) return;

        collectedArtifacts.Add(artifact);
        Debug.Log($"<color=green>Подобран артефакт:</color> {artifact.artifactName}");

        SaveCurrentProgress();
    }

    private void SaveCurrentProgress() 
    {
        SaveData data = new SaveData();
        // Если GameManager готов, берем HP оттуда, если нет - ставим заглушку 100
        data.currentHealth = (GameManager.Instance != null) ? GameManager.Instance.currentHealth : 100;
        
        foreach(var a in collectedArtifacts) 
        {
            data.collectedArtifactNames.Add(a.artifactName);
        }
        
        SaveManager.SaveGame(data);
    }

    private void LoadSavedProgress()
    {
        SaveData data = SaveManager.LoadGame();
        if (data == null || data.collectedArtifactNames == null) return;

        collectedArtifacts.Clear();

        foreach (string name in data.collectedArtifactNames)
        {
            // Пытаемся найти артефакт в папке Assets/Resources/Artifacts
            ArtifactData loaded = Resources.Load<ArtifactData>("Artifacts/" + name);
            if (loaded != null)
            {
                collectedArtifacts.Add(loaded);
                Debug.Log($"<color=blue>Загружен артефакт:</color> {name}");
            }
        }
    }

    void Shoot(Vector2 direction)
    {
        if (bulletPrefab == null) return;
        
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = direction.normalized * bulletSpeed;
        }
    }
}