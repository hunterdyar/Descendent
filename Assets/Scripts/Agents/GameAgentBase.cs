using UnityEngine;

public class GameAgentBase : MonoBehaviour
{
    private GameManager _gameManager;
    
    public void Init(GameManager gameManager, Vector2Int gridPos)
    {
        _gameManager = gameManager;
        gameManager.Level.AddGameAgent(this,gridPos);
    }
}
