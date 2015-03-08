using UnityEngine;
using System.Collections;

/// <summary>
/// This class takes care of instantiating any UI Element of the game.
/// </summary>
public class UIManager
{
	public static TurnOverlay instantiateTurnOverlay()
	{
		GameObject turnOverlayPrefab = Resources.Load<GameObject>("Prefabs/UI/TurnOverlay");
		return (GameObject.Instantiate(turnOverlayPrefab) as GameObject).GetComponent<TurnOverlay>();
	}
}