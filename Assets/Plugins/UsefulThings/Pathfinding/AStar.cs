using PM.UsefulThings.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings.Pathfinding
{
	public class AStar
	{
		public static List<IPathNode> FindPath(IPathNode start, IPathNode finish)
		{
			if (start == finish)
			{
				return new List<IPathNode>();
			}

			var frontier = new GenericPriorityQueue<IPathNode, int>(1000);
			frontier.Enqueue(start, 0);
			var cameFrom = new Dictionary<IPathNode, IPathNode>();
			var costSoFar = new Dictionary<IPathNode, int>();
			IPathNode current;
			IPathNode next;
			List<IPathNode> neighbours;
			int newCost;
			int priority;

			cameFrom[start] = null;
			costSoFar[start] = 0;


			while (frontier.Count > 0)
			{
				current = frontier.Dequeue();

				if (current == finish)
				{
					break;
				}

				neighbours = current.GetNeighbours();

				for (int i = 0; i < neighbours.Count; i++)
				{
					next = neighbours[i];
					newCost = costSoFar[current] + (next == finish || next.IsWalkable ? 1 : 10000);

					if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
					{
						if (costSoFar.ContainsKey(next))
						{
							costSoFar[next] = newCost;
						}
						else
						{
							costSoFar.Add(next, newCost);
						}

						priority = newCost + finish.GetHeuristic(next);
						if (frontier.Contains(next))
						{
							frontier.UpdatePriority(next, priority);
						}
						else
						{
							frontier.Enqueue(next, priority);
						}


						cameFrom[next] = current;
					}
				}
			}

			var path = new List<IPathNode>();

			if (cameFrom.ContainsKey(finish))
			{
				IPathNode last = finish;
				path.Add(finish);
				while (cameFrom[last] != start)
				{
					path.Add(cameFrom[last]);
					last = cameFrom[last];
				}

				path.Reverse();
			}

			return path;
		}
	}

	public interface IPathNode : IGenericPriorityQueueNode<int>
	{
		Vector3Int FieldPosition { get; }
		bool IsWalkable { get; }

		List<IPathNode> GetNeighbours();
		int GetHeuristic(IPathNode target);
	}
}