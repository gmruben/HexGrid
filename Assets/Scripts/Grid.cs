using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Model;
using PathFind;
using System.Linq;

public class Grid : MonoBehaviour
{
	public event Action onCreated;

	public int innerRadius = 1;
	public int outerRadius = 3;

	public float goldProbability = 0.1f;

	public GameObject[] hexPrefabList;
	public float[] hexProbabilityList;

	public GameObject hexPrefab;

	private List<GameObject> highlightedHexList = new List<GameObject>();

	//We use a dictionary to store the map because it allows us to have maps with
	//any type of shape. I create a unique key for each hex based on its coordinates
	private Dictionary<string, HexData> hexList = new Dictionary<string, HexData>();

	public void init()
	{
		StartCoroutine(instantiateGrid());
	}

	private IEnumerator instantiateGrid()
	{
		for (int i = innerRadius; i <= outerRadius; i++)
		{
			Vector3 center = Vector3.zero;
			List<Vector3> ring = Hex.cubeRing(center, i);

			for (int j = 0; j < ring.Count; j++)
			{
				Vector2 hexVector = Hex.cubeToHex(ring[j]);
				instantiateHex(new HexCoordinates((int) hexVector.x, (int) hexVector.y));
				
				yield return new WaitForSeconds(0.1f);
			}
		}
		
		hexList.ToList().ForEach(o => o.Value.FindNeighbours(this));
		if (onCreated != null) onCreated();
	}

	public HexCoordinates retrieveRandomCoord()
	{
		int randomIndex = UnityEngine.Random.Range(0, hexList.Count);
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
		HexData hex = (GameObject.Instantiate(retrieveRandomHex()) as GameObject).GetComponent<HexData>();

		hex.transform.parent = transform;
		hex.init(hexCoord);

		hexList.Add(createId(hexCoord), hex);
	}

	private GameObject retrieveRandomHex()
	{
		int index = 0;
		float random = UnityEngine.Random.Range(0.0f, 1.0f);
		float acc = hexProbabilityList[0];
		
		for (int i = 0; i < hexProbabilityList.Length; i++)
		{
			if (random <= acc)
			{
				index = i;
				break;
			}
			else
			{
				acc += hexProbabilityList[i + 1];
			}
		}

		return hexPrefabList[index];
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
		get { return hexList.Count; }
	}

	public void showHighlightedCells(HexCoordinates hex, int energy)
	{
		HexData startHexData = retrieveHexData(hex);

		List<HexCoordinates> hexCoordList = Hex.movementRange(Hex.hexToCube(hex.V2), energy);
		for (int i = 0; i < hexCoordList.Count; i++)
		{
			if (isCoordOnBounds(hexCoordList[i]) && hex != hexCoordList[i])
			{
				HexData hexData = retrieveHexData(hexCoordList[i]);
				if (hexData.isEmpty)
				{
					Path<HexData> path = PathFind.PathFind.FindPathHexData(startHexData, hexData);

					if (path.TotalCost <= energy)
					{
						GameObject hexInstance = GameObject.Instantiate(hexPrefab) as GameObject;
						
						hexInstance.transform.parent = transform;
						hexInstance.transform.localPosition = Hex.hexToWorld(hexCoordList[i]);
						
						highlightedHexList.Add(hexInstance);
					}
				}
			}
		}
	}

	public void hideHighlightedCells()
	{
		for (int i = 0; i < highlightedHexList.Count; i++)
		{
			GameObject.Destroy(highlightedHexList[i]);
		}
		highlightedHexList.Clear();
	}
}