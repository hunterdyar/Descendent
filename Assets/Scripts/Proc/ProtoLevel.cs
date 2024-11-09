using System;
using System.Collections.Generic;
using UnityEngine;

public class ProtoLevel
{
	public static readonly PDir[] Directions = {PDir.Left, PDir.Right, PDir.Up, PDir.Down};
	//not writing 'Ptile.' as much.
	private const PTile Floor = PTile.Floor;
	private const PTile Wall = PTile.Wall;

	public PTile[,] Tiles => _tiles;
	private PTile[,] _tiles;
	public int Width => _width;
	private int _width;
	public int Height => _height;
	private int _height;
	public bool Solved { get; private set; }
	private Vector2Int _playerStart;
	private Vector2Int _playerLoc;
	public List<PDir> Moves = new List<PDir>();
	public List<Vector2Int> Visited = new List<Vector2Int>();
	public Vector2Int PlayerStartLocation()
	{
		return _playerStart;
	}

	public bool MoveDir(PDir dir)
	{
		if (Solved)
		{
			throw new Exception("Trying to move in already solved protoLevel");
		}

		var delta = PDirToXY(dir);
		var c = RecursiveStepPlayerInDir(0,delta.x, delta.y);
		if (Visited.Contains(_playerLoc))
		{
			return false;
		}
		Visited.Add(_playerLoc);
		Moves.Add(dir);
		return c > 0;
	}

	public ProtoLevel Clone()
	{
		return new ProtoLevel()
		{
			_width = this._width,
			_height = this._height,
			_playerLoc = this._playerLoc,
			_playerStart = _playerStart,
			_tiles = this._tiles,
			Moves = new List<PDir>(this.Moves),
			Visited = new List<Vector2Int>(this.Visited),
		};
	}

	public int RecursiveStepPlayerInDir(int c, int dx, int dy)
	{
		if (dx == 0 && dy == 0)
		{
			throw new Exception("Invalid Delta");
		}
		
		var next = _playerLoc + new Vector2Int(dx, dy);
		if (next.x < 0 || next.x >= _width || next.y < 0 || next.y >= _height)
		{
			return c;
		}
		var nextTile = _tiles[next.x, next.y];
		if (nextTile == Wall)
		{
			return c;
		}

		if (nextTile == PTile.Exit)
		{
			_playerLoc = next;
			Solved = true;
			return c;
		}

		if (nextTile == Floor)
		{
			_playerLoc = next;
			return RecursiveStepPlayerInDir(c+1, dx, dy);
		}
		

		return c;
	}
	
	public Vector2Int ChangeRandomTile(PTile from, PTile to)
	{
		if (from == to)
		{
			return -Vector2Int.one;
		}

		int escape = _width * _height * 3;
		while (escape > 0)
		{
			int x = UnityEngine.Random.Range(0, _width);
			int y = UnityEngine.Random.Range(0, _height);
			if (_tiles[x, y] == from)
			{
				_tiles[x, y] = to;
				return new Vector2Int(x, y);
			}

			escape--;
		}
		
		//todo: change to TryOut pattern and catch errors?
		throw new Exception("Overflow Exception in ChangeRandomTile. Couldn't find tile");
	}

	public static (int x, int y) PDirToXY(PDir dir)
	{
		switch (dir)
		{
			case PDir.Down:
				return (0, -1);
			case PDir.Up:
				return (0, 1);
			case PDir.Left:
				return (-1, 0);
			case PDir.Right:
				return (1, 0);
			default:
				throw new Exception("Invalid PDir");
		}
	}
	public static ProtoLevel CreateRandom(int width, int height, int walls, int exits)
	{
		ProtoLevel pLevel = new ProtoLevel();
		pLevel._width = walls;
		pLevel._height = height;
		pLevel._tiles = new PTile[width, height];
		for (int i = 0; i < walls; i++)
		{
			pLevel.ChangeRandomTile(Floor, Wall);
		}

		for (int i = 0; i < exits; i++)
		{
			pLevel.ChangeRandomTile(Floor, PTile.Exit);
		}

		pLevel._playerLoc = pLevel.GetRandomTile(Floor);
		pLevel._playerStart = pLevel._playerLoc;
		return pLevel;
	}

	private Vector2Int GetRandomTile(PTile floor)
	{
		int escape = _width * _height * 3;
		while (escape > 0)
		{
			int x = UnityEngine.Random.Range(0, _width);
			int y = UnityEngine.Random.Range(0, _height);
			if (_tiles[x, y] == floor)
			{
				return new Vector2Int(x, y);
			}

			escape--;
		}
		throw new Exception("Overflow Exception in GetRandomTile. Couldn't find tile");
	}

	public static bool IsOppositeDir(PDir a, PDir b)
	{
		switch (a)
		{
			case PDir.Up:
				return b == PDir.Down;
			case PDir.Down :
				return b == PDir.Up;
			case PDir.Left:
				return b == PDir.Right;
			case PDir.Right:
				return b == PDir.Left;
		}
		throw new Exception("Invalid Direction");
	}
}
