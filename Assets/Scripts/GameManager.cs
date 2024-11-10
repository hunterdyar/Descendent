using System;
using Proc;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static Action<GameState> OnGameStateChanged;
    
    private GameState _state;
    public RuntimeLevel RuntimeLevel => _runtimeLevel;
    private RuntimeLevel _runtimeLevel;
    public WorldCreator WorldCreator => worldCreator;
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
        _runtimeLevel = LevelFactory.CreateRandomValidSquareLevel(10, 20, 1, 0);
        
        worldCreator.Generate(this, _runtimeLevel);
    }
}