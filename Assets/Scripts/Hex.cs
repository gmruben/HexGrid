using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hex : MonoBehaviour
{
	private const float size = 1.25f;

	public static Vector2 cubeToHex(Vector3 cubeCoord)
	{
		float q = cubeCoord.x;
		float r = cubeCoord.z;

		return new Vector2(q, r);
	}
			
	public static Vector3 hexToCube(Vector2 hexCoord)
	{
		float x = hexCoord.x;
		float z = hexCoord.y;
		float y = -x - z;

		return new Vector3(x, y, z);
	}

	public static List<HexCoordinates> movementRange(Vector3 index, int radius)
	{
		List<HexCoordinates> hexList = new List<HexCoordinates>();

		for (int x = -radius; x <= radius; x++)
		{
			for (int y = Mathf.Max(-radius, -x - radius); y <= Mathf.Min(radius, -x + radius); y++)
			{
				int z = -x - y;
				Vector3 cubeVector = index + new Vector3(x, y, z);
				Vector2 hexVector = cubeToHex(cubeVector);

				hexList.Add(new HexCoordinates((int) hexVector.x, (int) hexVector.y));
			}
		}

		return hexList;
	}

	public static Vector3 hexToWorld(HexCoordinates hex)
	{
		float q = (float) hex.q;
		float r = (float) hex.r;

		float posx = (size * (3.0f / 2.0f)) * q;
		float posz = size * Mathf.Sqrt(3.0f) * (r + (q / 2.0f));

		return new Vector3(posx, 0, -posz);
	}

	public static HexCoordinates worldToHex(Vector3 position)
	{
		float q = position.x * 2.0f / 3.0f / size;
		float r = (-position.x / 3.0f + Mathf.Sqrt(3.0f) / 3.0f * -position.z) / size;

		return hexRound(new Vector2(q, r));
	}

	public static Vector3 cubeRound(Vector3 cube)
	{
		int rx = Mathf.RoundToInt(cube.x);
		int ry = Mathf.RoundToInt(cube.y);
		int rz = Mathf.RoundToInt(cube.z);
			
		float x_diff = Mathf.Abs(rx - cube.x);
		float y_diff = Mathf.Abs(ry - cube.y);
		float z_diff = Mathf.Abs(rz - cube.z);
			
		if (x_diff > y_diff && x_diff > z_diff)
		{
			rx = -ry - rz;
		}
		else if (y_diff > z_diff)
		{
			ry = -rx - rz;
		}
		else
		{
			rz = -rx - ry;
		}
							
		return new Vector3(rx, ry, rz);
	}

	public static HexCoordinates hexRound(Vector2 hex)
	{
		Vector2 hexVector = cubeToHex(cubeRound(hexToCube(hex)));
		return new HexCoordinates((int) hexVector.x, (int) hexVector.y);
	}

	public static int cubeDistance(Vector3 a, Vector3 b)
	{
		return Mathf.FloorToInt((Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2);
	}

	public static int hexDistance(HexCoordinates a, HexCoordinates b)
	{
		Vector3 ac = hexToCube(a.V2);
		Vector3 bc = hexToCube(b.V2);

		return cubeDistance(ac, bc);
	}
}