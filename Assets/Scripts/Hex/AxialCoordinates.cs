using UnityEngine;
using System.Collections;

/// <summary>
/// This class stores a point in axial coordinates.
/// </summary>
public class AxialCoordinates
{
	public int q { get; private set; }
	public int r { get; private set; }

	public AxialCoordinates(int q, int r)
	{
		this.q = q;
		this.r = r;
	}

	public AxialCoordinates(Vector2 coord)
	{
		this.q = (int) coord.x;
		this.r = (int) coord.y;
	}

	public override string ToString()
	{
		return q + ", " + r;
	}

	public Vector2 V2
	{
		get { return new Vector2(q, r); }
	}
	
	public override bool Equals(object obj)
	{
		AxialCoordinates coord = (AxialCoordinates) obj;
		return coord.r == r && coord.q == q;
	}
	
	public static bool operator ==(AxialCoordinates a, AxialCoordinates b)
	{
		// If both are null, or both are same instance, return true.
		if (System.Object.ReferenceEquals(a, b))
		{
			return true;
		}
		
		// If one is null, but not both, return false.
		if (((object)a == null) || ((object)b == null))
		{
			return false;
		}
		
		// Return true if the fields match:
		return a.r == b.r && a.q == b.q;
	}
	
	public static bool operator !=(AxialCoordinates a, AxialCoordinates b)
	{
		return !(a == b);
	}

	public static AxialCoordinates operator +(AxialCoordinates a, AxialCoordinates b)
	{
		return new AxialCoordinates(a.r + b.q, a.r + b.q);
	}

	public static AxialCoordinates operator -(AxialCoordinates a, AxialCoordinates b)
	{
		return new AxialCoordinates(a.r - b.q, a.r - b.q);
	}
}