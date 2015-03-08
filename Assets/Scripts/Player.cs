using UnityEngine;
using System;
using System.Collections;

public class Player : MonoBehaviour
{
	public event Action onMoveEnd;
	public event Action<int> onGetGold;
	public event Action<int> onUpdateEnergy;

	public float moveSpeed = 3.5f;
	public int initialEnergy = 5;

	private Transform cachedTransform;

	private Vector3 startPosition;
	private Vector3 targetPosition;

	public int energy { get; private set; }
	public HexCoordinates hexCoord { get; private set; }

	private HexData hexData;
	private int gold;

	private Grid grid;
	private MoveController moveController;

	public void init(Grid grid)
	{
		this.grid = grid;
		cachedTransform = transform;

		moveController = new MoveController(moveSpeed, cachedTransform);

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
		grid.retrieveHexData(hexCoord).isEmpty = true;

		this.hexData = hexData;
		this.hexCoord = hexData.hexCoord;

		startPosition = cachedTransform.position;
		targetPosition = Hex.hexToWorld(hexData.hexCoord);

		//StartCoroutine(updateMove());
		moveController.moveTo(hexData.hexCoord);
		moveController.onMoveEnd += onControllerMoveEnd;
	}

	private void onControllerMoveEnd()
	{
		moveController.onMoveEnd -= onControllerMoveEnd;

		hexData.isEmpty = false;
		hexData.playerOn(this);

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

	public void setPosition(HexCoordinates hexCoord)
	{
		//Update hex data
		grid.retrieveHexData(hexCoord).isEmpty = false;

		this.hexCoord = hexCoord;
		cachedTransform.localPosition = Hex.hexToWorld(hexCoord);
	}

	private void updateEnergy(int value)
	{
		energy = value;
		if (onUpdateEnergy != null) onUpdateEnergy(energy);
	}
}