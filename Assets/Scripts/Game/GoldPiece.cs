using UnityEngine;
using System.Collections;

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