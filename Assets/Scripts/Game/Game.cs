using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using PathFind;

/// <summary>
/// This class controls the flow for the main game.
/// </summary>
public class Game : MonoBehaviour
{
	private Unit userUnit;
	private Unit aiUnit;

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
		userUnit = EntityManager.instantiateUserUnit();
		aiUnit = EntityManager.instantiateAIUnit();

		userUnit.init(grid);
		aiUnit.init(grid);

		userUnit.onUpdateEnergy += onUpdateEnergy;
		userUnit.onGetGold += onGetGold;

		userControllerList.Add(new UserController("USER", grid, userUnit, gameHUD));
		userControllerList.Add(new AIController("AI", grid, aiUnit, userUnit));

		AxialCoordinates hexCoord1 = grid.retrieveRandomCoord();
		AxialCoordinates hexCoord2 = grid.retrieveRandomCoord();

		while(hexCoord1 == hexCoord2)
		{
			hexCoord2 = grid.retrieveRandomCoord();
		}

		userUnit.setPosition(hexCoord1);
		aiUnit.setPosition(hexCoord2);

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