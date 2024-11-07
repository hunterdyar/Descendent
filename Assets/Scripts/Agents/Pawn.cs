
using UnityEngine;

public class Pawn : GameAgentBase
{
    public StatBlock Stats => _stats;
    [SerializeField] StatBlock _stats;

    public void Die()
    {
        Level.RemoveAgent(this);
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        _stats.LoseHealth(damage);
    }

    public void OnVictory(Pawn pawnB)
    {
        _stats.LevelUp();
    }
}
