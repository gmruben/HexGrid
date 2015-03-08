using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Abstract class for controlling a unit. Every unit controller has a username so we can identify them.
/// </summary>
public abstract class UnitController
{
	public event Action onTurnEnd;

	public string userName { get; private set; }

	public UnitController(string userName)
	{
		this.userName = userName;
	}

	public abstract void startTurn();
	public abstract void update(float deltaTime);

	protected void dispatchOnTurnEnd()
	{
		if (onTurnEnd != null) onTurnEnd();
	}
}