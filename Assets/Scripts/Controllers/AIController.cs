using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Model;
using PathFind;

using System.Linq;

/// <summary>
/// Unit controller for an AI. It calculates the best path to move to where its target is, and follows it
/// until it runs out of energy
/// </summary
public class AIController : UnitController
{
	private Grid grid;
	
	private Player unit;
	private Player target;

	private MoveController moveController;

	private int currentIndex;
	private List<HexData> currentPath;

	public AIController(string userName, Grid grid, Player unit, Player target) : base(userName)
	{
		this.grid = grid;
		this.unit = unit;
		this.target = target;

		currentPath = new List<HexData>();
	}

	public override void startTurn()
	{
		unit.reset();
		currentPath.Clear();

		currentPath = findPath();

		currentIndex = 0;
		if (currentIndex < currentPath.Count && unit.energy >= currentPath[currentIndex].energy)
		{
			moveTo(currentPath[currentIndex]);
		}
		else
		{
			dispatchOnTurnEnd();
		}
	}

	private List<HexData> findPath()
	{
		HexData start = grid.retrieveHexData(unit.hexCoord);
		HexData destination = grid.retrieveHexData(target.hexCoord);

		//For the distance function we use the cell energy
		Func<HexData, HexData, double> distance = (node1, node2) => node2.energy;
		//For the estimate we just use the direct distance between two cells
		Func<HexData, double> estimate = t => Hex.hexDistance(t.hexCoord, destination.hexCoord);
		
		List<HexData> path = PathFind.PathFind.FindPath(start, destination, distance, estimate).ToList();

		//Remove the first(unit position) and last node(target position)
		path.RemoveAt(0);
		path.RemoveAt(path.Count - 1);

		path.Reverse();

		return path;
	}

	public override void update(float deltaTime)
	{

	}

	public void moveTo(HexData hexData)
	{
		unit.moveTo(hexData);
		unit.onMoveEnd += onMoveEnd;
	}

	private void onMoveEnd()
	{
		unit.onMoveEnd -= onMoveEnd;

		//Move to the next cell, if there is still cells left in the path and we have enough energy to move to that cell
		currentIndex++;
		if (currentIndex < currentPath.Count && unit.energy >= currentPath[currentIndex].energy)
		{
			moveTo(currentPath[currentIndex]);
		}
		else
		{
			dispatchOnTurnEnd();
		}
	}
}