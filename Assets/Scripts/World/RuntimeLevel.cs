using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Random = UnityEngine.Random;

public class RuntimeLevel
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

    public RuntimeLevel()
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

    public (GameAgentBase agent, Vector2Int) GetFirstAgentOrWall(Vector2Int start, Vector2Int direction)
    {
        if (direction == Vector2Int.zero)
        {
            throw new ArgumentException("Direction cannot be zero", nameof(direction));
        }
        GameAgentBase agent = null;
        var test = start;
        while (true)
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

    public void AddProtoLevel(ProtoLevel pl, Vector2Int offset)
    {
        for(int x = 0;x<pl.Width;x++)
        {
            for (int y = 0; y < pl.Height; y++)
            {
                var pt = pl.Tiles[x, y];
                if (pt == PTile.Floor)
                {
                    _environment.Add(new Vector2Int(x,y)+offset, EnvTile.Floor);
                }else if (pt == PTile.Wall)
                {
                    _environment.Add(new Vector2Int(x, y)+offset, EnvTile.Wall);
                }else if (pt == PTile.Exit)
                {
                    _environment.Add(new Vector2Int(x, y)+offset, EnvTile.Floor);
                    _initialAgents.Add(new Vector2Int(x, y)+offset, AgentType.Exit);
                }else
                {
                    throw new Exception($"Unknown tile type {pt}");
                }
            }
        }

    }
    public static RuntimeLevel FromProtoLevel(ProtoLevel pl, int createEnemiesAtRandomFloor=0)
    {
        var rl = new RuntimeLevel();
        for(int x = 0;x<pl.Width;x++)
        {
            for (int y = 0; y < pl.Height; y++)
            {
                var pt = pl.Tiles[x, y];
                if (pt == PTile.Floor)
                {
                    rl._environment.Add(new Vector2Int(x,y), EnvTile.Floor);
                }else if (pt == PTile.Wall)
                {
                    rl._environment.Add(new Vector2Int(x, y), EnvTile.Wall);
                }else if (pt == PTile.Exit)
                {
                    rl._environment.Add(new Vector2Int(x, y), EnvTile.Floor);
                    rl._initialAgents.Add(new Vector2Int(x, y), AgentType.Exit);
                }else
                {
                    throw new Exception($"Unknown tile type {pt}");
                }
            }
        }
        
        rl._initialAgents.Add(pl.PlayerStartLocation(), AgentType.Player);
        
        for (int i = 0; i < createEnemiesAtRandomFloor; i++)
        {
            var ePos= pl.GetRandomTile(PTile.Floor);
            int escape = 1000;
            while (rl._initialAgents.ContainsKey(ePos) && escape > 0)
            {
                ePos = pl.GetRandomTile(PTile.Floor);
                escape--;
            }
            rl._initialAgents.Add(ePos, AgentType.Enemy);
        }
        
        //add outer ring of walls?

        return rl;
    }
}
