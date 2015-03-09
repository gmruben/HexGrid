using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

using PathFind;

/// <summary>
/// Unit controller for a human user. When a user clicks in an adjacent cell, it moves the unit if it
/// has enough energy and if it a valid cell.
/// </summary>
public class UserController : UnitController
{
	private Grid grid;
	private Unit unit;

	private GameHUD gameHUD;

	public UserController(string userName, Grid grid, Unit unit, GameHUD gameHUD) : base (userName)
	{
		this.grid = grid;
		this.unit = unit;

		this.gameHUD = gameHUD;

		//Listen to End Turn button in the HUD
		gameHUD.endTurnButton.onClick += onEndTurnButtonClick;
	}

	public override void startTurn()
	{
		gameHUD.endTurnButton.setActive(true);

		unit.reset();
		grid.showHighlightedCells(unit.hexCoord, unit.energy);
	}

	public override void update(float deltaTime)
	{
		if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
		{
			//Convert the mouse position to hex coordinates
			Vector3 position = GameCamera.cachedCamera.ScreenToWorldPoint(Input.mousePosition);
			AxialCoordinates hex = grid.worldToHex(position);

			if (grid.isCoordOnBounds(hex))
			{
				//Check that the unity has enough energy to move to that cell and that is it is adjacent and empty
				HexData hexData = grid.retrieveHexData(hex);
				if (grid.isAdjacent(unit.hexCoord, hex) && hexData.isEmpty && hexData.energy <= unit.energy)
				{
					unit.moveTo(hexData);
					unit.onMoveEnd += onMoveEnd;

					grid.hideHighlightedCells();
				}
			}
		}
	}

	private void onMoveEnd()
	{
		unit.onMoveEnd -= onMoveEnd;

		//If the player ran out of energy we can't make any more moves, so finish the turn
		if (unit.energy == 0)
		{
			gameHUD.endTurnButton.setActive(false);
			dispatchOnTurnEnd();
		}
		else
		{
			grid.showHighlightedCells(unit.hexCoord, unit.energy);
		}
	}

	private void onEndTurnButtonClick()
	{
		gameHUD.endTurnButton.setActive(false);
		dispatchOnTurnEnd();
	}
}