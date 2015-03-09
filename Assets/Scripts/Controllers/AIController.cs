using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using PathFind;

/// <summary>
/// Unit controller for an AI. It calculates the best path to move to where its target is, and follows it
/// until it runs out of energy
/// </summary>
public class AIController : UnitController
{
	private Grid grid;
	
	private Unit unit;
	private Unit target;

	private MoveController moveController;

	private int currentIndex;
	private List<HexData> currentPath;

	public AIController(string userName, Grid grid, Unit unit, Unit target) : base(userName)
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

		//Find the path for this turn
		currentPath = findPath();

		currentIndex = 0;
		checkForMove();
	}

	private List<HexData> findPath()
	{
		List<HexData> hexList = new List<HexData>(); 

		HexData start = grid.retrieveHexData(unit.hexCoord);
		HexData destination = grid.retrieveHexData(target.hexCoord);

		//Calculate the path to that hex. We calculate the path to the user unit taking in account all neighbours and then removing that last hex 
		Path<HexData> path = PathFind.PathFind.FindPathHexData(start, destination);

		//If there is a path
		if (path != null)
		{
			hexList = path.ToList();

			//Remove the first(unit position) and last node(target position)
			hexList.RemoveAt(0);
			hexList.RemoveAt(hexList.Count - 1);

			hexList.Reverse();
		}

		return hexList;
	}

	public override void update(float deltaTime)
	{

	}

	private void moveTo(HexData hexData)
	{
		unit.moveTo(hexData);
		unit.onMoveEnd += onMoveEnd;
	}

	/// <summary>
	/// Calculates whether the unit can move to the next cell of not
	/// </summary>
	private void checkForMove()
	{
		//Move to the next cell, if there is still cells left in the path and we have enough energy to move to that cell
		if (currentIndex < currentPath.Count && unit.energy >= currentPath[currentIndex].energy)
		{
			moveTo(currentPath[currentIndex]);
		}
		else //If not, end the turn
		{
			dispatchOnTurnEnd();
		}
	}

	private void onMoveEnd()
	{
		unit.onMoveEnd -= onMoveEnd;

		currentIndex++;
		checkForMove();
	}
}