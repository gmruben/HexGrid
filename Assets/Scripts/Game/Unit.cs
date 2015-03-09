using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// A unit can move through the hex grid. It has e fixed amount of energy that uses to move from one hex to another.
/// </summary>
public class Unit : MonoBehaviour
{
	public event Action onMoveEnd;
	public event Action<int> onGetGold;
	public event Action<int> onUpdateEnergy;

	public float moveSpeed = 5.0f;
	public int initialEnergy = 5;

	private Transform cachedTransform;

	public int energy { get; private set; }

	private HexData hexData;
	private int gold;

	private Grid grid;
	private MoveController moveController;

	public void init(Grid grid)
	{
		this.grid = grid;
		cachedTransform = transform;

		moveController = new MoveController(grid, cachedTransform, moveSpeed);

		energy = initialEnergy;
		gold = 0;
	}

	public void reset()
	{
		//Update energy
		updateEnergy(initialEnergy);
	}

	public void moveTo(HexData hexData)
	{
		//Update energy
		updateEnergy(energy - hexData.energy);

		//Update hex data
		this.hexData.isEmpty = true;
		this.hexData = hexData;

		moveController.moveTo(hexData.hexCoord);
		moveController.onMoveEnd += onControllerMoveEnd;
	}

	private void onControllerMoveEnd()
	{
		moveController.onMoveEnd -= onControllerMoveEnd;

		hexData.setPlayerOn(this);

		if (onMoveEnd != null) onMoveEnd();
	}

	void Update()
	{
		moveController.update(Time.deltaTime);
	}

	public void updateGold(int value)
	{
		gold += value;
		if (onGetGold != null) onGetGold(gold);
	}

	public void setPosition(AxialCoordinates hexCoord)
	{
		//Update hex data
		hexData = grid.retrieveHexData(hexCoord);
		hexData.setPlayerOn(this);

		cachedTransform.localPosition = grid.hexToWorld(hexCoord);
	}

	private void updateEnergy(int value)
	{
		energy = value;
		if (onUpdateEnergy != null) onUpdateEnergy(energy);
	}

	public AxialCoordinates hexCoord
	{
		get { return hexData.hexCoord; }
	}
}