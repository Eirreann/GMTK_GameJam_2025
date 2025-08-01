using UnityEngine;
using Utilities;

public class PlayerController : MonoBehaviour, IDamageable
{
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public PlayerStats playerStats;
    
    public void Awake() 
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerStats = GetComponent<PlayerStats>();
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
