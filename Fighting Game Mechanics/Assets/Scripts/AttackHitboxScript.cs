using UnityEngine;

public class AttackHitboxScript : MonoBehaviour
{
    [Header("Hitbox Settings")] 
    [Tooltip("Set the damage output")]
    public int damage;
    [Tooltip("Set how much knockback the opponent gets")]
    public float knockbackUp = 0;
    [Tooltip("Set how much knockback the opponent gets")]
    public float knockbackTowardsPlayer = 5;
    [Tooltip("Set how many frames the opponent gets stunned for")]
    public int stun;
    [Tooltip("Set who are immune to this attack.")]
    public GameObject immunePlayers;
}