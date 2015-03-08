using UnityEngine;
using System.Collections;

/// <summary>
/// This component can be added to any object, and a pool of those object can be created really easily
/// </summary>
public class PoolInstance : MonoBehaviour
{
	private bool _isAlive;

	public void init()
	{
		kill();
	}

	public void kill()
	{
		_isAlive = false;
		gameObject.SetActive(false);
	}

	public void revive()
	{
		_isAlive = true;
		gameObject.SetActive(true);
	}

	public bool isAlive
	{
		get { return _isAlive; }
	}
}