using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;

public class Level
{
    //spawn data.
    public Dictionary<Vector2Int, EnvTile> Environment => _environment;
    private readonly Dictionary<Vector2Int, EnvTile> _environment;

    public Dictionary<Vector2Int, AgentType> Agents => _initialAgents;
    private readonly Dictionary<Vector2Int, AgentType> _initialAgents;
    
    //runtime data
    private BoundsInt _bounds;

    //bidirectional dictionary
    private Dictionary<Vector2Int, GameAgentBase> _gameAgentsMap;
    private Dictionary<GameAgentBase, Vector2Int> _gameAgentsPositionLookup;

    
    public Level()
    {
        _environment = new Dictionary<Vector2Int, EnvTile>();
        _initialAgents = new Dictionary<Vector2Int, AgentType>();
        _gameAgentsMap = new Dictionary<Vector2Int, GameAgentBase>();
        _gameAgentsPositionLookup = new Dictionary<GameAgentBase, Vector2Int>();
    }

    public BoundsInt CalculateBounds()
    {
        int minX = _environment.Keys.Min(k => k.x);
        int maxX = _environment.Keys.Max(k => k.x);
        int minY = _environment.Keys.Min(k => k.y);
        int maxY = _environment.Keys.Max(k => k.y);
        _bounds = new BoundsInt(minX, minY, 0, maxX-minX, maxY-minY, 1);
        return _bounds;
    }

    public void AddGameAgent(GameAgentBase gameAgent, Vector2Int position)
    {
        if (!_gameAgentsMap.ContainsKey(position))
        {
            if (!_gameAgentsPositionLookup.ContainsKey(gameAgent))
            {
                _gameAgentsMap.Add(position, gameAgent);
                _gameAgentsPositionLookup.Add(gameAgent, position);
                return;
            }
        }
        throw new Exception("Game Agent already registered or agent at this position.");
    }

    public void MoveAgent(GameAgentBase agent, Vector2Int newPosition)
    {
        var old = _gameAgentsPositionLookup[agent];
        if (old == newPosition)
        {
            Debug.LogError("Agent already in this position!");
            return;
        }

        if (_gameAgentsMap.ContainsKey(newPosition))
        {
            Debug.LogError("Agent at this position already!");
            return;
        }

        _gameAgentsMap.Remove(old);
        _gameAgentsMap.Add(newPosition, agent);
        _gameAgentsPositionLookup[agent] = newPosition;
    }

    public void RemoveAgent(GameAgentBase agent)
    {
        var current = _gameAgentsPositionLookup[agent];

        if (!_gameAgentsPositionLookup.ContainsKey(agent))
        {
            Debug.LogError("Agent already removed?");
            return;
        }

        _gameAgentsMap.Remove(current);
        _gameAgentsPositionLookup.Remove(agent);
    }

    public Vector2Int GetPosition(GameAgentBase agent)
    {
        if (_gameAgentsPositionLookup.TryGetValue(agent, out var pos))
        {
            return pos;
        }

        throw new Exception("Can't get position, agent not in map");
    }

    public (GameAgentBase? agent, Vector2Int) GetFirstAgentOrWall(Vector2Int start, Vector2Int direction)
    {
        if (direction == Vector2Int.zero)
        {
            throw new ArgumentException("Direction cannot be zero", nameof(direction));
        }
        GameAgentBase? agent = null;
        var test = start;
        while (agent == null)
        {
            if (_environment.TryGetValue(test+direction, out var tile))
            {
                if (tile == EnvTile.Wall)
                {
                    break;
                }
            }
            else
            {
                break;
            }
            
            test = test + direction;
            
            if (_gameAgentsMap.TryGetValue(test, out agent))
            {
                break;
            }
        }

        return (agent, test);
    }

    public Vector2Int GetRandomPosition(EnvTile ofType)
    {
        var positions = _environment.Where(x=>x.Value == ofType).ToArray();
        return positions[UnityEngine.Random.Range(0, positions.Length)].Key;
    }
}
