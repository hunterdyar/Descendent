using System;
using UnityEngine;

public class WorldCreator : MonoBehaviour
{
    [Header("Tiles")]
    public GameObject _floorTilePrefab;
    public GameObject _wallTilePrefab;
    [Header("Agents")]
    public GameObject _playerPrefab;
    public GameObject _enemyPrefab;
    public GameObject _exitPrefab;
    
    private Grid _grid;
    public Grid Grid => _grid;

    private void Awake()
    {
        _grid = GetComponent<Grid>();
    }

    public Vector3 GridToWorld(Vector2Int pos)
    {
        return _grid.CellToWorld(new Vector3Int(pos.x, pos.y, 0));
    }

    public void Generate(GameManager gameManager, RuntimeLevel runtimeLevel)
    {
        ClearCurrent();
        if (runtimeLevel.Environment == null)
        {
            return;
        }
        foreach (var envkvp in runtimeLevel.Environment)
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
        
        foreach (var envkvp in runtimeLevel.Agents)
        {
            switch (envkvp.Value)
            {
                case AgentType.Player:
                    SpawnAgent(_playerPrefab, envkvp.Key, gameManager);
                    continue;
                case AgentType.Enemy:
                    //how do we randomly decide how stronk they are?
                    SpawnAgent(_enemyPrefab, envkvp.Key, gameManager);
                    continue;
                case AgentType.Exit:
                    SpawnAgent(_exitPrefab, envkvp.Key, gameManager);
                    continue;
            }
        }

        SetCamera(runtimeLevel);
        
    }

    private void ClearCurrent()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void SetCamera(RuntimeLevel runtimeLevel)
    {
        var bounds = runtimeLevel.CalculateBounds();
        var centerGridPos = new Vector3Int((int)bounds.center.x, (int)bounds.center.y,0);
        var cam = Camera.main;
        var center = _grid.CellToWorld(centerGridPos);
        var dsizeint = bounds.max - bounds.min;
        var dsize = _grid.CellToWorld(dsizeint);
        cam.transform.position = new Vector3(center.x, center.y, cam.transform.position.z);
        cam.orthographicSize = dsize.magnitude / 2;

    }

    private void Spawn(GameObject prefab, Vector2Int gridPos)
    {
        var pos = new Vector3Int(gridPos.x,gridPos.y,0);
        Instantiate(prefab,_grid.CellToWorld(pos), Quaternion.identity, transform);
    }
    private void SpawnAgent(GameObject prefab, Vector2Int gridPos, GameManager gameManager)
    {
        var pos = new Vector3Int(gridPos.x,gridPos.y,0);
        var gen = Instantiate(prefab,_grid.CellToWorld(pos), Quaternion.identity, transform);
        var agent = gen.GetComponent<GameAgentBase>();
        agent.Init(gameManager, gridPos);
    }


    
}
