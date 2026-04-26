using UnityEngine;
using UnityEngine.UI;

public class MinimapIcon : MonoBehaviour
{
    public Image image;
    public Color visitedColor = Color.white;
    public Color currentColor = Color.yellow;
    public Color hiddenColor = new Color(1, 1, 1, 0.3f); // Полупрозрачный

    public void SetState(bool isCurrent, bool isVisited)
    {
        image.enabled = isVisited || isCurrent; // Показываем только если были там
        
        if (isCurrent) image.color = currentColor;
        else if (isVisited) image.color = visitedColor;
        else image.color = hiddenColor;
    }
}