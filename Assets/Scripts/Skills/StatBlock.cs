
using System;
using UnityEngine;

[System.Serializable]
public class StatBlock
{
    public Action<int> OnHealthChange;
    public Action<int> OnAttackChange;
    public Action<int> OnExperienceChange;
    
    public int Health => _health;
    [SerializeField] private int _health = 5;
    public int AttackPower => _attackPower;
    [SerializeField] private int _attackPower = 1;
    
    public int Experience => _experience;
    [SerializeField] private int _experience = 1;
    
    public void LoseHealth(int damage)
    {
        _health -= damage;
        OnHealthChange?.Invoke(_health);
    }

    public void GainExperience(int experience)
    {
        _experience += experience;
        OnExperienceChange?.Invoke(_experience);
    }
    
    public void LevelUp()
    {
        _health++;
        _attackPower++;
        OnHealthChange?.Invoke(_health);
        OnAttackChange?.Invoke(_attackPower);
    }
}
