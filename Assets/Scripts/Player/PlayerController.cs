using System;
using Game;
using Player;
using UnityEngine;
using Utilities;

public class PlayerController : MonoBehaviour, IDamageable
{
    public UIPlayerHud HUD;
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

    private void Start()
    {
        HUD.Fade(false);
    }

    public void Reset()
    {
        wallHandler.ResetWalls();
        playerMovement.ResetPlayer();
        
        playerStats.ReplenishAllHealth();
        playerStats.ReplenishAllJuice();
        HUD.Fade(false);
    }

    public void TakeDamage(int damage)
    {
        var health = playerStats.DamagePlayer(damage);
        HUD.DamageFlash();
        
        AudioManager.Instance.OnTakeDamage();
        
        if (health <= 0)
        {
            GameManager.Instance.ResetLevel();
        }
    }
}
