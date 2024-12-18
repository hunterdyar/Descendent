﻿using System;
using System.Collections.Generic;
using System.Linq;
using Proc;
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
	private List<Vector2Int> _required;

	public PlayerGraph GetPlayerGraph() => _playerGraph;
	private PlayerGraph _playerGraph;
	
	private BSPNode _bsp;
	public ProtoLevel(BSPNode root = null)
	{
		_bsp = root;
		_width = _bsp.Size.x;
		_height = _bsp.Size.y;
		_tiles = new PTile[_width, _height];
		_walk = new int[_width, _height];
		//todo: wait, this isn't... true? i have to dig into children i think
		_required = root.GetAllInternalConnectionPoints();
		_playerGraph = new PlayerGraph();
	}

	public void Generate()
	{
		//recursively slap some data down into our rooms
		StampBSPRoom(_bsp);
		
		//remove dead ends.
		 int removedDeadEnds = RemoveDeadEnds();
		 while (removedDeadEnds > 0)
		 {
		 	removedDeadEnds = RemoveDeadEnds();
		 }

		 _playerStart = GetRandomTile(Floor);
		//Debug.Log("Calculating Walk Paths");
		CalculateWalkPath();

		//todo: if the required tiles that are in this level are not in the walkpath, then we failed.
		//Can we interset the walk paths where the player starts at every valid node?

		//int fillCount = FillUnwalkable();
	}

	private void StampBSPRoom(BSPNode node)
	{
		if (node.IsLeaf)
		{
			if (node.Size.x >= 2 && node.Size.y >= 2)
			{
				var connectionPoints = new List<Vector2Int>();
				//BSPNode.GetConnectionPoints(node, ref connectionPoints);
				// var pl = ProtoLevel.CreateSolidLevel(node.Size.x - xg, node.Size.y - yg, connectionPoints);
				//StampSingleRoom(node.Position, node.Size, connectionPoints);
				StampSingleRoom(node.Position, node.Size, _required);
			}
			else
			{
				Debug.LogWarning("node size too small!");
			}
		}
		else
		{
			//gaps between the floors.
			 foreach (var internalConnectionPoint in node.InternalConnectionPoints)
			 {
			 	_tiles[internalConnectionPoint.x, internalConnectionPoint.y] = Floor;
			 }

			StampBSPRoom(node.ChildA);
			StampBSPRoom(node.ChildB);
		}
	}
	
	private void StampSingleRoom(Vector2Int position, Vector2Int size, List<Vector2Int> connectionPoints)
	{
		int width = size.x;
		int height = size.y;
		int px = position.x;
		int py = position.y;
		if (width <= 1|| height <= 1)
		{
			throw new Exception("Invalid Dimensions to create stamp level.");
		}

		var required = connectionPoints ?? new List<Vector2Int>();
		
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				_tiles[px+x, py+y] = Wall;
			}
		}

		foreach (var pos in required)
		{
			if (pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height)
			{
				continue;
			}
			_tiles[pos.x, pos.y] = Floor;
			StampWalk(position, size, Random.Range(5, Random.value < 0.5f ? _width : _height), 0.2f, pos);
		}
		
		for (int i = 0; i < 11; i++)
		{
			StampWalk(position, size, Random.Range(5,Random.value < 0.5f ? _width : _height));
		}

		int c = Random.Range(0, 2);
		for (int i = 0; i <c; i++)
		{
			StampRectangle(position, size, Floor,4);
		}
		
		c = Random.Range(5, 8);
		for (int i = 0; i < c; i++)
		{
			StampRectangle(position,size, Floor,3);
		}
		
		c = Random.Range(7, 9);
		for (int i = 0; i < c; i++)
		{
			StampRectangle(position,size,Floor,2);
		}
		
		for (int i = 0; i < 3; i++)
		{
			StampRectangle(position,size,Wall,2);
		}
	}
	
	public Vector2Int PlayerStartLocation()
	{
		return _playerStart;
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

	private void StampRectangle(Vector2Int pos, Vector2Int size, PTile tile, int maxSize)
	{
		if (maxSize == 0)
		{
			return;
		}

		if (maxSize > size.x || maxSize > size.y)
		{
			maxSize = Mathf.Min(size.x, size.y);
		}
		int startX = Random.Range(0, size.x-maxSize-1);
		int startY = Random.Range(0, size.y-maxSize-1);
		
		int width = Random.Range(0, maxSize);
		int height = Random.Range(0, maxSize);

		if (size.x == 1)
		{
			width = 1;
			startX = 0;
		}
		if (size.y == 1)
		{
			height = 1;
			startY = 0;
		}
		
		if (startX < 0 || startY < 0 || startX+width >= size.x || startY+height >= size.y)
		{
			Debug.LogWarning($"Invalid Input to StampRect. BoundRect (bsproom) size is {size.x}, {size.y}");
			return;
		}
		
		for (int x = startX; x < startX+width; x++)
		{
			for (int y = startY; y < startY+height; y++)
			{
				//todo: Detect why this is still sometimes out of range. 
				_tiles[x+pos.x, y+pos.y] = tile;
			}
		}
	}

	private void StampWalk(Vector2Int boundPos, Vector2Int boundsize, int maxLength, float chanceToTurn, Vector2Int start)
	{
		int x = start.x;
		int y = start.y;
		var d = Directions[Random.Range(0, Directions.Length)];
		for (int i = 0; i < maxLength; i++)
		{
			//just restart if out-of-bounds of the bounding box.
			if (x < 0 || y < 0 || x >= boundsize.x || y >= boundsize.y)
			{
				x = start.x;
				y = start.y;
			}

			//clamp? this should hopefully never happen, remove once i am sure of that.
			int px = x + boundPos.x;
			int py = y + boundPos.y;

			_tiles[px, py] = Floor;
			if (Random.value < chanceToTurn)
			{
				d = Utility.GetRandomOrthogonalDirection(d);
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
		}
	}
	private void StampWalk(Vector2Int boundPos, Vector2Int boundsize, int maxLength, float chanceToTurn = 0.2f)
	{
		if (maxLength == 0)
		{
			return;
		}
		int centerPad = 1;
		int startX = Random.Range(centerPad, boundsize.x - centerPad - 1) ;
		if (boundsize.x <= centerPad)
		{
			startX = 0;
		}
		int startY = Random.Range(centerPad, boundsize.y - centerPad - 1);
		if (boundsize.y <= centerPad)
		{
			startY = 0;
		}

		startX = Mathf.Clamp(startX + boundPos.x, 0, _width - 1) - boundPos.x;
		startY = Mathf.Clamp(startY + boundPos.y, 0, _height- 1) - boundPos.y;
		
		StampWalk(boundPos, boundsize, maxLength, chanceToTurn, new Vector2Int(startX, startY));
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
					var delta = Utility.PDirToXY(d);
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
		_playerGraph.Clear();
		_walk = new int[_width, _height];
		RecursiveCalculateWalkPath(_playerStart.x, _playerStart.y,1);
	}

	private void RecursiveCalculateWalkPath(int playerX, int playerY, int steps)
	{
		if (steps > _width * _height)
		{
			Debug.Log("Escape! Calc Walk Path");
			return;
		}
		var startPos = new Vector2Int(playerX, playerY);
		
		//The player has already been here, so it's not a true stopping point.
		//imagine moving into the middle of a wall. player has already been along it, it returns them to the path just like returning to the corner.
		if (_walk[playerX, playerY] > 1)
		{
			return;
		}
		
		if (_playerGraph.HasVisitedDuringGeneration(startPos))
		{
			return;
		}

		int nsteps = steps + 1;
		_playerGraph.Visit(startPos);
		foreach (var direction in Directions)
		{
			//todo: skip incoming direction (we just hit something) and reverse direction (we've been there).
			var delta = Utility.PDirToXY(direction);
			int x = playerX;
			int y = playerY;
			int distance = RecursiveWalkSinglePlayerMove(ref x,ref y, delta.x,delta.y,0,Mathf.Max(_width,_height),0);
			if (distance > 0)
			{
				var p = new Vector2Int(x, y);
				_playerGraph.DirectedConnect(startPos,p);
				RecursiveCalculateWalkPath(x, y, nsteps);
			}
		}
	}

	private int RecursiveWalkSinglePlayerMove(ref int px, ref int py, int dx, int dy, int depth, int maxDepth, int c = 0)
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
