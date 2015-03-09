using UnityEngine;
using System.Collections;

/// <summary>
/// Class for a Gold Piece. It plays an animation whenever a unit takes it.
/// </summary>
public class GoldPiece : MonoBehaviour
{
	private Animator animator;

	void Start ()
	{
		animator = GetComponent<Animator>();
	}
	
	public void take()
	{
		animator.Play("Take");
	}

	private void animationEnded()
	{
		GameObject.Destroy(gameObject);
	}
}