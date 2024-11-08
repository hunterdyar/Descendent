
using System;
using UnityEngine;

[System.Serializable]
public class StatBlock
{
    public int Health => _health;
    [SerializeField] private int _health;
    public int AttackPower => _attackPower;
    public Action<int> OnHealthChange;
    public Action<int> OnAttackChange;
    
    [SerializeField] private int _attackPower;

    public void LoseHealth(int damage)
    {
        _health -= damage;
        OnHealthChange?.Invoke(_health);
    }

    public void LevelUp()
    {
        _health++;
        _attackPower++;
        OnHealthChange?.Invoke(_health);
        OnAttackChange?.Invoke(_attackPower);
    }
}
