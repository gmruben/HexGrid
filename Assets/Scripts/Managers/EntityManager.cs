using UnityEngine;
using System.Collections;

/// <summary>
/// This class takes care of instantiating any Entity of the game.
/// </summary>
public class EntityManager
{
	public static Player instantiatePlayer()
	{
		GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Game/Player");
		return (GameObject.Instantiate(playerPrefab) as GameObject).GetComponent<Player>();
	}

	public static Player instantiateEnemy()
	{
		GameObject enemyPrefab = Resources.Load<GameObject>("Prefabs/Game/Enemy");
		return (GameObject.Instantiate(enemyPrefab) as GameObject).GetComponent<Player>();
	}
}