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
    private BSPNode _lastNode;
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
        NewGame();
    }

    void NewGame()
    {
        ChangeState(GameState.Generating);
        //_runtimeLevel = LevelFactory.CreateRandomValidSquareLevel(10, 20, 1, 2);
        //node just used for gizmo drawing.
        _runtimeLevel = LevelFactory.CreateDungeonLevels(20,2, out _lastNode);

        worldCreator.Generate(this, _runtimeLevel);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NewGame();
        }
    }

    private void OnDrawGizmos()
    {
        if (_lastNode != null)
        {
            _lastNode.DrawGizmos(transform.localToWorldMatrix);
        }
    }
}