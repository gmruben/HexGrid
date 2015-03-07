using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class GameHUD : MonoBehaviour
{
	public event Action onEndTurn;

	public Text energy;
	public Text gold;

	public UIButton endTurnButton;

	public void init()
	{
		endTurnButton.onClick += onEndTurnButtonClick;
	}

	public void updateEnergy(int value)
	{
		energy.text = "ENERGY: " + value.ToString();
	}

	public void updateGold(int value)
	{
		gold.text = "GOLD: " + value.ToString();
	}

	public void endTurn()
	{
		endTurnButton.setActive(true);
	}

	private void onEndTurnButtonClick()
	{
		if (onEndTurn != null) onEndTurn();
	}
}