using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthBar : MonoBehaviour
{
    public GameObject heartPrefab; // Префаб с компонентом Image
    public Sprite fullHeart;
    public Sprite halfHeart; // Если есть
    public Sprite emptyHeart;

    private List<Image> hearts = new List<Image>();

    public void SetupHearts(int maxHealth)
    {
        // Допустим, 1 сердечко = 20 ХП. Значит 100 ХП = 5 сердечек.
        int heartCount = maxHealth / 20; 

        foreach (Transform child in transform) Destroy(child.gameObject);
        hearts.Clear();

        for (int i = 0; i < heartCount; i++)
        {
            GameObject h = Instantiate(heartPrefab, transform);
            hearts.Add(h.GetComponent<Image>());
        }
    }

    public void UpdateHearts(int currentHealth)
    {
        // Простая логика: каждое сердечко отвечает за 20 единиц здоровья
        for (int i = 0; i < hearts.Count; i++)
        {
            int heartValue = (i + 1) * 20;

            if (currentHealth >= heartValue)
                hearts[i].sprite = fullHeart;
            else if (currentHealth >= heartValue - 10 && halfHeart != null)
                hearts[i].sprite = halfHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }
}