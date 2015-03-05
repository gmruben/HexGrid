using UnityEngine;
using System;
using System.Collections;

public class MoveController : MonoBehaviour
{
	private readonly float speed = 5;

	public event Action onMoveEnd;

	private Transform cachedTransform;

	private Vector3 startPosition;
	private Vector3 targetPosition;

	public MoveController(Transform cachedTransform)
	{
		this.cachedTransform = cachedTransform;
	}

	public void moveTo(HexCoordinates hexCoord)
	{	
		startPosition = cachedTransform.position;
		targetPosition = Hex.hexToWorld(hexCoord);
		
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
}