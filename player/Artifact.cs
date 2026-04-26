using UnityEngine;

[CreateAssetMenu(fileName = "New Artifact", menuName = "Inventory/Artifact")]
public class ArtifactData : ScriptableObject
{
    public string artifactName;
    public string description;
    public Sprite icon;
    
    [Header("Bonuses")]
    public float speedMultiplier = 1f;
    public int healthBonus = 0;
    public float fireRateMultiplier = 1f; // Теперь контроллер увидит эту переменную
}