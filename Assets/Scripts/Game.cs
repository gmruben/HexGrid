using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Model;
using PathFind;

public class Game : MonoBehaviour
{
	public Camera gameCamera;

	public Player player;
	public Player enemy;

	public Grid grid;

	public GameHUD gameHUD;

	private int currentUserIndex = 0;
	private List<UnitController> userControllerList = new List<UnitController>();

	void Start()
	{
		init ();
	}

	public void init()
	{
		userControllerList.Add(new UserController(grid, player));
		userControllerList.Add(new AIController(grid, enemy, player));

		player.init(grid);
		enemy.init(grid);

		player.onUpdateEnergy += onUpdateEnergy;
		player.onGetGold += onGetGold;

		grid.init();

		gameHUD.init();
		gameHUD.onEndTurn += onTurnEnd;

		HexCoordinates hexCoord1 = grid.retrieveRandomCoord();
		HexCoordinates hexCoord2 = grid.retrieveRandomCoord();

		player.setPosition(hexCoord1);
		enemy.setPosition(hexCoord2);

		currentUserIndex = 0;

		userControllerList[currentUserIndex].onTurnEnd += onTurnEnd;
		userControllerList[currentUserIndex].startTurn();
	}

	void Update()
	{
		userControllerList[currentUserIndex].update(Time.deltaTime);
	}

	private void logTile(Tile tile)
	{
		Debug.Log(tile.X + " - " + tile.Y);
	}

	private void onGetGold(int gold)
	{
		gameHUD.updateGold(gold);
	}

	private void onTurnEnd()
	{
		grid.hideHighlightedCells();

		userControllerList[currentUserIndex].onTurnEnd -= onTurnEnd;

		currentUserIndex++;
		if (currentUserIndex >= userControllerList.Count) currentUserIndex = 0;

		userControllerList[currentUserIndex].onTurnEnd += onTurnEnd;
		userControllerList[currentUserIndex].startTurn();
	}

	private void onUpdateEnergy(int energy)
	{
		gameHUD.updateEnergy(energy);
	}
}