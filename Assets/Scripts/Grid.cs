using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Model;
using System.Linq;

public class Grid : MonoBehaviour
{
	public const int radius = 2;

	public GameObject[] hexPrefabList;

	public Tile[, ] tileList = new Tile[5, 5];

	//We use a dictionary to store the map because it allows us to have maps with
	//any type of shape. I create a unique key for each hex based on its coordinates
	private Dictionary<string, HexData> hexList = new Dictionary<string, HexData>();

	public void init()
	{
		for (int i = -radius; i <= radius; i++)
		{
			for (int j = -radius; j <= radius; j++)
			{
				instantiateHex(new HexCoordinates(i, j));

				int x = i + radius;
				int y = j + radius;
				tileList[x, y] = new Tile(x, y);
			}
		}

		AllTiles.ToList().ForEach(o => o.FindNeighbours(this));
	}

	public IEnumerable<Tile> AllTiles
	{
		get
		{
			for (int x = 0; x < 5; x++)
			{
				for (int y = 0; y < 5; y++)
				{
					yield return tileList[x, y];
				}
			}
		}
	}

	public HexCoordinates retrieveRandomCoord()
	{
		int randomIndex = Random.Range(0, hexList.Count);
		List<HexData> dataList = new List<HexData>(hexList.Values);

		return dataList[randomIndex].hexCoord;
	}

	public List<HexCoordinates> retrieveNeighbours(HexCoordinates hexCoord)
	{
		List<HexCoordinates> neighbours = new List<HexCoordinates>();
		List<HexCoordinates> hexList = Hex.movementRange(Hex.hexToCube(hexCoord.V2), 1);

		for (int i = 0; i < hexList.Count; i++)
		{
			if (hexCoord != hexList[i] && isCoordOnBounds(hexList[i]))
			{
				neighbours.Add(hexList[i]);
			}
		}

		return neighbours;
	}

	public bool isCoordOnBounds(HexCoordinates hexCoord)
	{
		string id = createId(hexCoord);
		return hexList.ContainsKey(id);
	}

	private void instantiateHex(HexCoordinates hexCoord)
	{
		int randomIndex = Random.Range(0, hexPrefabList.Length);

		HexData hex = (GameObject.Instantiate(hexPrefabList[randomIndex]) as GameObject).GetComponent<HexData>();

		hex.transform.parent = transform;
		hex.init(hexCoord);

		hexList.Add(createId(hexCoord), hex);
	}

	public string createId(HexCoordinates hexCoord)
	{
		return "Hex_" + hexCoord.q + "_" + hexCoord.r;
	}

	public bool isAdjacent(HexCoordinates hexCoord1, HexCoordinates hexCoord2)
	{
		return Hex.hexDistance(hexCoord1, hexCoord2) == 1;
	}

	public HexData retrieveHexData(HexCoordinates hexCoord)
	{
		string id = createId(hexCoord);
		return hexList[id];
	}

	public int cost(HexCoordinates from, HexCoordinates to)
	{
		string id = createId(to);
		return hexList[id].energy;
	}

	public int count
	{
		get { return (int) Mathf.Pow(radius * 2 + 1, 2); }
	}
}