using UnityEngine.UI;
using UnityEngine;

public class HealthOrb : MonoBehaviour
{
    private Slider slider;
    private float _maxHealth = 100f;
    private float _currentHealth = 100f;

    public float MaxHealth
    {
        get { return _maxHealth; }
        set
        {
            _maxHealth = value;
            slider.maxValue = value;
        }
    }
    public float CurrentHealth
    {
        get { return _currentHealth; }
        set
        {
            _currentHealth = value;
            slider.value = value;
        }
    }

    private void Awake()
    {
        slider = transform.GetComponentInChildren<Slider>();
    }
}

