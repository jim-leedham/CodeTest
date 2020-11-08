using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : Singleton<GameUI>
{
    [SerializeField] private Image healthBar;
    [SerializeField] private Text healthText;

    [SerializeField] private MessageUI messageUI;

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        float ratio = currentHealth / maxHealth;
        healthBar.rectTransform.localPosition = new Vector3(healthBar.rectTransform.rect.width * ratio - healthBar.rectTransform.rect.width, 0, 0);
        healthText.text = currentHealth.ToString("0") + "/" + maxHealth.ToString("0");
    }

    public void ShowMessage(string title, string message, Color messageColor, float time)
    {
        messageUI.ShowUI(title, message, messageColor, time);
    }
}
