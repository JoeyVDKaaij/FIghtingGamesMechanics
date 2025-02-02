using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObjects/Player")]
public class PlayerObject : ScriptableObject
{
	[Header("Player Settings")]
	[Min(1)]
	public int maxHealth;
    public int health;
	public bool ai;
    
    
	[Header("Changing Player Values Settings")]
	public PlayerState playerState = PlayerState.Alive;
	public bool comboActive;

	private void Update()
	{
		if (health <= 0)
		{
			playerState = PlayerState.Dead;
		}
	}

	public void ResetPlayer()
	{
		health = maxHealth;
		comboActive = false;
		playerState = PlayerState.Alive;
	}
}