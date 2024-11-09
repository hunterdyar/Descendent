using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Proc
{
	public static class Solver
	{
		public static bool Solve(ProtoLevel proto, int minMoves)
		{
			//don't have a valid "start" directoin for the recursive search, so we have to start all four. This is ugly but whatever.
			for (int i = 0; i < 4; i++)
			{
				var validSolutions = new List<ProtoLevel>();
				var x = RecursiveTestMove(proto, ProtoLevel.Directions[i],ref validSolutions, 0);
				if (x)
				{
					if (validSolutions.Count == 0)
					{
						continue;
					}
					
					if(validSolutions.Min(x=>x.Moves.Count) >= minMoves){
						return true;
					}
				}
			}
			
			return false;
		}

		static bool RecursiveTestMove(ProtoLevel level, PDir testDir, ref List<ProtoLevel> validSolutions, int depth)
		{
			if (depth > 1000)
			{
				return false;
			}

			if (level.Solved)
			{
				validSolutions.Add(level);
				return true;
			}
			//try a move.
			if (level.MoveDir(testDir))
			{
				//did we solve the puzzle? we're done!
				if (level.Solved)
				{
					validSolutions.Add(level);
					return true;
				}
				
				//no? Time to keep trying this path. 
				for (int i = 0; i < 4; i++)
				{
					var d = ProtoLevel.Directions[i];
					if (ProtoLevel.IsOppositeDir(d, testDir))
					{
						//don't go back right from where we came from!
						continue;
					}

					var newLevel = level.Clone();
					var x = RecursiveTestMove(newLevel, d, ref validSolutions, depth + 1);
					
					if (x)
					{
						//return false
						//how to more exhaustively search?
						continue;
					}
					else
					{
						continue;
					}
				}
			}

			//This move didn't accomplish anything! Give up this path.
			return false;
		}
	}
}