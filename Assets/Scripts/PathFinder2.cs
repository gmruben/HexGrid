using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFinder2 : MonoBehaviour
{
	//distance f-ion should return distance between two adjacent nodes
	//estimate should return distance between any node and destination node
	public static Path<Node> FindPath<Node>(Node start, Node destination) where Node: IHasNeighbours<Node>
	{
		//set of already checked nodes
		HashSet<Node> closed = new HashSet<Node>();
		//queued nodes in open set
		PriorityQueue<double, Path<Node>> queue = new PriorityQueue<double, Path<Node>>();
		queue.Enqueue(0, new Path<Node>(start));
		
		while (!queue.IsEmpty)
		{
			Path<Node> path = queue.Dequeue();
			
			if (closed.Contains(path.LastStep))
				continue;
			if (path.LastStep.Equals(destination))
				return path;
			
			closed.Add(path.LastStep);
			
			foreach (Node n in path.LastStep.Neighbours)
			{
				double d = distance(path.LastStep, n);
				//new step added without modifying current path
				Path<Node> newPath = path.AddStep(n, d);
				queue.Enqueue(newPath.TotalCost + estimate(n), newPath);
			}
		}
		
		return null;
	}

	private static double distance<Node>(Node hexCoord1, Node hexCoord2)
	{
		return 1; //Hex.hexDistance(hexCoord1, hexCoord2);
	}

	private static double estimate<Node>(Node hexCoord1)
	{
		return 1; //Mathf.Abs(hexCoord1.r - hexCoord2.r) + Mathf.Abs(hexCoord1.q - hexCoord2.q);
	}
}