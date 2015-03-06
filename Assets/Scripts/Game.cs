using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
	public Camera gameCamera;

	public Player player;
	public Grid grid;

	public GameObject hexPrefab;

	public GameHUD gameHUD;

	private List<GameObject> hexList = new List<GameObject>();

	void Start()
	{
		init ();
	}

	public void init()
	{
		player.init();
		grid.init();

		gameHUD.init();

		HexCoordinates hexCoord = grid.retrieveRandomCoord();

		player.setPosition(hexCoord);
		drawRadius(hexCoord);
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
		{
			Vector3 position = gameCamera.ScreenToWorldPoint(Input.mousePosition);
			HexCoordinates hex = Hex.worldToHex(position);

			HexData hexData = grid.retrieveHexData(hex);
			if (grid.isAdjacent(player.hexCoord, hex) && hexData.energy <= player.energy)
			{
				player.moveTo(hexData);
				player.onMoveEnd += onMoveEnd;

				gameHUD.updateEnergy(player.energy);
				hideRadius();
			}
		}
	}

	private void drawRadius(HexCoordinates hex)
	{
		List<HexCoordinates> hexCoordList = Hex.movementRange(Hex.hexToCube(hex.V2), 2);
		for (int i = 0; i < hexCoordList.Count; i++)
		{
			if (grid.isCoordOnBounds(hexCoordList[i]) && player.hexCoord != hexCoordList[i])
			{
				GameObject hexInstance = GameObject.Instantiate(hexPrefab) as GameObject;
				
				hexInstance.transform.parent = transform;
				hexInstance.transform.localPosition = Hex.hexToWorld(hexCoordList[i]);

				hexList.Add(hexInstance);
			}
		}
	}

	private void hideRadius()
	{
		for (int i = 0; i < hexList.Count; i++)
		{
			GameObject.Destroy(hexList[i]);
		}
		hexList.Clear();
	}

	private void onMoveEnd()
	{
		player.onMoveEnd -= onMoveEnd;
		drawRadius(player.hexCoord);

		if (player.energy == 0)
		{
			gameHUD.endTurn();
			gameHUD.onEndTurn += onEndTurn;
		}
	}

	private void onEndTurn()
	{
		player.reset();
		gameHUD.updateEnergy(player.energy);
	}
}