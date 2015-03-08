using UnityEngine;
using System.Collections;

using Model;
using PathFind;

using System.Linq;
using System.Collections.Generic;

public class HexData : MonoBehaviour, IHasNeighbours<HexData>
{
	public int energy = 1;
	public bool isEmpty { get; set; }

	public GameObject goldPrefab;
	private GameObject gold;

	public HexCoordinates hexCoord { get; private set; }

	public void init(HexCoordinates hexCoord, bool hasGold)
	{
		this.hexCoord = hexCoord;
		isEmpty = true;

		//If the hex has a gold piece, create it
		if (hasGold)
		{
			gold = GameObject.Instantiate(goldPrefab) as GameObject;
			gold.transform.position = transform.position;
		}
	}

	public IEnumerable<HexData> AllNeighbours { get; set; }
	public IEnumerable<HexData> Neighbours
	{
		get
		{
			return AllNeighbours; //.Where(o => o.isEmpty);
		}
	}

	public void playerOn(Unit player)
	{
		if (hasGold)
		{
			player.updateGold(1);
			GameObject.Destroy(gold);
		}
	}

	public void FindNeighbours(Grid grid)
	{
		AllNeighbours = grid.retrieveNeighbours(hexCoord).Select(h => grid.retrieveHexData(h));
	}

	public bool hasGold
	{
		get { return gold != null; }
	}
}