using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathFind
{
	/// <summary>
	/// I use this generic implementation for A* algorithm, which is very elegant end flexible: https://tbswithunity3d.wordpress.com/2012/02/23/hexagonal-grid-path-finding-using-a-algorithm/
	/// I have added a little modification, which is the neighbours function. Instead of having a node class that implements IHasNeighbours, I am
	/// passing a function that retrieves the neighbour list for each node, just in case we want to run the algorithm under different circunstances (like using
	/// all the neighbours, or only those that are empty)
	/// </summary>
	public static class PathFind
	{
		public static Path<Node> FindPath<Node>( Node start, Node destination, Func<Node, Node, double> distance, Func<Node, double> estimate, Func<Node, List<Node>> neighbours) where Node : IHasNeighbours<Node>
		{
			var closed = new HashSet<Node>();
			var queue = new PriorityQueue<double, Path<Node>>();
			queue.Enqueue(0, new Path<Node>(start));
			
			while (!queue.IsEmpty)
			{
				var path = queue.Dequeue();
				
				if (closed.Contains(path.LastStep))
					continue;
				if (path.LastStep.Equals(destination))
					return path;
				
				closed.Add(path.LastStep);

				foreach (Node n in neighbours(path.LastStep))
				{
					double d = distance(path.LastStep, n);
					var newPath = path.AddStep(n, d);
					queue.Enqueue(newPath.TotalCost + estimate(n), newPath);
				}
			}
			
			return null;
		}

		/// <summary>
		/// This is an specific implementation of the generic version of FindPath where I use the node energy as the distance function
		/// and the axial distance as the estimate function. I also use all neighbours as neighbours function since there is no need to check
		/// for unwalkable cells or any other type.
		/// </summary>
		/// <returns>The path with all the hexes.</returns>
		/// <param name="start">Start hex.</param>
		/// <param name="destination">Destination hex.</param>
		public static Path<HexData> FindPathHexData(HexData start, HexData destination)
		{	
			//For the distance function we use the cell energy
			Func<HexData, HexData, double> distance = (node1, node2) => node2.energy;
			//For the estimate we just use the direct distance between two cells
			Func<HexData, double> estimate = t => HexMath.axialDistance(t.hexCoord.V2, destination.hexCoord.V2);
			//We add all neighbours 
			Func<HexData, List<HexData>> neighbours = node => node.AllNeighbours.ToList();
			
			return FindPath(start, destination, distance, estimate, neighbours);
		}
	}
}