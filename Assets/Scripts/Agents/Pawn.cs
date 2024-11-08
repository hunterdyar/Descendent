
using System;
using UnityEngine;

public class Pawn : GameAgentBase
{
    public StatBlock Stats => _stats;
    [SerializeField] StatBlock _stats;
    [SerializeField] UIStatBlockOverlay _statsUI;

    private void Start()
    {
        _statsUI?.Init(_stats);
    }

    public void Die()
    {
        Level.RemoveAgent(this);
        Destroy(gameObject);
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
