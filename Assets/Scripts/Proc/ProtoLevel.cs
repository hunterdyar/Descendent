using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class ProtoLevel
{
	public static readonly PDir[] Directions = {PDir.Left, PDir.Right, PDir.Up, PDir.Down};
	//not writing 'Ptile.' as much.
	private const PTile Floor = PTile.Floor;
	private const PTile Wall = PTile.Wall;

	public PTile[,] Tiles => _tiles;
	private PTile[,] _tiles;
	private int[,] _walk;
	public int Width => _width;
	private int _width;
	public int Height => _height;
	private int _height;
	private Vector2Int _playerStart;
	private Vector2Int _playerLoc;
	private List<Vector2Int> _required = new List<Vector2Int>();
	private readonly Dictionary<Vector2Int, int> _visited = new Dictionary<Vector2Int, int>();
	public Vector2Int PlayerStartLocation()
	{
		return _playerStart;
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
			int x = Random.Range(0, _width);
			int y = Random.Range(0, _height);
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

	private static (int x, int y) PDirToXY(PDir dir)
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
	
	public Vector2Int GetRandomTile(PTile floor)
	{
		int escape = _width * _height * 3;
		while (escape > 0)
		{
			int x = Random.Range(0, _width);
			int y = Random.Range(0, _height);
			if (_tiles[x, y] == floor)
			{
				return new Vector2Int(x, y);
			}

			escape--;
		}
		throw new Exception("Overflow Exception in GetRandomTile. Couldn't find tile");
	}

	public static ProtoLevel CreateRandomStampLevel(int width, int height, List<Vector2Int> requiredPlayerFloors = null)
	{
		if (width <= 1|| height <= 1)
		{
			throw new Exception("Invalid Dimensions to create stamp level.");
		}
		ProtoLevel pLevel = new ProtoLevel
		{
			_width = width,
			_height = height,
			_tiles = new PTile[width, height],
			_required = requiredPlayerFloors ?? new List<Vector2Int>()
		};
		
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				pLevel._tiles[x, y] = Wall;
			}
		}

		foreach (var pos in pLevel._required)
		{
			pLevel._tiles[pos.x, pos.y] = Floor;
		}
		
		Debug.Log("Walks and Stamps");
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

		
		//remove dead ends.
		Debug.Log("Removing Dead Ends");
		int removedDeadEnds = pLevel.RemoveDeadEnds();
		while (removedDeadEnds > 0)
		{
			removedDeadEnds = pLevel.RemoveDeadEnds();
		}
		
		Debug.Log("Picking Player Start for room");
		pLevel._playerStart = pLevel.GetRandomTile(Floor);

		Debug.Log("Calculating Walk Paths");
		pLevel.CalculateWalkPath();
		int fillCount = pLevel.FillUnwalkable();
		Debug.Log($"{fillCount} dead tiles filled.");
		//todo: this can't be the best way about getting this value...
		var end = pLevel._visited.OrderByDescending(x=>x.Value).First().Key;
		Debug.Log($"Exit placed on visited: {pLevel._visited[end]}");
		pLevel._tiles[end.x, end.y] = PTile.Exit;
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

	private void StampRect(PTile tile, int maxSize)
	{
		if (maxSize == 0)
		{
			return;
		}

		if (maxSize > _width || maxSize > _height)
		{
			maxSize = Mathf.Min(_width, _height);
		}
		int startX = Random.Range(0, _width-maxSize-1);
		int startY = Random.Range(0, _height-maxSize-1);
		
		int width = Random.Range(0, maxSize);
		int height = Random.Range(0, maxSize);

		if (_width == 1)
		{
			width = 1;
			startX = 0;
		}
		if (_height == 1)
		{
			height = 1;
			startY = 0;
		}
		if (startX < 0 || startY < 0 || startX+width >= _width || startY+height >= _height)
		{
			Debug.LogWarning($"Invalid Input to StampRect. Node size is {_width}, {_height}");
			return;
		}
		
		for (int x = startX; x < startX+width; x++)
		{
			for (int y = startY; y < startY+height; y++)
			{
				//todo: Detect why this is still sometimes out of range. 
				_tiles[x, y] = tile;
			}
		}
	}

	private void StampWalk(int maxLength, float chanceToTurn = 0.2f)
	{
		if (maxLength == 0)
		{
			return;
		}
		int centerPad = 1;
		int startX = Random.Range(centerPad, _width - centerPad - 1);
		if (_width <= centerPad)
		{
			startX = 0;
		}
		int startY = Random.Range(centerPad, _height - centerPad - 1);
		if (_height <= centerPad)
		{
			startY = 0;
		}
		int x = startX;
		int y = startY;
		var d = Directions[Random.Range(0, Directions.Length)];
		for (int i = 0; i < maxLength; i++)
		{
			_tiles[x, y] = Floor;
			if (Random.value < chanceToTurn)
			{
				d = Directions[Random.Range(0, Directions.Length)];
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

	private int RemoveDeadEnds()
	{
		int changed = 0;
		//Fill any tile with 4 covered sides.
		//Open a nearby tile for any with 3 or more sides filled.
		for (int x = 0; x < _width; x++)
		{
			for (int y = 0; y < _height; y++)
			{
				if (_tiles[x, y] != Floor)
				{
					continue;
				}
				
				//count sides
				int c = CountSurroundingWalls(x, y);
				
				//Fill isolated squares.
				if (c == 4)
				{
					//only fill square if tile is not required.
					if (!_required.Contains(new Vector2Int(x, y)))
					{
						changed++;
						_tiles[x, y] = Wall;
						continue;
					}
				}
				
				if (c < 3)
				{
					continue;
				}
				
				//Fill a tile on some nearby side.
				var dir = Proc.Utility.GetDirectionsShuffled();
				for (int i = 0; i < 4; i++)
				{
					var d = dir[i];
					var delta = PDirToXY(d);
					int nextX = x + delta.x;
					int nextY = y + delta.y;
					if (nextX < 0 || nextX >= _width || nextY < 0 || nextY >= _height)
					{
						continue;
					}
					_tiles[nextX, nextY] = Floor;
					changed++;
					break;
				}
			}
		}

		return changed;
	}

	private int CountSurroundingWalls(int x, int y)
	{
		int count = 0;
		if (x > 0)
		{
			//don't cast to int because exits/players/stuff will count wrong.
			count += _tiles[x - 1, y] == PTile.Wall ? 1 : 0;
		}
		else
		{
			count++;
		}
		
		if (x < _width-1)
		{
			count += _tiles[x + 1, y] == PTile.Wall ? 1 : 0;
		}
		else
		{
			count++;
		}
		
		if (y > 0)
		{
			count += _tiles[x, y-1] == PTile.Wall ? 1 : 0;
		}
		else
		{
			count++;
		}
		if (y < _height-1)
		{
			count += _tiles[x, y+1] == PTile.Wall ? 1 : 0;
		}
		else
		{
			count++;
		}

		return count;
	}
	
	private int FillUnwalkable()
	{
		if (_walk == null)
		{
			throw new NullReferenceException("Unable to fill unwalkable");
		}

		int count = 0;
		for (int x = 0; x < _width; x++)
		{
			for (int y = 0; y < _height; y++)
			{
				if (_tiles[x,y] == Floor && _walk[x, y] == 0)
				{
					_tiles[x, y] = PTile.Wall;
					count++;
				}
			}
		}

		return count;
	}

	private void CalculateWalkPath()
	{
		_visited.Clear();
		_walk = new int[_width, _height];
		RecursiveCalculateWalkPath(_playerStart.x, _playerStart.y,1);
	}

	private void RecursiveCalculateWalkPath(int playerX, int playerY, int steps)
	{
		var startPos = new Vector2Int(playerX, playerY);
		
		//The player has already been here, so it's not a true stopping point.
		//imagine moving into the middle of a wall. player has already been along it, it returns them to the path just like returning to the corner.
		if (_walk[playerX, playerY] > 1)
		{
			return;
		}
		
		// If the key already exists, TryAdd does nothing and returns false.
		if (!_visited.TryAdd(startPos, steps))
		{
			return;
		}

		int nsteps = steps + 1;
		foreach (var direction in Directions)
		{
			var delta = PDirToXY(direction);
			int x = playerX;
			int y = playerY;
			int distance = RecursiveWalkSinglePlayerMove(ref x,ref y, delta.x,delta.y,0,Mathf.Max(_width,_height));
			if (distance > 0)
			{
				RecursiveCalculateWalkPath(x, y, nsteps);
			}
		}
	}

	private int RecursiveWalkSinglePlayerMove( ref int px, ref int py, int dx, int dy, int depth, int maxDepth, int c = 0)
	{
		if (dx == 0 && dy == 0)
		{
			throw new Exception("Invalid Delta");
		}

		if (depth >= maxDepth)
		{
			return c;
		}

		var nextx = px + dx;
		int nexty = py + dy;
		if (nextx < 0 || nextx >= _width || nexty < 0 || nexty >= _height)
		{
			return c;
		}
		var nextTile = _tiles[nextx, nexty];
		if (nextTile == Wall)
		{
			return c;
		}

		if (nextTile == Floor)
		{
			px = nextx;
			py = nexty;
			_walk[px, py]++;
			return RecursiveWalkSinglePlayerMove(ref px,ref py, dx, dy,depth+1, maxDepth,c+1);
		}

		return c;
	}
}
