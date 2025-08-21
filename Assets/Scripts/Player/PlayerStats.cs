using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Player
{
    public class PlayerStats : MonoBehaviour
    {

        [SerializeField] private PlayerStat health;
        [SerializeField] private PlayerStat wallJuice;
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        [Serializable]
        private struct PlayerStat
        {
            public int current;
            public int max;
        }

        public IEnumerator JOTEffect(float frequency, int amount)
        {
            if(wallJuice.current > 0 )DrainJuice(amount);
            yield return new WaitForSeconds(frequency);
        
            StartCoroutine(JOTEffect(frequency, amount));
        }

        public int DrainJuice(int drain)
        {
            wallJuice.current -= drain;
            if(wallJuice.current < 0)
                wallJuice.current = 0;
        
            _updateUI();
            return wallJuice.current;
        }

        public int InjectJuice(int inject)
        {
            wallJuice.current += inject;
            if(wallJuice.current > wallJuice.max)
                wallJuice.current = wallJuice.max;
            _updateUI();
            return wallJuice.current;
        }

        public int DamagePlayer(int damage)
        {
            health.current -= damage;
            _updateUI();
            return health.current;
        }

        public int HealPlayer(int heal)
        {
            health.current += heal;
            _updateUI();
            return health.current;
        }

        public void IncreaseMaxJuice(int increase)
        {
            wallJuice.max += increase;
            _updateUI();
        }

        public void ReplenishAllJuice()
        {
            wallJuice.current = wallJuice.max;
            _updateUI();
        }
    
        public void ReplenishAllHealth()
        {
            health.current = health.max;
            _updateUI();
        }

        private void _updateUI()
        {
            GameManager.Instance.Player.HUD.UpdateHealthUI(health.current, health.max);
            GameManager.Instance.Player.HUD.UpdateWallJuiceUI(wallJuice.current, wallJuice.max);
        }
    }

}