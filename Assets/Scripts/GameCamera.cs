using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour
{
	public static Camera cachedCamera;

	void Start()
	{
		cachedCamera = camera;
	}
}