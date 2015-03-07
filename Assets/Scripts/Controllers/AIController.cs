using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Model;
using PathFind;

using System.Linq;

public class AIController : UnitController
{
	private Grid grid;
	
	private Player unit;
	private Player target;

	private MoveController moveController;

	private int currentIndex;
	private List<HexData> path;

	public AIController(Grid grid, Player unit, Player target)
	{
		this.grid = grid;
		this.unit = unit;
		this.target = target;

		path = new List<HexData>();
	}

	public override void startTurn()
	{
		unit.reset();
		path.Clear();

		HexData start = grid.retrieveHexData(unit.hexCoord);
		HexData destination = grid.retrieveHexData(target.hexCoord);
		
		Func<HexData, HexData, double> distance = (node1, node2) => node2.energy;
		Func<HexData, double> estimate = t => Hex.hexDistance(t.hexCoord, destination.hexCoord);

		path = PathFind.PathFind.FindPath(start, destination, distance, estimate).ToList();

		path.RemoveAt(0);
		path.RemoveAt(path.Count - 1);

		path.Reverse();

		currentIndex = 0;
		if (currentIndex < path.Count && unit.energy >= path[currentIndex].energy)
		{
			moveTo(path[currentIndex]);
		}
		else
		{
			dispatchOnTurnEnd();
		}
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

		currentIndex++;
		if (currentIndex < path.Count && unit.energy >= path[currentIndex].energy)
		{
			moveTo(path[currentIndex]);
		}
		else
		{
			dispatchOnTurnEnd();
		}
	}
}