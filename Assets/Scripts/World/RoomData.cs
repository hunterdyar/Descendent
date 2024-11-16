using UnityEngine;

namespace DefaultNamespace
{
	
	public class RoomData
	{
		public Vector2Int Position;
		public Vector2Int Size;

		public RoomData(Vector2Int position, Vector2Int size)
		{
			Position = position;
			Size = size;
		}

		public bool Contains(Vector2Int position)
		{
			return position.x >= Position.x && position.x <= Position.x + Size.x && position.y >= Position.y && position.y <= Position.y + Size.y;
		}
	}
}