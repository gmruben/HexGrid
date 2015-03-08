using UnityEngine;
using System.Collections;

/// <summary>
/// This class takes care of instantiating any Entity of the game.
/// </summary>
public class EntityManager
{
	public static Unit instantiateUserUnit()
	{
		GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Game/UserUnit");
		return (GameObject.Instantiate(playerPrefab) as GameObject).GetComponent<Unit>();
	}

	public static Unit instantiateAIUnit()
	{
		GameObject enemyPrefab = Resources.Load<GameObject>("Prefabs/Game/AIUnit");
		return (GameObject.Instantiate(enemyPrefab) as GameObject).GetComponent<Unit>();
	}
}