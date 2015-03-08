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

	private Grid grid;
	private Transform transform;

	private Vector3 startPosition;
	private Vector3 targetPosition;

	private bool inMove = false;
	private Vector3 moveDirection;

	public MoveController(Grid grid, Transform transform, float speed)
	{
		this.grid = grid;
		this.speed = speed;
		this.transform = transform;
	}

	public void moveTo(HexCoordinates hexCoord)
	{	
		startPosition = transform.position;
		targetPosition = grid.hexToWorld(hexCoord);

		inMove = true;
		moveDirection = (targetPosition - startPosition).normalized;
	}
	
	public void update(float deltaTime)
	{
		if (inMove)
		{
			if ((targetPosition - transform.position).sqrMagnitude > 0.005f)
			{
				transform.position += moveDirection * speed * Time.deltaTime;
			}
			else
			{
				inMove = false;
				transform.position = targetPosition;

				if (onMoveEnd != null) onMoveEnd();
			}
		}
	}
}