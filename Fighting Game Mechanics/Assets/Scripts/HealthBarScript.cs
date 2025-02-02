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
    [SerializeField, Tooltip("Set player object.")]
    private PlayerObject playerObject = null;
    
    private float _sliderValue = 100;
    private int _healthDifference = 100;
    private float _timer = 0;

    private Slider _slider;
    private ComboMeterScript _comboMeterScript;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.maxValue = playerObject.maxHealth;
        _slider.value = playerObject.maxHealth;

        if (healthbarText != null)
            healthbarText.SetText($"{(int)_slider.value} / {playerObject.maxHealth}");

        _comboMeterScript = GetComponent<ComboMeterScript>();
    }

    private void Update()
    {
        UpdateSlider();

        if (setHealthDebug != playerObject.health)
        {
            UpdateHealth(setHealthDebug);
        }

        if (healthType == HealthType.Healing)
        {
            _timer += Time.deltaTime;
            if (_timer >= timeUntilHealthRegerates)
            {
                playerObject.health++;
                playerObject.health = (int)Math.Clamp(playerObject.health, 0, playerObject.maxHealth);
                setHealthDebug = playerObject.health;
                _timer = 0;
            }
        }

        _slider.value = _sliderValue;
    }

    /* Updates the slider depending on the health that the player has. */
    private void UpdateSlider()
    {
        if (_slider.value < playerObject.health)
        {
            _sliderValue += _healthDifference * healthbarSpeed;

            _sliderValue = Mathf.Clamp(_sliderValue, playerObject.health - _healthDifference, playerObject.health);
            
            if (healthbarText != null)
                healthbarText.SetText($"{(int)_sliderValue} / {playerObject.maxHealth}");
        }
        else if (_slider.value > playerObject.health)
        {
            _sliderValue -= _healthDifference * healthbarSpeed;

            if (playerObject.health <= 0)
            {
                if (healthType == HealthType.Reset)
                {
                    playerObject.health = (int)playerObject.maxHealth;
                    _slider.value = (int)playerObject.maxHealth;
                }
                else
                {
                    playerObject.health = 0;
                    _sliderValue = Mathf.Clamp(_sliderValue, playerObject.health, playerObject.health + _healthDifference);
                }
            }
            
            if (healthbarText != null)
                healthbarText.SetText($"{(int)_sliderValue} / {playerObject.maxHealth}");
        }
    }

    /* Updates how much health the player currently has.
     * pHealth is the amount of health the player should now have.
     */
    public void UpdateHealth(int pHealth)
    {
        _healthDifference = (int)_slider.value - playerObject.health + (playerObject.health - pHealth);
        if (_healthDifference < 0) _healthDifference = -_healthDifference;
        playerObject.health = pHealth;
        setHealthDebug = playerObject.health;
    }

    /* Decrease the health by the damage taken.
     * pDamage shows how much damage the player has taken.
     */
    public void Damage(int pDamage)
    {
        _healthDifference = (int)_slider.value - (playerObject.health - pDamage);
        if (_healthDifference < 0) _healthDifference = -_healthDifference;
        playerObject.health -= pDamage;
        setHealthDebug = playerObject.health;
        // if (_comboMeterScript)
            // _comboMeterScript.EndCombo();
    }

    /* Set the max health of the player.
     * pMaxHealth is the given max health that player player will adapt to.
     */
    public void SetMaxHealth(float pMaxHealth)
    {
        playerObject.maxHealth = (int)pMaxHealth;
        if (_slider.value > playerObject.maxHealth)
        {
            _slider.value = playerObject.maxHealth;
            _sliderValue = playerObject.maxHealth;
        }
    }

    public HealthType HealthType
    {
        get { return healthType; }
    }
}
