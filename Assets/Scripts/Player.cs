using UnityEngine;
using System;
using System.Collections;

public class Player : MonoBehaviour
{
	private const float speed = 3.5f;

	public event Action onMoveEnd;
	public event Action<int> onGetGold;
	public event Action<int> onUpdateEnergy;

	public int initialEnergy = 5;

	private Transform cachedTransform;

	private Vector3 startPosition;
	private Vector3 targetPosition;

	public int energy { get; private set; }
	public HexCoordinates hexCoord { get; private set; }

	private HexData hexData;
	private int gold;

	private Grid grid;

	public void init(Grid grid)
	{
		this.grid = grid;
		cachedTransform = transform;

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

		StartCoroutine(updateMove());
	}

	private IEnumerator updateMove()
	{
		Vector3 direction = (targetPosition - startPosition).normalized;

		while ((targetPosition - cachedTransform.position).sqrMagnitude > 0.005f)
		{
			cachedTransform.position += direction * speed * Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);
		}

		hexData.isEmpty = false;
		hexData.playerOn(this);

		cachedTransform.position = targetPosition;
		if (onMoveEnd != null) onMoveEnd();
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