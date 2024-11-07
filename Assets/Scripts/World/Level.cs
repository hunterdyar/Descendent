using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;

public class Level
{
    public Dictionary<Vector2Int, EnvTile> Environment => _environment;
    private Dictionary<Vector2Int, EnvTile> _environment;
    
    public Dictionary<Vector2Int, Agent> Agents => _agents;
    private Dictionary<Vector2Int, Agent> _agents;

    private BoundsInt _bounds;
    
    public Level()
    {
        _environment = new Dictionary<Vector2Int, EnvTile>();
        _agents = new Dictionary<Vector2Int, Agent>();
    }

    public BoundsInt CalculateBounds()
    {
        var minX = _environment.Keys.Min(k => k.x);
        var maxX = _environment.Keys.Max(k => k.x);
        var minY = _environment.Keys.Min(k => k.y);
        var maxY = _environment.Keys.Max(k => k.y);
        _bounds = new BoundsInt(minX, minY, 0, maxX-minX, maxY-minY, 1);
        return _bounds;
    }
}
