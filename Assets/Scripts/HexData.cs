using UnityEngine;
using System.Collections;

using Model;
using PathFind;

using System.Linq;
using System.Collections.Generic;

public class HexData : MonoBehaviour, IHasNeighbours<HexData>
{
	public int energy = 1;
	public bool isEmpty = true;

	public GameObject goldPrefab;
	private GameObject gold;

	public HexCoordinates hexCoord { get; private set; }

	public void init(HexCoordinates hexCoord)
	{
		this.hexCoord = hexCoord;
		transform.localPosition = Hex.hexToWorld(hexCoord);

		if (Random.Range(0.0f, 1.0f) < 0.5f)
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

	public void playerOn(Player player)
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