using System;
using TMPro;
using UnityEngine;

public class UIStatBlockOverlay : MonoBehaviour
{
    public TMP_Text _health;
    public TMP_Text _attack;
    private StatBlock _stat;
    
    public void Init(StatBlock stats)
    {
        _stat = stats;
        stats.OnHealthChange += UpdateHealth;
        stats.OnAttackChange += UpdateAttack;
        UpdateHealth(stats.Health);
        UpdateAttack(stats.AttackPower);
    }
    
    void UpdateHealth(int health)
    {
        _health.text = health.ToString();
    }

    void UpdateAttack(int attack)
    {
        _attack.text = attack.ToString();
    }

    private void OnDestroy()
    {
        if (_stat != null)
        {
            _stat.OnAttackChange -= UpdateAttack;
            _stat.OnHealthChange -= UpdateHealth;
        }
    }
}
