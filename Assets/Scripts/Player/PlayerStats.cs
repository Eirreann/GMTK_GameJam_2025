using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int _currentWallJuice;
    [SerializeField] private int _maxWallJuice;

    [SerializeField] private int _currentPlayerHealth;
    [SerializeField] private int _maxPlayerHealth;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int DrainJuice(int drain)
    {
        _currentWallJuice -= drain;
        return _currentWallJuice;
    }

    public int InjectJuice(int inject)
    {
        _currentWallJuice += inject;
        return _currentWallJuice;
    }

    public int DamagePlayer(int damage)
    {
        _currentPlayerHealth -= damage;
        return _currentPlayerHealth;
    }

    public int HealPlayer(int heal)
    {
        _currentPlayerHealth += heal;
        return _currentPlayerHealth;
    }

    public void ReplenishAllJuice()
    {
        _currentWallJuice = _maxWallJuice;
    }
    
    public void ReplenishAllHealth()
    {
        _currentPlayerHealth = _maxPlayerHealth;
    }
}
