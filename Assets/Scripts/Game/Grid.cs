using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using PathFind;

/// <summary>
/// A grid contains a list of hexes. It has several different settable properties. It uses axial coordinates to store
/// the positions of the hexes.
/// </summary>
public class Grid : MonoBehaviour
{
	public event Action onCreated;

	public float hexSize = 1.25f;

	public int innerRadius = 1;				//The inner radius of the hex
	public int outerRadius = 3;				//The outer radius of the hex

	public float goldProbability = 0.5f;	//The probability of a gold piece appearing on a hex

	public GameObject[] hexPrefabList;		//An array with the different hex that can be instantiated
	public float[] hexProbabilityList;		//The probability of each hex

	public GameObject hexPrefab;

	private List<GameObject> highlightedHexList = new List<GameObject>();

	//We use a dictionary to store the map because it allows us to have maps with
	//any type of shape. I create a unique key for each hex based on its coordinates
	private Dictionary<string, HexData> hexList = new Dictionary<string, HexData>();

	public void init()
	{
		//Create a pool for the higlighted hexs, since we are going to be creating and destroying them very often
		PoolManager.instance.createPool (PoolManager.PoolIds.Hex, Resources.Load("Prefabs/Game/Hex") as GameObject, 50);

		StartCoroutine(instantiateGrid());
	}
	
	private IEnumerator instantiateGrid()
	{
		Vector3 center = Vector3.zero;

		//If the inner radius is zero, create the center hex
		if (innerRadius == 0)
		{
			Vector2 hexVector = HexMath.cubeToAxial(center);
			instantiateHex(new AxialCoordinates(hexVector));
		}

		for (int i = innerRadius; i <= outerRadius; i++)
		{
			List<Vector3> ring = HexMath.cubeRing(center, i);
			for (int j = 0; j < ring.Count; j++)
			{
				Vector2 hexVector = HexMath.cubeToAxial(ring[j]);
				instantiateHex(new AxialCoordinates(hexVector));

				//Wait 0.1 seconds between cells to make it look cool when instantiating the grid
				yield return new WaitForSeconds(0.1f);
			}
		}

		//Calculate the neighbours for each cell
		hexList.ToList().ForEach(o => o.Value.FindNeighbours(this));
		if (onCreated != null) onCreated();
	}

	/// <summary>
	/// Retrieves a random hex coord
	/// </summary>
	/// <returns>The hex coord.</returns>
	public AxialCoordinates retrieveRandomCoord()
	{
		//Get a random index from the hex list
		int randomIndex = UnityEngine.Random.Range(0, hexList.Count);
		List<HexData> dataList = new List<HexData>(hexList.Values);

		return dataList[randomIndex].hexCoord;
	}

	/// <summary>
	/// Retrieves all the neighbours for a hex.
	/// </summary>
	/// <returns>A list with the neighbours.</returns>
	/// <param name="hexCoord">The coordinates of the hex we want to retrieve the neighbours for.</param>
	public List<AxialCoordinates> retrieveNeighbours(AxialCoordinates hexCoord)
	{
		//Retrieve all the hexes in a radius of one
		List<AxialCoordinates> neighbours = new List<AxialCoordinates>();
		List<AxialCoordinates> hexList = HexMath.axialRadius(hexCoord.V2, 1);

		for (int i = 0; i < hexList.Count; i++)
		{
			//If the hex is not the current one and it is on bounds, add it to the list
			if (hexCoord != hexList[i] && isCoordOnBounds(hexList[i]))
			{
				neighbours.Add(hexList[i]);
			}
		}

		return neighbours;
	}

	/// <summary>
	/// Calculates if a hex coord is on the grid bounds
	/// </summary>
	/// <returns><c>true</c>, if coordinate on bounds was ised, <c>false</c> otherwise.</returns>
	/// <param name="hexCoord">Hex coordinate.</param>
	public bool isCoordOnBounds(AxialCoordinates hexCoord)
	{
		string id = createHexId(hexCoord);
		return hexList.ContainsKey(id);
	}

	/// <summary>
	/// Instantiates a hex in a position
	/// </summary>
	/// <param name="hexCoord">Hex coordinate.</param>
	private void instantiateHex(AxialCoordinates hexCoord)
	{
		//Calculate gold probablity
		bool hasGold = UnityEngine.Random.Range(0.0f, 1.0f) < goldProbability;

		HexData hex = (GameObject.Instantiate(retrieveRandomHex()) as GameObject).GetComponent<HexData>();

		hex.transform.parent = transform;
		hex.transform.localPosition = hexToWorld(hexCoord);

		hex.init(hexCoord, hasGold);
		hexList.Add(createHexId(hexCoord), hex);
	}

