using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private UIPlayerHud _hud;
    
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
        if(_hud == null) return;
        
        _hud.UpdateHealthUI(_currentPlayerHealth, _maxPlayerHealth);
        _hud.UpdateWallJuiceUI(_currentWallJuice, _maxWallJuice);
    }

    public void UpdateInteractText(String text)
    {
        _hud.UpdateInteractText(text);
    }

    public void UpdateTooltipText(String text)
    {
        _hud.UpdateTooltipText(text);
    }
}
