using UnityEngine;

namespace Proc
{
	public class BSPConnection
	{
		public Vector2Int A;
		public Vector2Int B;

		public static BSPConnection Connect(BSPNode a, BSPNode b)
		{
			//find shared side (horizontal, vertical, which is top/bottom)
			
			//recursively...
				//if the rooms are not leaves, then...
					//find which child node has the shared wall..
					//if they both do, pick one at random, weighted the same as their widths.
					
			//readjust the shared side's min and max points.
			
			//expand this point towards the side until it is contained in that node.
			//do the same the other way.
			
			return new BSPConnection();
		}
	}
}