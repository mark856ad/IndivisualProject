using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthHUD : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text healthText;

    private void OnEnable()
    {
        playerHealth.HealthChanged += Refresh;
    }

    private void Start()
    {
        Refresh(playerHealth.CurrentHealth, playerHealth.MaxHealth);
    }

    private void OnDisable()
    {
        playerHealth.HealthChanged -= Refresh;
    }

    private void Refresh(float current, float max)
    {
        fillImage.fillAmount = max > 0f ? current / max : 0f;
        healthText.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
    }
}