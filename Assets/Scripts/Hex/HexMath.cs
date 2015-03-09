using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class has different functions to operate with hex coordinates. We use two different types
/// of coordinate systems, cube and axial. To learn more about this, check this link: http://www.redblobgames.com/grids/hexagons
/// </summary>
public class HexMath : MonoBehaviour
{
	/// <summary>
	/// Converts cube cordinates to axial coordinates
	/// </summary>
	/// <returns>The to hex.</returns>
	/// <param name="cubeCoord">Cube coordinates.</param>
	public static Vector2 cubeToAxial(Vector3 cubeCoord)
	{
		float q = cubeCoord.x;
		float r = cubeCoord.z;

		return new Vector2(q, r);
	}

	/// <summary>
	/// Converts axial cordinates to cube coordinates
	/// </summary>
	/// <returns>The to hex.</returns>
	/// <param name="axialCoord">Axial coordinates.</param>
	public static Vector3 axialToCube(Vector2 axialCoord)
	{
		float x = axialCoord.x;
		float z = axialCoord.y;
		float y = -x - z;

		return new Vector3(x, y, z);
	}

	/// <summary>
	/// Calculates all the hexes inside a radius
	/// </summary>
	/// <returns>A list with the coordinates of all the hexes inside the range.</returns>
	/// <param name="index">The center of the radius.</param>
	/// <param name="radius">Radius.</param>
	public static List<AxialCoordinates> cubeRadius(Vector3 center, int radius)
	{
		List<AxialCoordinates> hexList = new List<AxialCoordinates>();

		for (int x = -radius; x <= radius; x++)
		{
			for (int y = Mathf.Max(-radius, -x - radius); y <= Mathf.Min(radius, -x + radius); y++)
			{
				int z = -x - y;
				Vector3 cubeVector = center + new Vector3(x, y, z);
				Vector2 hexVector = cubeToAxial(cubeVector);

				hexList.Add(new AxialCoordinates((int) hexVector.x, (int) hexVector.y));
			}
		}

		return hexList;
	}

	/// <summary>
	/// Calculates all the hexes inside a radius
	/// </summary>
	/// <returns>A list with the coordinates of all the hexes inside the range.</returns>
	/// <param name="index">The center of the radius.</param>
	/// <param name="radius">Radius.</param>
	public static List<AxialCoordinates> axialRadius(Vector2 center, int radius)
	{
		return cubeRadius(axialToCube(center), radius);
	}

	/// <summary>
	/// Converts axial cordinates to world coordinates
	/// </summary>
	/// <returns>The position in world coordinates.</returns>
	/// <param name="axialCoord">Axial coordinates.</param>
	/// <param name="hexSize">The size of an hexagon.</param>
	public static Vector3 hexToWorld(Vector2 axialCoord, float hexSize)
	{
		float q = (float) axialCoord.x;
		float r = (float) axialCoord.y;

		float posx = (hexSize * (3.0f / 2.0f)) * q;
		float posz = hexSize * Mathf.Sqrt(3.0f) * (r + (q / 2.0f));

		return new Vector3(posx, 0, -posz);
	}

	/// <summary>
	/// Converts world coordinates to axial coordinates
	/// </summary>
	/// <returns>The hex coordinates for that position.</returns>
	/// <param name="position">Position in worldl coordinates.</param>
	/// <param name="hexSize">The size of an hexagon.</param>
	public static Vector2 worldToHex(Vector3 position, float hexSize)
	{
		float q = position.x * 2.0f / 3.0f / hexSize;
		float r = (-position.x / 3.0f + Mathf.Sqrt(3.0f) / 3.0f * -position.z) / hexSize;

		return axialRound(new Vector2(q, r));
	}

	/// <summary>
	/// Rounds a cube coordinate
	/// </summary>
	/// <returns>The rounded coordinate.</returns>
	/// <param name="cube">Cube coordinates.</param>
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

	/// <summary>
	/// Rounds an axial coordinate
	/// </summary>
	/// <returns>The rounded coordinate.</returns>
	/// <param name="hex">Axial coordinate.</param>
	public static Vector2 axialRound(Vector2 axialCoord)
	{
		return cubeToAxial(cubeRound(axialToCube(axialCoord)));
	}

	/// <summary>
	/// Calculates the distance between two points in cube coordinates
	/// </summary>
	/// <returns>The distance between points.</returns>
	/// <param name="a">The first point.</param>
	/// <param name="b">The second point.</param>
	public static int cubeDistance(Vector3 a, Vector3 b)
	{
		return Mathf.FloorToInt((Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2);
	}

	/// <summary>
	/// Calculates the distance between two points in axial coordinates
	/// </summary>
	/// <returns>The distance between points.</returns>
	/// <param name="a">The first point.</param>
	/// <param name="b">The second point.</param>
	public static int axialDistance(Vector2 a, Vector2 b)
	{
		//Convert coordinates to cube and calculate distance
		Vector3 ac = axialToCube(a);
		Vector3 bc = axialToCube(b);

		return cubeDistance(ac, bc);
	}
	
	public static List<Vector3> cubeRing(Vector3 center, int radius)
	{
		List<Vector3> results = new List<Vector3>();
		Vector3 cube = center + cubeDirections[4] * radius;
		
		for (int i = 0; i < 6; i++)
		{
			for (int j = 0; j < radius; j++)
			{
				results.Add(cube);
				cube = cubeNeighbour(cube, i);
			}
		}
		
		return results;
	}

	/// <summary>
	/// It calculates a neighbour for a cube coordinate
	/// </summary>
	/// <returns>The neighbour for that coordinate.</returns>
	/// <param name="cube">Cube coordinates.</param>
	/// <param name="direction">The direction we want the neighbour to be from.</param>
	public static Vector3 cubeNeighbour(Vector3 cubeCoord, int direction)
	{
		return cubeCoord + cubeDirections[direction];
	}

	/// <summary>
	/// An array storing the directions in axial coordinates for each neighbour
	/// </summary>
	private static Vector2[] axialDirections = new Vector2[6]
	{
		new Vector2(1, 1),
		new Vector2(1, -1),
		new Vector2(0, -1),
		new Vector2(-1, 0),
		new Vector2(-1, 1),
		new Vector2(0, 1)
	};

	/// <summary>
	/// An array storing the directions in cube coordinates for each neighbour
	/// </summary>
	private static Vector3[] cubeDirections = new Vector3[6]
	{
		new Vector3(1, -1, 0),
		new Vector3(1, 0, -1),
		new Vector3(0, 1, -1),
		new Vector3(-1, 1, 0),
		new Vector3(-1, 0, 1),
		new Vector3(0, -1, 1)
	};
}