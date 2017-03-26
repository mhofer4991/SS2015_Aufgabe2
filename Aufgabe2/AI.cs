//-----------------------------------------------------------------------
// <copyright file="AI.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>Represents a battle ship player, which owns artificial intelligence and therefore doesn't need any user input.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a battle ship player, which owns artificial intelligence and therefore doesn't need any user input.
    /// </summary>
    public class AI : Player
    {
        /// <summary> Used for getting random coordinates. </summary>
        private Random rand;

        /// <summary>
        /// Initializes a new instance of the <see cref="AI"/> class.
        /// </summary>
        /// <param name="name">Name of the AI.</param>
        /// <param name="map">Map of the AI.</param>
        /// <param name="enemyMap">Hostile map of the AI.</param>
        public AI(string name, GameMap map, GameMap enemyMap) : base(name, map, enemyMap)
        {
            this.rand = new Random();
        }

        /// <summary>
        /// Gets the coordinates on the hostile map, where the AI assumes a ship.
        /// </summary>
        /// <returns> An integer array, which contains the coordinates. </returns>
        public override int[] Move()
        {
            // If the last coordinates were not successful, get a random position, which could contain a ship yet.
            if (this.LastHit == null)
            {
                int[,] emptyCoords = this.GetEmptyCoordinates();

                int index = this.rand.Next(0, emptyCoords.GetUpperBound(0) + 1);

                return new int[] { emptyCoords[index, 0], emptyCoords[index, 1] };
            }
            else
            {
                // If the last coordinates were successful, choose random from all positions, where the remaining ship could hide.
                int[] pos = new int[] { this.LastHit[0], this.LastHit[1] };

                int[,] aC = this.GetPossibleCoordinates(pos[0], pos[1]);

                int index = this.rand.Next(0, aC.GetUpperBound(0) + 1);

                pos[0] = aC[index, 0];
                pos[1] = aC[index, 1];

                while (!this.IsValidCoordinate(pos[0], pos[1]))
                {
                    index = this.rand.Next(0, aC.GetUpperBound(0) + 1);

                    pos[0] = aC[index, 0];
                    pos[1] = aC[index, 1];
                }

                return pos;
            }
        }

        /// <summary>
        /// Scans the hostile map and delivers all coordinates, which could contain a ship yet.
        /// </summary>
        /// <returns> An two-dimensional array, which contains all possible positions. </returns>
        private int[,] GetEmptyCoordinates()
        {
            int[,] temp = new int[(this.EnemyMap.Width * this.EnemyMap.Height), 2];
            int index = 0;

            for (int i = 0; i < this.EnemyMap.Width; i++)
            {
                for (int j = 0; j < this.EnemyMap.Height; j++)
                {
                    if (this.IsValidCoordinate(i, j))
                    {
                        temp[index, 0] = i;
                        temp[index, 1] = j;

                        index++;
                    }
                }
            }

            int[,] emptyCoords = new int[index, 2];
            Array.Copy(temp, emptyCoords, emptyCoords.Length);

            return emptyCoords;
        }

        /// <summary>
        /// Looks at a given position for all potential coordinates, which could contain a ship.
        /// </summary>
        /// <param name="x">X - coordinate of the given position.</param>
        /// <param name="y">Y - coordinate of the given position.</param>
        /// <returns> An two-dimensional array, which contains all possible positions. </returns>
        private int[,] GetPossibleCoordinates(int x, int y)
        {
            // Gets the adjacent coordinates for the given position.
            int[,] aC = this.GetAdjacentCoordinates(x, y);

            // Checks, if a adjacent coordinate is also hit, which would then form a row with the given position.
            for (int i = 0; i < aC.GetUpperBound(0) + 1; i++)
            {
                if (this.EnemyMap.Field[aC[i, 0], aC[i, 1]] == GameMap.FieldHit)
                {
                    // Is the formed row horizontally or vertically?
                    if (aC[i, 0] == x)
                    {
                        return this.FindNextCoordinates(x, y, Ship.ShipOrientation.Vertical);
                    }
                    else if (aC[i, 1] == y)
                    {
                        return this.FindNextCoordinates(x, y, Ship.ShipOrientation.Horizontal);
                    }
                }
            }

            return aC;
        }

        /// <summary>
        /// Delivers all coordinates, which border directly with the given position.
        /// </summary>
        /// <param name="x">X - coordinate of the given position.</param>
        /// <param name="y">Y - coordinate of the given position.</param>
        /// <returns> A two-dimensional array, which contains all adjacent positions. </returns>
        private int[,] GetAdjacentCoordinates(int x, int y)
        {
            int[,] positions;

            if (x > 0 && x < (this.EnemyMap.Width - 1))
            {
                if (y > 0 && y < (this.EnemyMap.Height - 1))
                {
                    positions = new int[4, 2];
                    positions[0, 0] = x;
                    positions[1, 0] = x + 1;
                    positions[2, 0] = x;
                    positions[3, 0] = x - 1;

                    positions[0, 1] = y - 1;
                    positions[1, 1] = y;
                    positions[2, 1] = y + 1;
                    positions[3, 1] = y;
                }
                else
                {
                    positions = new int[3, 2];
                    positions[0, 0] = x - 1;
                    positions[1, 0] = x;
                    positions[2, 0] = x + 1;

                    if (y > 0)
                    {
                        positions[0, 1] = y;
                        positions[1, 1] = y - 1;
                        positions[2, 1] = y;
                    }
                    else
                    {
                        positions[0, 1] = y;
                        positions[1, 1] = y + 1;
                        positions[2, 1] = y;
                    }
                }
            }
            else
            {
                if (y > 0 && y < (this.EnemyMap.Height - 1))
                {
                    positions = new int[3, 2];
                    positions[0, 1] = y - 1;
                    positions[1, 1] = y;
                    positions[2, 1] = y + 1;

                    if (x > 0)
                    {
                        positions[0, 0] = x;
                        positions[1, 0] = x - 1;
                        positions[2, 0] = x;
                    }
                    else
                    {
                        positions[0, 0] = x;
                        positions[1, 0] = x + 1;
                        positions[2, 0] = x;
                    }
                }
                else
                {
                    positions = new int[2, 2];

                    if (x > 0 && y > 0)
                    {
                        positions[0, 0] = x - 1;
                        positions[1, 0] = x;

                        positions[0, 1] = y;
                        positions[1, 1] = y - 1;
                    }
                    else if (x > 0 && y < (this.EnemyMap.Height - 1))
                    {
                        positions[0, 0] = x - 1;
                        positions[1, 0] = x;

                        positions[0, 1] = y;
                        positions[1, 1] = y + 1;
                    }
                    else if (x < (this.EnemyMap.Width - 1) && y > 0)
                    {
                        positions[0, 0] = x + 1;
                        positions[1, 0] = x;

                        positions[0, 1] = y;
                        positions[1, 1] = y - 1;
                    }
                    else
                    {
                        positions[0, 0] = x + 1;
                        positions[1, 0] = x;

                        positions[0, 1] = y;
                        positions[1, 1] = y + 1;
                    }
                }
            }

            return positions;
        }

        /// <summary>
        /// Looks by the given position in a specific orientation for empty coordinates.
        /// </summary>
        /// <param name="x">X - coordinate of the given position.</param>
        /// <param name="y">Y - coordinate of the given position.</param>
        /// <param name="or">Direction, where the coordinates should be checked.</param>
        /// <returns> An two-dimensional array, which contains all adjacent positions. </returns>
        private int[,] FindNextCoordinates(int x, int y, Ship.ShipOrientation or)
        {
            int[,] newP = new int[1, 2];

            if (or == Ship.ShipOrientation.Horizontal)
            {
                // Start at the given position.
                newP[0, 1] = y;

                int i = x;

                // Go left until the field's value does not contain a hit.
                while (i > 0 && this.EnemyMap.Field[i, y] == GameMap.FieldHit)
                {
                    i--;
                }

                // If it's a valid coordinate, it could contain a part of a ship.
                if (this.IsValidCoordinate(i, y))
                {
                    newP[0, 0] = i;
                }
                else
                {
                    // Otherwise, go right until the field's value does not contain a hit.
                    i = x;

                    while (i < (this.EnemyMap.Width - 1) && this.EnemyMap.Field[i, y] == GameMap.FieldHit)
                    {
                        i++;
                    }

                    // This coordinate must contain a ship.
                    newP[0, 0] = i;
                }
            }
            else
            {
                // Start at the given position.
                newP[0, 0] = x;

                int i = y;

                // Go up until the field's value does not contain a hit.
                while (i > 0 && this.EnemyMap.Field[x, i] == GameMap.FieldHit)
                {
                    i--;
                }

                // If it's a valid coordinate, it could contain a part of a ship.
                if (this.IsValidCoordinate(x, i))
                {
                    newP[0, 1] = i;
                }
                else
                {
                    // Otherwise, go down until the field's value does not contain a hit.
                    i = y;

                    while (i < (this.EnemyMap.Height - 1) && this.EnemyMap.Field[x, i] == GameMap.FieldHit)
                    {
                        i++;
                    }

                    // This coordinate must contain a ship.
                    newP[0, 1] = i;
                }
            }

            return newP;
        }

        /// <summary>
        /// Checks, if the given position could contain a ship.
        /// </summary>
        /// <param name="x">X - coordinate of the given position.</param>
        /// <param name="y">Y - coordinate of the given position.</param>
        /// <returns> A boolean, which indicates whether the given position is valid or not.</returns>
        private bool IsValidCoordinate(int x, int y)
        {
            Ship[] ships = this.RecessedShips.Ships;

            if (this.EnemyMap.Field[x, y] == GameMap.FieldEmpty)
            {
                for (int i = 0; i < this.RecessedShips.SavedShips; i++)
                {
                    if (ships[i].IsCollision(x, y))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
