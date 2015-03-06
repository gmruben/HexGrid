using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Model;
using PathFind;

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

			//var sp = _game.GamePieces.First();
			//var dp = _game.GamePieces.Last();
			
			//Tile start = new Tile(player.hexCoord.q, player.hexCoord.r); //_game.AllTiles.Single(o => o.X == sp.Location.X && o.Y == sp.Location.Y);
			//Tile destination = new Tile(hex.q, hex.r); //_game.AllTiles.Single(o => o.X == dp.Location.X && o.Y == dp.Location.Y);

			int px = player.hexCoord.q;
			int py = player.hexCoord.r;

			int hx = hex.q;
			int hy = hex.r;

			Tile start = grid.AllTiles.Single(o => o.X == px && o.Y == py);
			Tile destination = grid.AllTiles.Single(o => o.X == hx && o.Y == hy);

			Debug.Log(start.AllNeighbours.Count());

			Func<Tile, Tile, double> distance = (node1, node2) => 1;
			Func<Tile, double> estimate = t => Mathf.Sqrt(Mathf.Pow(t.X - destination.X, 2) + Mathf.Pow(t.Y - destination.Y, 2));
			
			PathFind.Path<Tile> path = PathFind.PathFind.FindPath(start, destination, distance, estimate);

			path.ToList().ForEach(logTile);
			//DrawPath(path);
		}
	}

	private void logTile(Tile tile)
	{
		Debug.Log(tile.X + " - " + tile.Y);
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