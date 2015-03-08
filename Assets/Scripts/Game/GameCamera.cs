using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour
{
	//Static reference to the Game Camera so we can access it easily
	public static Camera cachedCamera;

	void Start()
	{
		cachedCamera = camera;
	}
}