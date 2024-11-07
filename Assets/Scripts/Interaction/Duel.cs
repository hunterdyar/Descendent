
public class Duel
{
    /// <summary>
    /// Pawn A Attacks First
    /// </summary>
    private Pawn _pawnA;
    private Pawn _pawnB;

    public Duel(Pawn instigating, Pawn defending)
    {
        _pawnA = instigating;
        _pawnB = defending;
    }

    public void Resolve()
    {
        var aAttackPower = _pawnA.Stats.AttackPower;
        if (aAttackPower >= _pawnB.Stats.Health)
        {
            _pawnB.Die();
            _pawnA.OnVictory(_pawnB);
            return;
        }
        _pawnB.TakeDamage(_pawnA.Stats.AttackPower);
        
        //CounterAttack
        var bAttackPower = _pawnB.Stats.AttackPower;
        if (bAttackPower >= _pawnA.Stats.Health)
        {
            _pawnA.Die();
            _pawnA.OnVictory(_pawnB);
            return;
        }
        _pawnA.TakeDamage(_pawnB.Stats.AttackPower);
        
        
    }
}
