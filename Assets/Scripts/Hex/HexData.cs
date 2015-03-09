using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using PathFind;

/// <summary>
/// This class stores all the data for a hex. It implements the interface IHasNeighbours so we can
/// use it as node for the path finder
/// </summary>
public class HexData : MonoBehaviour, IHasNeighbours<HexData>
{
	public int energy = 1;
	public bool isEmpty { get; set; }
	
	private GoldPiece goldPiece;

	public AxialCoordinates hexCoord { get; private set; }

	public void init(AxialCoordinates hexCoord, bool hasGold)
	{
		this.hexCoord = hexCoord;
		isEmpty = true;

		//If the hex has a gold piece, create it
		if (hasGold)
		{
			goldPiece = EntityManager.instantiateGoldPiece();
			goldPiece.transform.position = transform.position;
		}
	}

	public IEnumerable<HexData> AllNeighbours { get; set; }
	public IEnumerable<HexData> Neighbours	//Neighbours list only has those neighbours that are empty
	{
		get	{ return AllNeighbours.Where(h => h.isEmpty); }
	}

	/// <summary>
	/// Sets the player on the hex.
	/// </summary>
	/// <param name="player">Player.</param>
	public void setPlayerOn(Unit player)
	{
		isEmpty = false;

		if (hasGold)
		{
			player.updateGold(1);
			goldPiece.take();
		}
	}

	public void FindNeighbours(Grid grid)
	{
		//Retrieve neighbours from the grid
		AllNeighbours = grid.retrieveNeighbours(hexCoord).Select(h => grid.retrieveHexData(h));
	}

	public bool hasGold
	{
		get { return goldPiece != null; }
	}
}