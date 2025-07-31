using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float _currentWallJuice;
    [SerializeField] private float _maxWallJuice;

    [SerializeField] private float _currentPlayerHealth;
    [SerializeField] private float _maxPlayerHealth;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float DrainJuice(float drain)
    {
        _currentWallJuice -= drain;

        return _currentWallJuice;
    }

    public float InjectJuice(float inject)
    {
        _currentWallJuice += inject;
        
        return _currentWallJuice;
    }

    public float DamagePlayer(float damage)
    {
        _currentPlayerHealth -= damage;
        
        return _currentPlayerHealth;
    }

    public float HealPlayer(float heal)
    {
        _currentPlayerHealth += heal;
        
        return _currentPlayerHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
