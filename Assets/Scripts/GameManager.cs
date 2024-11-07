using System;
using Proc;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static Action<GameState> OnGameStateChanged;
    
    private GameState _state;
    public Level Level => _level;
    private Level _level;
    [SerializeField] WorldCreator worldCreator;
   
    private void ChangeState(GameState newState)
    {
        if (_state != newState)
        {
            _state = newState;
            OnGameStateChanged?.Invoke(newState);
        }
    }

    void Start()
    {
        ChangeState(GameState.Generating);
        _level = LevelFactory.CreateCircleLevel(10);
        
        worldCreator.Generate(this, _level);
    }
}