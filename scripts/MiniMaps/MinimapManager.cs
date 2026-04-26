using UnityEngine;
using System.Collections.Generic;

public class MinimapManager : MonoBehaviour
{
    public static MinimapManager Instance;

    public GameObject iconPrefab;
    public Transform container; 
    public float iconSpacing = 40f;
    
    private Dictionary<Vector2Int, MinimapIcon> mapIcons = new Dictionary<Vector2Int, MinimapIcon>();
    private List<Vector2Int> visitedRooms = new List<Vector2Int>();

    void Awake() => Instance = this;

    public void ClearMap()
    {
        if (container == null) return;

        for (int i = container.childCount - 1; i >= 0; i--)
        {
            GameObject child = container.GetChild(i).gameObject;
            // Убеждаемся, что не удаляем сам префаб, если он по ошибке в дочерних
            if (child != iconPrefab) 
            {
                Destroy(child);
            }
        }

        mapIcons.Clear();
        visitedRooms.Clear();
    }

    public void InitializeMap(IEnumerable<Vector2Int> roomPositions)
    {
        ClearMap();

        if (iconPrefab == null)
        {
            Debug.LogError("MinimapManager: Icon Prefab не назначен!");
            return;
        }

        foreach (var pos in roomPositions)
        {
            GameObject go = Instantiate(iconPrefab, container);
            if (go == null) continue;

            RectTransform rect = go.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = new Vector2(pos.x * iconSpacing, pos.y * iconSpacing);
            }
            
            MinimapIcon icon = go.GetComponent<MinimapIcon>();
            if (icon != null)
            {
                mapIcons[pos] = icon;
                icon.SetState(false, false);
            }
        }
    }

    public void UpdateMap(Vector2Int currentPos)
    {
        if (!visitedRooms.Contains(currentPos))
            visitedRooms.Add(currentPos);

        foreach (var pair in mapIcons)
        {
            // Чтобы избежать ошибок, если иконка была удалена при Reset
            if (pair.Value == null) continue;

            bool isCurrent = (pair.Key == currentPos);
            bool isVisited = visitedRooms.Contains(pair.Key);
            
            pair.Value.SetState(isCurrent, isVisited);
        }
    }
}