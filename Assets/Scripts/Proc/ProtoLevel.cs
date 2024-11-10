using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

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
		pLevel._width = width;
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

	public Vector2Int GetRandomTile(PTile floor)
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

	public static ProtoLevel CreateRandomStampLevel(int width, int height)
	{
		ProtoLevel pLevel = new ProtoLevel();
		pLevel._width = width;
		pLevel._height = height;
		pLevel._tiles = new PTile[width, height];
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				pLevel._tiles[x, y] = Wall;
			}
		}
		
		for (int i = 0; i < 11; i++)
		{
			pLevel.StampWalk(Random.Range(5,10));
		}

		int c = Random.Range(0, 2);
		for (int i = 0; i <c; i++)
		{
			pLevel.StampRect(Floor,4);
		}

		c = Random.Range(5, 8);
		for (int i = 0; i < c; i++)
		{
			pLevel.StampRect(Floor,3);
		}

		c = Random.Range(7, 9);
		for (int i = 0; i < c; i++)
		{
			pLevel.StampRect(Floor,2);
		}

		for (int i = 0; i < 3; i++)
		{
			pLevel.StampRect(Wall,2);
		}

		pLevel._playerStart = pLevel.GetRandomTile(Floor);
		
		//pick a better exit for the floor with walk path.
		pLevel.ChangeRandomTile(Floor, PTile.Exit);
		//remove dead ends.
		return pLevel;
		
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

	public void StampRect(PTile tile, int maxSize)
	{
		int startX = UnityEngine.Random.Range(0, _width-maxSize-1);
		int startY = UnityEngine.Random.Range(0, _height-maxSize-1);
		int width = UnityEngine.Random.Range(0, maxSize);
		int height = UnityEngine.Random.Range(0, maxSize);

		for (int x = startX; x < startX+width; x++)
		{
			for (int i = startY; i < startY+height; i++)
			{
				_tiles[x, i] = tile;
			}
		}
	}

	public void StampWalk(int maxLength, float chanceToTurn = 0.2f)
	{
		int centerPad = 1;
		int startX = Random.Range(centerPad, _width - centerPad - 1);
		int startY = Random.Range(centerPad, _height - centerPad - 1);
		int x = startX;
		int y = startY;
		var d = Directions[UnityEngine.Random.Range(0, Directions.Length)];
		for (int i = 0; i < maxLength; i++)
		{
			_tiles[x, y] = Floor;
			if (Random.value > chanceToTurn)
			{
				d = Directions[UnityEngine.Random.Range(0, Directions.Length)];
			}
			switch (d)
			{
				case PDir.Up:
					y++;
					break;
				case PDir.Down:
					y--;
					break;
				case PDir.Left:
					x--;
					break;
				case PDir.Right:
					x++;
					break;
			}

			//just restart if out-of-bounds.
			if (x < 0 || y < 0 || x >= _width || y >= _height)
			{
				x = startX;
				y = startY;
			}
		}

	}
	
}
