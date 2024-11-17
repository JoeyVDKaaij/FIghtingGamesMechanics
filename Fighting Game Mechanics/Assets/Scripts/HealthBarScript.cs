using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Slider))]
public class HealthBarScript : MonoBehaviour
{
    [Header("Healthbar Settings")] 
    [SerializeField, Tooltip("Set the speed that the healthbar moves in."), Range(0.00000001f, 1)]
    private float healthbarSpeed = 1;
    [SerializeField, Tooltip("Set the health to test the healthbar."), Min(0)]
    private int setHealthDebug = 100;
    [SerializeField, Tooltip("Set the text that shows the amount of health.")]
    private TMP_Text healthbarText = null;
    [SerializeField, Tooltip("Set the health type.")]
    private HealthType healthType = HealthType.Normal;
    [SerializeField, Tooltip("Set the health regeration speed."), Min(0.000001f)]
    private float timeUntilHealthRegerates = 1;
    
    private int _currentHealth = 100;
    private float _sliderValue = 100;
    private int _healthDifference = 100;
    private float _timer = 0;

    private Slider _slider;
    private ComboMeterScript _comboMeterScript;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.maxValue = _currentHealth;
        _slider.value = _currentHealth;

        if (healthbarText != null)
            healthbarText.SetText($"{(int)_slider.value} / {_slider.maxValue}");

        _comboMeterScript = GetComponent<ComboMeterScript>();
    }

    private void Update()
    {
        UpdateSlider();

        if (setHealthDebug != _currentHealth)
        {
            UpdateHealth(setHealthDebug);
        }

        if (healthType == HealthType.Healing)
        {
            _timer += Time.deltaTime;
            if (_timer >= timeUntilHealthRegerates)
            {
                _currentHealth++;
                _currentHealth = (int)Math.Clamp(_currentHealth, 0, _slider.maxValue);
                setHealthDebug = _currentHealth;
                _timer = 0;
            }
        }

        _slider.value = _sliderValue;
    }

    /* Updates the slider depending on the health that the player has. */
    private void UpdateSlider()
    {
        if (_slider.value < _currentHealth)
        {
            _sliderValue += _healthDifference * healthbarSpeed;

            _sliderValue = Mathf.Clamp(_sliderValue, _currentHealth - _healthDifference, _currentHealth);
            
            if (healthbarText != null)
                healthbarText.SetText($"{(int)_sliderValue} / {_slider.maxValue}");
        }
        else if (_slider.value > _currentHealth)
        {
            _sliderValue -= _healthDifference * healthbarSpeed;

            if (_currentHealth <= 0)
            {
                if (healthType == HealthType.Reset)
                {
                    _currentHealth = (int)_slider.maxValue;
                    _slider.value = (int)_slider.maxValue;
                }
                else
                {
                    _currentHealth = 0;
                    _sliderValue = Mathf.Clamp(_sliderValue, _currentHealth, _currentHealth + _healthDifference);
                }
            }
            
            if (healthbarText != null)
                healthbarText.SetText($"{(int)_sliderValue} / {_slider.maxValue}");
        }
    }

    /* Updates how much health the player currently has.
     * pHealth is the amount of health the player should now have.
     */
    public void UpdateHealth(int pHealth)
    {
        _healthDifference = (int)_slider.value - _currentHealth + (_currentHealth - pHealth);
        if (_healthDifference < 0) _healthDifference = -_healthDifference;
        _currentHealth = pHealth;
        setHealthDebug = _currentHealth;
    }

    /* Decrease the health by the damage taken.
     * pDamage shows how much damage the player has taken.
     */
    public void Damage(int pDamage)
    {
        _healthDifference = (int)_slider.value - (_currentHealth - pDamage);
        if (_healthDifference < 0) _healthDifference = -_healthDifference;
        _currentHealth -= pDamage;
        setHealthDebug = _currentHealth;
        // if (_comboMeterScript)
            // _comboMeterScript.EndCombo();
    }

    /* Set the max health of the player.
     * pMaxHealth is the given max health that player player will adapt to.
     */
    public void SetMaxHealth(float pMaxHealth)
    {
        _slider.maxValue = pMaxHealth;
        if (_slider.value > _slider.maxValue)
        {
            _slider.value = _slider.maxValue;
            _sliderValue = _slider.maxValue;
        }
    }

    public HealthType HealthType
    {
        get { return healthType; }
    }
}
