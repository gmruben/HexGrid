using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hex : MonoBehaviour
{
	public Vector2 cubeToHex(Vector3 cubeCoord)
	{
		float q = cubeCoord.x;
		float r = cubeCoord.z;

		return new Vector2(q, r);
	}
			
	public Vector3 hexToCube(Vector2 hexCoord)
	{
		float x = hexCoord.x;
		float z = hexCoord.y;
		float y = -x - z;

		return new Vector3(x, y, z);
	}

	public float cubeDistance(Vector3 a, Vector3 b)
	{
		return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2;
	}

	public float hexDistance(Vector2 a, Vector2 b)
	{
		Vector3 ac = hexToCube(a);
		Vector3 bc = hexToCube(b);

		return cubeDistance(ac, bc);
	}

	public void movementRange(Vector3 index, int radius)
	{
		List<Vector3> results = new List<Vector3>();

		for (int x = -radius; x <= radius; x++)
		{
			for (int y = Mathf.Max(-radius, -x - radius); y <= Mathf.Min(radius, -x + radius); y++)
			{
				int z = -x - y;
				results.Add(index + new Vector3(x, y, z));
			}
		}
	}
}