
using UnityEngine;

[System.Serializable]
public class StatBlock
{
    public int Health => _health;
    [SerializeField] private int _health;
    public int AttackPower => _attackPower;
    [SerializeField] private int _attackPower;

    public void LoseHealth(int damage)
    {
        _health -= damage;
    }

    public void LevelUp()
    {
        _health++;
        _attackPower++;
    }
}
