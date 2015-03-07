using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

using Model;
using PathFind;

public class UserController : UnitController
{
	private Grid grid;
	private Player player;

	public UserController(Grid grid, Player player)
	{
		this.grid = grid;
		this.player = player;
	}

	public override void startTurn()
	{
		player.reset();
		grid.showHighlightedCells(player.hexCoord, player.energy);
	}

	public override void update(float deltaTime)
	{
		if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
		{
			Vector3 position = GameCamera.cachedCamera.ScreenToWorldPoint(Input.mousePosition);
			HexCoordinates hex = Hex.worldToHex(position);
			
			HexData hexData = grid.retrieveHexData(hex);
			if (grid.isAdjacent(player.hexCoord, hex) && hexData.isEmpty && hexData.energy <= player.energy)
			{
				player.moveTo(hexData);
				player.onMoveEnd += onMoveEnd;

				grid.hideHighlightedCells();
			}
		}
	}

	private void onMoveEnd()
	{
		player.onMoveEnd -= onMoveEnd;

		if (player.energy == 0)
		{
			dispatchOnTurnEnd();
		}
		else
		{
			grid.showHighlightedCells(player.hexCoord, player.energy);
		}
	}
}