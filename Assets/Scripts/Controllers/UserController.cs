using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

using Model;
using PathFind;

/// <summary>
/// Unit controller for a human user. When a user clicks in an adjacent cell, it moves the unit if it
/// has enough energy and if it a valid cell.
/// </summary>
public class UserController : UnitController
{
	private Grid grid;
	private Player player;

	private GameHUD gameHUD;

	public UserController(string userName, Grid grid, Player player, GameHUD gameHUD) : base (userName)
	{
		this.grid = grid;
		this.player = player;

		this.gameHUD = gameHUD;

		//Listen to End Turn button in the HUD
		gameHUD.endTurnButton.onClick += onEndTurnButtonClick;
	}

	public override void startTurn()
	{
		gameHUD.endTurnButton.setActive(true);

		player.reset();
		grid.showHighlightedCells(player.hexCoord, player.energy);
	}

	public override void update(float deltaTime)
	{
		if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
		{
			//Convert the mouse position to hex coordinates
			Vector3 position = GameCamera.cachedCamera.ScreenToWorldPoint(Input.mousePosition);
			HexCoordinates hex = Hex.worldToHex(position);

			if (grid.isCoordOnBounds(hex))
			{
				//Check that the unity has enough energy to move to that cell and that is it is adjacent and empty
				HexData hexData = grid.retrieveHexData(hex);
				if (grid.isAdjacent(player.hexCoord, hex) && hexData.isEmpty && hexData.energy <= player.energy)
				{
					player.moveTo(hexData);
					player.onMoveEnd += onMoveEnd;

					grid.hideHighlightedCells();
				}
			}
		}
	}

	private void onMoveEnd()
	{
		player.onMoveEnd -= onMoveEnd;

		//If the player ran out of energy we can't make any more moves, so finish the turn
		if (player.energy == 0)
		{
			gameHUD.endTurnButton.setActive(false);
			dispatchOnTurnEnd();
		}
		else
		{
			grid.showHighlightedCells(player.hexCoord, player.energy);
		}
	}

	private void onEndTurnButtonClick()
	{
		gameHUD.endTurnButton.setActive(false);
		dispatchOnTurnEnd();
	}
}