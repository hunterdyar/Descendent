using System;
using UnityEngine;

public class WorldCreator : MonoBehaviour
{
    [Header("Tiles")]
    public GameObject _floorTilePrefab;
    public GameObject _wallTilePrefab;
    [Header("Agents")]
    public GameObject _playerPrefab;
    
    
    private Grid _grid;

    private void Awake()
    {
        _grid = GetComponent<Grid>();
    }


    public void Generate(Level level)
    {
        foreach (var envkvp in level.Environment)
        {
            switch (envkvp.Value)
            {
                case EnvTile.Floor:
                    Spawn(_floorTilePrefab, envkvp.Key);
                    continue;
                case EnvTile.Wall:
                    Spawn(_wallTilePrefab, envkvp.Key);
                    continue;
            }
        }
        
        foreach (var envkvp in level.Agents)
        {
            switch (envkvp.Value.AgentType)
            {
                case AgentType.Player:
                    SpawnAgent(_playerPrefab, envkvp.Key);
                    continue;
            }
        }

        SetCamera(level);
        
    }

    private void SetCamera(Level level)
    {
        var bounds = level.CalculateBounds();
        var centerGridPos = new Vector3Int((int)bounds.center.x, (int)bounds.center.y,0);
        var cam = Camera.main;
        var center = _grid.CellToWorld(centerGridPos);
        cam.transform.position = new Vector3(center.x, center.y, cam.transform.position.z);
    }

    private void Spawn(GameObject prefab, Vector2Int gridPos)
    {
        var pos = new Vector3Int(gridPos.x,gridPos.y,0);
        Instantiate(prefab,_grid.CellToWorld(pos), Quaternion.identity, transform);
    }
    private void SpawnAgent(GameObject prefab, Vector2Int gridPos)
    {
        var pos = new Vector3Int(gridPos.x,gridPos.y,0);
        var gen = Instantiate(prefab,_grid.CellToWorld(pos), Quaternion.identity, transform);
        var agent = gen.GetComponent<GameAgentBase>();
        agent.Init(gridPos);
    }
}
