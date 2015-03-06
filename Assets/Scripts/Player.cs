using UnityEngine;
using System;
using System.Collections;

public class Player : MonoBehaviour
{
	private const float speed = 3.5f;

	public event Action onMoveEnd;

	private Transform cachedTransform;

	private Vector3 startPosition;
	private Vector3 targetPosition;

	public int energy { get; private set; }
	public HexCoordinates hexCoord { get; private set; }

	public void init()
	{
		cachedTransform = transform;
		energy = 5;
	}

	public void reset()
	{
		energy = 5;
	}

	public void moveTo(HexData hexData)
	{
		//Update energy
		energy -= hexData.energy;

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

		cachedTransform.position = targetPosition;
		if (onMoveEnd != null) onMoveEnd();
	}

	public void setPosition(HexCoordinates hexCoord)
	{
		this.hexCoord = hexCoord;
		cachedTransform.localPosition = Hex.hexToWorld(hexCoord);
	}
}