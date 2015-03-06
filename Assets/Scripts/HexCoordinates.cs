using UnityEngine;
using System.Collections;

public class HexCoordinates
{
	public int q {get; private set; }
	public int r {get; private set; }

	public HexCoordinates(int q, int r)
	{
		this.q = q;
		this.r = r;
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
		HexCoordinates coord = (HexCoordinates) obj;
		return coord.r == r && coord.q == q;
	}
	
	public static bool operator ==(HexCoordinates a, HexCoordinates b)
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
	
	public static bool operator !=(HexCoordinates a, HexCoordinates b)
	{
		return !(a == b);
	}
	
	/*public static SquareIndex operator -(SquareIndex index1, SquareIndex index2)
	{
		return new SquareIndex(index1.x - index2.x, index1.y - index2.y);
	}
	
	public float magnitude
	{
		get { return Mathf.Sqrt(x * x + y * y); }
	}
	
	public static SquareIndex zero
	{
		get { return new SquareIndex(0, 0); }
	}*/
}