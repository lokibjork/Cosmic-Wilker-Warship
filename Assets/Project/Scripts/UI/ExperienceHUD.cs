using UnityEngine;
using UnityEngine.UI;
using TMPro; // Se usares TextMeshPro para mostrar o nível

public class ExperienceHUD : MonoBehaviour
{
    [SerializeField] private LevelSystem levelSystem;
    [SerializeField] private Slider xpSlider;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject notificationIcon; // O ícone "UP!" que avisa do cassino

    private void Start()
    {
        // Auto-busca se esqueceres
        if (levelSystem == null) levelSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<LevelSystem>();

        // Inscreve nos eventos
        levelSystem.OnExperienceChanged.AddListener(UpdateBar);
        levelSystem.OnLevelChanged.AddListener(UpdateLevelText);
        levelSystem.OnPendingUpgradesChanged.AddListener(ToggleNotification);

        // Inicia desligado
        if (notificationIcon) notificationIcon.SetActive(false);
    }

    private void UpdateBar(int current, int max)
    {
        xpSlider.maxValue = max;
        xpSlider.value = current;
    }

    private void UpdateLevelText(int level)
    {
        if (levelText) levelText.text = $"LVL {level}";
    }

    private void ToggleNotification(bool hasUpgrades)
    {
        if (notificationIcon) notificationIcon.SetActive(hasUpgrades);
    }
}