using UnityEngine;
using UnityEngine.UI;

public class ZHealthBar : MonoBehaviour
{
    public Slider slider;
    private float _maxHealth;
    private float _currentHealth;

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

    void Awake()
    {
        slider = GetComponent<Slider>();
    }
}