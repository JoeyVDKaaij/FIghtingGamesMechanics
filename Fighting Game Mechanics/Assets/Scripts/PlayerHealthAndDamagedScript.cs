using System;
using UnityEngine;

public class PlayerHealthAndDamagedScript : MonoBehaviour
{
    [Header("Health Settings")] 
    [SerializeField, Tooltip("Set the character Health."), Min(1)]
    private int maxHealth = 100;
    [SerializeField, Tooltip("Set the healthbar script.")]
    private HealthBarScript healthBarScript;

    private AttackScript _attackScript;
    private bool _alreadyDamaged = false;
    private MovementControllerScript _movementScript;
    private ComboMeterScript _comboMeterScript;

    private void Start()
    {
        healthBarScript.SetMaxHealth(maxHealth);
        _attackScript = GetComponent<AttackScript>();
        _movementScript = GetComponent<MovementControllerScript>();
        GameManager.onNewAttack += ReadyUpForNextAttack;
        _comboMeterScript = GetComponent<ComboMeterScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Hitbox") && !_alreadyDamaged && healthBarScript != null 
            && healthBarScript.HealthType != HealthType.Invincible && other.transform != transform.parent && !_attackScript.Blocking)
        {
            AttackHitboxScript hitboxScript = other.GetComponent<AttackHitboxScript>();
            if (hitboxScript.immunePlayers != gameObject)
            {
                healthBarScript.Damage(hitboxScript.damage);
                if (_attackScript != null)
                    _attackScript.StunPlayer(hitboxScript.stun);
                if (_movementScript != null)
                {
                    _movementScript.ApplyKnockback(hitboxScript.knockbackTowardsPlayer, hitboxScript.knockbackUp,
                        Mathf.RoundToInt(other.transform.rotation.y) == 1);
                }
                _alreadyDamaged = true;
                GameManager.instance.GetOtherPlayerDetails(gameObject).GetComponent<ComboMeterScript>().UpdateComboMeter(hitboxScript.damage);
            }
        }
    }

    /* Ready up for the next attack. */
    private void ReadyUpForNextAttack()
    {
        _alreadyDamaged = false;
    }
}