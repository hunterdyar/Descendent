using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

namespace Proc
{
	public class PlayerGraph
	{
		private readonly Dictionary<Vector2Int, PlayerNode> _nodes = new Dictionary<Vector2Int, PlayerNode>();

		public void Clear()
		{
			_nodes.Clear();
			//sorry gc. this only happens in generation tho. so uhhhhh well, its fine.
		}
		public bool TryAdd(Vector2Int pos)
		{
			if (!_nodes.TryGetValue(pos, out var fromNode))
			{
				fromNode = new PlayerNode(pos);
				_nodes.Add(pos, fromNode);
				return true;
			}
			else
			{
				return false;
			}
		}
		public void DirectedConnect(Vector2Int from, Vector2Int to)
		{
			if (!_nodes.TryGetValue(from, out var fromNode))
			{
				fromNode = new PlayerNode(from);
				 _nodes.Add(from, fromNode);
			}

			if (!_nodes.TryGetValue(to, out var toNode))
			{
				toNode = new PlayerNode(to);
				_nodes.Add(to, toNode);
			}
			
			fromNode.ConnectTo(toNode);
		}

		public bool NoTraps()
		{
			return _nodes.Count > 0 && !_nodes.Values.Any(x => x.IsTrap());
		}


		public bool HasVisited(Vector2Int pos)
		{
			if (_nodes.TryGetValue(pos, out var node))
			{
				return node.HasExits();
			}
			else
			{
				return false;
			}
		}
	}

	public class PlayerNode
	{
		public readonly Vector2Int Position;
		//UP - RIGHT - DOWN - LEFT
		private readonly PlayerNode[] _edgesTo = new PlayerNode[4];
		private readonly PlayerNode[] _edgesFrom = new PlayerNode[4];

		//wait whats the graph theory term for this.
		/// <summary>
		/// You can leave this node, but you cannot return to it.
		/// </summary>
		public bool IsTrap()
		{
			return _edgesFrom.All(x => x == null) && _edgesTo.Any(x => x != null);
		}
		public PlayerNode(Vector2Int position)
		{
			this.Position = position;
		}

		public void ConnectTo(PlayerNode toNode)
		{
			var direction = (toNode.Position - Position);
			if (direction == Vector2.zero)
			{
				//Can't connect to self.
				Debug.LogWarning("Tried to connect player graph node to self.");
				return;
			}
			direction.Clamp(-Vector2Int.one, Vector2Int.one);
			switch (direction.x,direction.y)
			{
				case (0,1):
					_edgesTo[0] = toNode;
					toNode._edgesFrom[0] = this;
					break;
				case (1,0):
					_edgesTo[1] = toNode;
					toNode._edgesFrom[1] = this;
					break;
				case (0, -1):
					_edgesTo[2] = toNode;
					toNode._edgesFrom[2] = this;
					break;
				case (-1, 0):
					_edgesTo[3] = toNode;
					toNode._edgesFrom[3] = this;
					break;
				default:
					Debug.LogError($"Invalid Direction for graph: {direction.x},{direction.y}");
					break;
			}
			
		}

		public bool HasExits()
		{
			return _edgesTo.All(x => x == null);
		}
	}
}