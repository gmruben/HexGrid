using UnityEngine;
using System;
using System.Collections;

public abstract class UnitController
{
	public event Action onTurnEnd;

	public abstract void startTurn();
	public abstract void update(float deltaTime);

	protected void dispatchOnTurnEnd()
	{
		if (onTurnEnd != null) onTurnEnd();
	}
}