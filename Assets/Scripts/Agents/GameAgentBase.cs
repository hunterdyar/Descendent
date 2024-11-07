using UnityEngine;

public class GameAgentBase : MonoBehaviour
{
    protected GameManager GameManager => _gameManager;
    private GameManager _gameManager;
    protected Level Level => _gameManager.Level;
    protected Vector2Int CurrentPos => _gameManager.Level.GetPosition(this);
    public void Init(GameManager gameManager, Vector2Int gridPos)
    {
        _gameManager = gameManager;
        gameManager.Level.AddGameAgent(this,gridPos);
    }

    public void MoveAgent(Vector2Int pos)
    {
        Level.MoveAgent(this, pos);
        transform.position = GameManager.WorldCreator.GridToWorld(pos);
    }
}
