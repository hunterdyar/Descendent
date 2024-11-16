using UnityEngine;

public class GameAgentBase : MonoBehaviour
{
    protected GameManager GameManager => _gameManager;
    private GameManager _gameManager;
    protected RuntimeLevel RuntimeLevel => _gameManager.RuntimeLevel;
    public Vector2Int CurrentPos => _gameManager.RuntimeLevel.GetPosition(this);
    public void Init(GameManager gameManager, Vector2Int gridPos)
    {
        _gameManager = gameManager;
        gameManager.RuntimeLevel.AddGameAgent(this,gridPos);
    }

    public void MoveAgent(Vector2Int pos)
    {
        RuntimeLevel.MoveAgent(this, pos);
        transform.position = GameManager.WorldCreator.GridToWorld(pos);
    }

    public void Remove()
    {
        RuntimeLevel.RemoveAgent(this);
        Destroy(gameObject);
    }
}
