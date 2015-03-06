using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFinder
{
	private int heuristic(HexCoordinates hexCoord1, HexCoordinates hexCoord2)
	{
		return Mathf.Abs(hexCoord1.r - hexCoord2.r) + Mathf.Abs(hexCoord1.q - hexCoord2.q);
	}
	
	public void search(Grid grid, HexCoordinates start, HexCoordinates goal)
	{
		Stack<HexCoordinates> frontier = new Stack<HexCoordinates>();
		frontier.Push(start);

		Dictionary<string, HexCoordinates> came_from = new Dictionary<string, HexCoordinates>();
		Dictionary<string, int> cost_so_far = new Dictionary<string, int>();

		string startId = grid.createId(start);

		came_from.Add(startId, null);
		cost_so_far.Add(startId, 0);

		int iterations = 0;
		HexCoordinates current = start;

		while (frontier.Count > 0 || iterations < 25)
		{
			current = frontier.Pop();

			if (current == goal)
			{
				break;
			}

			List<HexCoordinates> neighbours = grid.retrieveNeighbours(current);

			for (int i = 0; i < neighbours.Count; i++)
			{
				HexCoordinates next = neighbours[i];

				string nextId = grid.createId(next);
				string currentId = grid.createId(current);

				int new_cost = cost_so_far[currentId] + grid.cost(current, next);

				//if (!cost_so_far.count(next) || new_cost < cost_so_far[next])
				if (cost_so_far.ContainsKey(nextId) != null || new_cost < cost_so_far[nextId])
				{
					cost_so_far[nextId] = new_cost;

					//int priority = new_cost + heuristic(next, goal);
					//frontier.put(next, priority);
					frontier.Push(next);

					came_from[nextId] = current;
				}
			}

			iterations++;
		}

		List<HexCoordinates> hexList = new List<HexCoordinates>();
		HexCoordinates hex = current;

		string id = grid.createId(current);
		hexList.Add(hex);

		int it = 0;
		while (id != startId || it < 10)
		{
			hex = came_from[id];
			id = grid.createId(hex);

			if (hex != null)
			{
				Debug.Log(hex + "(" + id + ")");
				hexList.Add(hex);
			}

			it++;
		}
	}

	private int index(HexCoordinates coord)
	{
		return coord.r * Grid.radius + coord.q;
	}
}