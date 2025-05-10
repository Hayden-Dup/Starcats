using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Tooltip("Assign the world object's Health component")]
    public Health health;

    Slider _slider;

    void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.maxValue = health.maxHealth;
    }

    void Update()
    {
        _slider.value = health.currentHealth;
    }
}
