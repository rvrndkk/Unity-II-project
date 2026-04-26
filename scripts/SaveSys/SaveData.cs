[System.Serializable]
public class SaveData
{
    public int currentHealth;
    public int totalMoney;
    public string lastRoomID; // Например, координаты сетки
    public System.Collections.Generic.List<string> collectedArtifactNames; 

    public SaveData()
    {
        collectedArtifactNames = new System.Collections.Generic.List<string>();
    }
}