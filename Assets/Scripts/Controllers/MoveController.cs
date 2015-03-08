using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// This class controls the movement of a unit. This controller moves the unit in a straight line from
/// the start position to the target position
/// </summary>
public class MoveController
{
	private readonly float speed;

	public event Action onMoveEnd;

	private Transform cachedTransform;

	private Vector3 startPosition;
	private Vector3 targetPosition;

	private bool inMove = false;
	private Vector3 moveDirection;

	public MoveController(float speed, Transform cachedTransform)
	{
		this.speed = speed;
		this.cachedTransform = cachedTransform;
	}

	public void moveTo(HexCoordinates hexCoord)
	{	
		startPosition = cachedTransform.position;
		targetPosition = Hex.hexToWorld(hexCoord);

		inMove = true;
		moveDirection = (targetPosition - startPosition).normalized;
	}
	
	public void update(float deltaTime)
	{
		if (inMove)
		{
			if ((targetPosition - cachedTransform.position).sqrMagnitude > 0.005f)
			{
				cachedTransform.position += moveDirection * speed * Time.deltaTime;
			}
			else
			{
				inMove = false;
				cachedTransform.position = targetPosition;

				if (onMoveEnd != null) onMoveEnd();
			}
		}
	}
}