	/// <summary>
	/// Retrieves a random cell out of the list of cell types (easy, medium and hard)
	/// </summary>
	/// <returns>The GameObject for the cell.</returns>
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

	/// <summary>
	/// Creates a unique Hex Id to store the cell in the dictionary
	/// </summary>
	/// <returns>The Hex Id.</returns>
	/// <param name="hexCoord">The coordinates for the cell.</param>
	public string createHexId(AxialCoordinates hexCoord)
	{
		return "Hex_" + hexCoord.q + "_" + hexCoord.r;
	}

	/// <summary>
	/// Checks whether a hex is adjacent to another one or not
	/// </summary>
	/// <returns><c>true</c>, if the hex is adjacent, <c>false</c> otherwise.</returns>
	/// <param name="hexCoord1">First hex coord.</param>
	/// <param name="hexCoord2">Second hex coord.</param>
	public bool isAdjacent(AxialCoordinates hexCoord1, AxialCoordinates hexCoord2)
	{
		//If the distance between hexes is one, they are adjacent
		return HexMath.axialDistance(hexCoord1.V2, hexCoord2.V2) == 1;
	}

	public HexData retrieveHexData(AxialCoordinates hexCoord)
	{
		string id = createHexId(hexCoord);
		return hexList[id];
	}

	/// <summary>
	/// It shows highlighted cells around a hex and inside a radius and that are reachable with the specified energy
	/// </summary>
	/// <param name="hex">Hex coordinate.</param>
	/// <param name="energy">Energy.</param>
	public void showHighlightedCells(AxialCoordinates hex, int energy)
	{
		HexData startHexData = retrieveHexData(hex);

		//Retrieve all hexes inside a radius that equals the available energy
		List<AxialCoordinates> hexCoordList = HexMath.cubeRadius(HexMath.axialToCube(hex.V2), energy);
		for (int i = 0; i < hexCoordList.Count; i++)
		{
			//If the current hex is on bounds and is not the center
			if (isCoordOnBounds(hexCoordList[i]) && hex != hexCoordList[i])
			{
				//If the current hex is empry
				HexData hexData = retrieveHexData(hexCoordList[i]);
				if (hexData.isEmpty)
				{
					//Calculate the path to that hex
					Path<HexData> path = PathFind.PathFind.FindPathHexData(startHexData, hexData);
					//If a path exists and the energy necessary to move to that hex is less than the available energy, that hex is reachable
					if (path != null && path.TotalCost <= energy)
					{
						//Retreive an instance from the pool
						GameObject hexInstance = PoolManager.instance.retrievePoolInstance(PoolManager.PoolIds.Hex).gameObject;
						
						hexInstance.transform.parent = transform;
						hexInstance.transform.localPosition = hexToWorld(hexCoordList[i]);

						//Add that instance to the list
						highlightedHexList.Add(hexInstance);
					}
				}
			}
		}
	}

	/// <summary>
	/// Hides the current list of highlighted cells.
	/// </summary>
	public void hideHighlightedCells()
	{
		//Iterate through the hexes and destroy the instances (using the pool manager)
		for (int i = 0; i < highlightedHexList.Count; i++)
		{
			PoolManager.instance.destroyInstance(highlightedHexList[i].GetComponent<PoolInstance>());
		}
		highlightedHexList.Clear();
	}

	/// <summary>
	/// Converts a hex coord into a world position
	/// </summary>
	/// <returns>The position in world coordinates.</returns>
	/// <param name="hexCoord">Hex coordinate.</param>
	public Vector3 hexToWorld(AxialCoordinates hexCoord)
	{
		return HexMath.hexToWorld(hexCoord.V2, hexSize);
	}

	/// <summary>
	/// Converts a world position into a hex coord
	/// </summary>
	/// <returns>The hex coordinates.</returns>
	/// <param name="position">World position.</param>
	public AxialCoordinates worldToHex(Vector3 position)
	{
		Vector2 axialCoordinates = HexMath.worldToHex(position, hexSize);
		return new AxialCoordinates((int) axialCoordinates.x, (int) axialCoordinates.y);
	}
}