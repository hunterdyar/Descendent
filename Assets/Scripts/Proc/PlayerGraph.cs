using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Proc
{
	public class PlayerGraph
	{
		private readonly Dictionary<Vector2Int, PlayerNode> _nodes = new Dictionary<Vector2Int, PlayerNode>();

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
					Debug.LogError("Invalid Direction for graph.");
					break;
			}
			
			throw new System.NotImplementedException();
		}
	}
}