using System;
using Player;
using UnityEngine;
using Utilities;

public class PlayerController : MonoBehaviour, IDamageable
{
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public PlayerStats playerStats;
    [HideInInspector] public PlayerInteractions playerInteractions;
    [HideInInspector] public Player_WallHandler wallHandler;
    
    public void Awake() 
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerStats = GetComponent<PlayerStats>();
        playerInteractions = GetComponent<PlayerInteractions>();
        wallHandler = GetComponent<Player_WallHandler>();
    }

    public void Reset()
    {
        wallHandler.ResetWalls();
        playerMovement.ResetPlayer();
        
        playerStats.ReplenishAllHealth();
        playerStats.ReplenishAllJuice();
        // TODO: Fade in/out?
    }

    public void TakeDamage(int damage)
    {
        var health = playerStats.DamagePlayer(damage);
        // TODO: Trigger some sort of effect on the player?
        if (health <= 0)
        {
            // TODO: Game Over logic
        }
    }
}
