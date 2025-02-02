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
    [SerializeField, Tooltip("Set the scriptable object that this script should look at.")]
    private PlayerObject playerObject;
    private MovementControllerScript _movementScript;

    private void Start()
    {
        healthBarScript.SetMaxHealth(maxHealth);
        _attackScript = GetComponent<AttackScript>();
        _movementScript = GetComponent<MovementControllerScript>();
        GameManager.onNewAttack += ReadyUpForNextAttack;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Hitbox") && healthBarScript != null 
            && healthBarScript.HealthType != HealthType.Invincible && other.transform != transform.GetChild(0) && other.transform != transform.GetChild(1) && playerObject.playerState != PlayerState.Blocking)
        {
            playerObject.playerState = PlayerState.Stunned;
            AttackHitboxScript hitboxScript = other.GetComponent<AttackHitboxScript>();
            healthBarScript.Damage(hitboxScript.damage);
            if (_attackScript != null)
                _attackScript.StunPlayer(hitboxScript.stun);
            if (_movementScript != null)
            {
                _movementScript.ApplyKnockback(hitboxScript.knockbackTowardsPlayer, hitboxScript.knockbackUp,
                    Mathf.RoundToInt(other.transform.rotation.y) == 1);
            }
            // _alreadyDamaged = true;
            GameManager.instance.GetOtherPlayerGameObject(gameObject).GetComponent<ComboMeterScript>().UpdateComboMeter(hitboxScript.damage);
        }
    }

    /* Ready up for the next attack. */
    private void ReadyUpForNextAttack()
    {
        _alreadyDamaged = false;
    }
}