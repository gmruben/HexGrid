using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class TurnOverlay : MonoBehaviour
{
	public event Action onAnimationEnd;

	public Text title;

	public void init(string message)
	{
		title.text = message;
	}

	private void animationEnded()
	{
		if (onAnimationEnd != null) onAnimationEnd();
	}
}