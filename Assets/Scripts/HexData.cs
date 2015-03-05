using UnityEngine;
using System.Collections;

public class HexData : MonoBehaviour
{
	public int energy = 1;

	public HexCoordinates hexCoord { get; private set; }

	public void init(HexCoordinates hexCoord)
	{
		this.hexCoord = hexCoord;
		transform.localPosition = Hex.hexToWorld(hexCoord);
	}
}