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

	private Player player;
	private Player enemy;

	public Grid grid;

	public GameHUD gameHUD;

	private int currentUserIndex = 0;
	private List<UnitController> userControllerList = new List<UnitController>();

	private bool isActive = false;

	private TurnOverlay turnOverlay;

	void Start()
	{
		createGrid();
	}

	public void createGrid()
	{
		grid.init();
		grid.onCreated += onGridCreated;
	}

	public void onGridCreated()
	{
		player = EntityManager.instantiatePlayer();
		enemy = EntityManager.instantiateEnemy();

		player.init(grid);
		enemy.init(grid);

		player.onUpdateEnergy += onUpdateEnergy;
		player.onGetGold += onGetGold;

		userControllerList.Add(new UserController("PLAYER", grid, player, gameHUD));
		userControllerList.Add(new AIController("ENEMY", grid, enemy, player));

		HexCoordinates hexCoord1 = grid.retrieveRandomCoord();
		HexCoordinates hexCoord2 = grid.retrieveRandomCoord();

		while(hexCoord1 == hexCoord2)
		{
			hexCoord2 = grid.retrieveRandomCoord();
		}

		player.setPosition(hexCoord1);
		enemy.setPosition(hexCoord2);

		string message = userControllerList[currentUserIndex].userName + " " + StringsStore.retrieveString("TURN");
		showTurnOverlay(message, onTurnOverlayEnd);
	}

	private void onTurnOverlayEnd()
	{
		turnOverlay.onAnimationEnd += onTurnOverlayEnd;
		GameObject.Destroy(turnOverlay.gameObject);

		currentUserIndex = 0;
		
		userControllerList[currentUserIndex].onTurnEnd += onTurnEnd;
		userControllerList[currentUserIndex].startTurn();

		isActive = true;
	}

	private void showTurnOverlay(string message, Action onEnd)
	{
		turnOverlay = UIManager.instantiateTurnOverlay();
		
		turnOverlay.init(message);
		turnOverlay.onAnimationEnd += onEnd;
	}

	void Update()
	{
		if (isActive)
		{
			userControllerList[currentUserIndex].update(Time.deltaTime);
		}
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

		string message = userControllerList[currentUserIndex].userName + " " + StringsStore.retrieveString("TURN"); 
		showTurnOverlay(message, onTurnOverlayEnd2);
	}

	private void onTurnOverlayEnd2()
	{
		turnOverlay.onAnimationEnd += onTurnOverlayEnd;
		GameObject.Destroy(turnOverlay.gameObject);
		
		userControllerList[currentUserIndex].onTurnEnd += onTurnEnd;
		userControllerList[currentUserIndex].startTurn();
	}

	private void onUpdateEnergy(int energy)
	{
		gameHUD.updateEnergy(energy);
	}
}