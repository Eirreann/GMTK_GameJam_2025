using System;
using Game;
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
        if(_currentWallJuice < 0)
            _currentWallJuice = 0;
        
        _updateUI();
        return _currentWallJuice;
    }

    public int InjectJuice(int inject)
    {
        _currentWallJuice += inject;
        if(_currentWallJuice > _maxWallJuice)
            _currentWallJuice = _maxWallJuice;
        _updateUI();
        return _currentWallJuice;
    }

    public int DamagePlayer(int damage)
    {
        GameManager.Instance._audioManager.OnTakeDamage();
        
        _currentPlayerHealth -= damage;
        _updateUI();
        return _currentPlayerHealth;
    }

    public int HealPlayer(int heal)
    {
        _currentPlayerHealth += heal;
        _updateUI();
        return _currentPlayerHealth;
    }

    public void IncreaseMaxJuice(int increase)
    {
        _maxWallJuice += increase;
        _updateUI();
    }

    public void ReplenishAllJuice()
    {
        _currentWallJuice = _maxWallJuice;
        _updateUI();
    }
    
    public void ReplenishAllHealth()
    {
        _currentPlayerHealth = _maxPlayerHealth;
        _updateUI();
    }

    private void _updateUI()
    {
        GameManager.Instance.Player.HUD.UpdateHealthUI(_currentPlayerHealth, _maxPlayerHealth);
        GameManager.Instance.Player.HUD.UpdateWallJuiceUI(_currentWallJuice, _maxWallJuice);
    }
}
