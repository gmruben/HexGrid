using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

/// <summary>
/// Game HUD for the game.
/// </summary>
public class GameHUD : MonoBehaviour
{
	public Text energy;
	public Text gold;

	public UIButton endTurnButton;

	void Awake()
	{
		//Set End Turn button inactive when awake
		endTurnButton.setActive(false);
	}

	public void updateEnergy(int value)
	{
		energy.text = StringsStore.retrieveString("ENERGY") + ": " + value.ToString();
	}

	public void updateGold(int value)
	{
		gold.text = StringsStore.retrieveString("GOLD") + ": " + value.ToString();
	}
}