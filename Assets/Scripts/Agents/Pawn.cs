
using System;
using UnityEngine;

public class Pawn : GameAgentBase
{
    public StatBlock Stats => _stats;
    [SerializeField] StatBlock _stats;
    [SerializeField] UIStatBlockOverlay _statsUI;

    protected virtual void Start()
    {
        _statsUI?.Init(_stats);
        
    }

    public void Die()
    {
        base.Remove();
    }

    public void TakeDamage(int damage)
    {
        _stats.LoseHealth(damage);
    }

    public virtual void OnVictory(Pawn pawnB)
    {
        _stats.LevelUp();
    }
}
