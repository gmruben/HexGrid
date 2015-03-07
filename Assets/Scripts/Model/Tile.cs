﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathFind;

namespace Model
{
    public class Tile : SpacialObject, IHasNeighbours<Tile>
    {
        public Tile(int x, int y)
            : base(x, y)
        {
            CanPass = true;
        }

        public bool CanPass { get; set; }

        public IEnumerable<Tile> AllNeighbours { get; set; }
		public IEnumerable<Tile> Neighbours
		{
			get
			{
				UnityEngine.Debug.Log(AllNeighbours.Count());

				return AllNeighbours; //.Where(o => o.CanPass);
			}
		}

        /*public void FindNeighbours(Tile[,] gameBoard)
        {
            var neighbours = new List<Tile>();

            var possibleExits = X % 2 == 0 ? EvenNeighbours : OddNeighbours;

            foreach (var vector in possibleExits)
            {
                var neighbourX = X + vector.X;
                var neighbourY = Y + vector.Y;

                if (neighbourX >= 0 && neighbourX < gameBoard.GetLength(0) && neighbourY >= 0 && neighbourY < gameBoard.GetLength(1))
                    neighbours.Add(gameBoard[neighbourX, neighbourY]);
            }

            AllNeighbours = neighbours;
        }*/

		public void FindNeighbours(Grid grid)
		{
			//AllNeighbours = new List<Tile>(grid.retrieveNeighbours(new HexCoordinates(X, Y)).Select(h => new Tile(h.q, h.r)).ToList());
			List<HexCoordinates> hexList = grid.retrieveNeighbours(new HexCoordinates(X, Y));
			List<Tile> tileList = new List<Tile>();

			for (int i = 0; i < hexList.Count; i++)
			{
				int x = hexList[i].q + 2;
				int y = hexList[i].r + 2;

				tileList.Add(grid.tileList[x, y]);
			}
			AllNeighbours = tileList;
		}

        public static List<Point> EvenNeighbours
        {
            get
            {
                return new List<Point>
                {
                    new Point(0, 1),
                    new Point(1, 1),
                    new Point(1, 0),
                    new Point(0, -1),
                    new Point(-1, 0),
                    new Point(-1, 1),
                };
            }
        }

        public static List<Point> OddNeighbours
        {
            get
            {
                return new List<Point>
                {
                    new Point(0, 1),
                    new Point(1, 0),
                    new Point(1, -1),
                    new Point(0, -1),
                    new Point(-1, 0),
                    new Point(-1, -1),
                };
            }
        }
    }
}