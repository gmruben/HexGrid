using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
	private const int radius = 2;

	public GameObject hexPrefab;

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
			}
		}
	}

	public HexCoordinates retrieveRandomCoord()
	{
		int randomIndex = Random.Range(0, hexList.Count);
		List<HexData> dataList = new List<HexData>(hexList.Values);

		return dataList[randomIndex].hexCoord;
	}

	private void instantiateHex(HexCoordinates hexCoord)
	{
		HexData hex = (GameObject.Instantiate(hexPrefab) as GameObject).GetComponent<HexData>();

		hex.transform.parent = transform;
		hex.init(hexCoord);

		hexList.Add(createId(hexCoord), hex);
	}

	private string createId(HexCoordinates hexCoord)
	{
		return "Hex_" + hexCoord.q + "_" + hexCoord.r;
	}
}