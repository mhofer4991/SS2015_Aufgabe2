//-----------------------------------------------------------------------
// <copyright file="Ship.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents different forms of ships and provides methods to find random positions within a game map and check collisions.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class represents different forms of ships and provides methods to find random positions within a game map and check collisions.
    /// </summary>
    public abstract class Ship : IDrawable
    {
        /// <summary> Needed for generating random positions. </summary>
        private Random rand;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ship"/> class.
        /// </summary>
        /// <param name="x">X - position of the ship.</param>
        /// <param name="y">Y - position of the ship.</param>
        /// <param name="width">Width of the ship.</param>
        /// <param name="height">Height of the ship.</param>
        /// <param name="color">Color of the ship.</param>
        /// <param name="gm">Game map, which is associated with this ship.</param>
        /// <exception cref="NoPlaceForShipException">
        /// Is raised if...
        /// ...there is no place left for the ship.
        /// </exception>
        public Ship(int x, int y, int width, int height, ConsoleColor color, GameMap gm)
        {
            this.GameMap = gm;

            this.Width = width;
            this.Height = height;

            this.ShipMask = new int[width, height];

            this.rand = this.GameMap.Rand;

            this.Orientation = Ship.ShipOrientation.Horizontal;

            this.SetColor(color);

            if (!this.SetPosition(x, y))
            {
                int[] newPos = this.FindNewPosition();

                if (!this.SetPosition(newPos[0], newPos[1]))
                {
                    this.Rotate(newPos[0], newPos[1]);
                }
            }
        }

        /// <summary>
        /// Represents the orientation of a ship.
        /// </summary>
        public enum ShipOrientation
        {
            /// <summary>
            /// Ship is arranged horizontally.
            /// </summary>
            Horizontal,

            /// <summary>
            /// Ship is arranged vertically.
            /// </summary>
            Vertical 
        }

        /// <summary>
        /// Gets the orientation of the ship.
        /// </summary>
        /// <value>The orientation of the ship.</value>
        public ShipOrientation Orientation { get; private set; }

        /// <summary>
        /// Gets the X - position of the ship.
        /// </summary>
        /// <value>The X - position of the ship.</value>
        public int X { get; private set; }

        /// <summary>
        /// Gets the Y - Position of the ship.
        /// </summary>
        /// <value>The Y - Position of the ship.</value>
        public int Y { get; private set; }

        /// <summary>
        /// Gets the width of the ship.
        /// </summary>
        /// <value>The width of the ship.</value>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height of the ship.
        /// </summary>
        /// <value>The height of the ship.</value>
        public int Height { get; private set; }

        /// <summary>
        /// Gets the associated game map.
        /// </summary>
        /// <value>An instance of a game map.</value>
        public GameMap GameMap { get; private set; }

        /// <summary>
        /// Gets the color of the ship.
        /// </summary>
        /// <value>The color of the ship.</value>
        public ConsoleColor ShipColor { get; private set; }

        /// <summary>
        /// Gets the state of all parts of the ship. 0 means intact, 1 hit.
        /// </summary>
        /// <value>An array, which represents the state of the ship.</value>
        public int[,] ShipMask { get; private set; }

        /// <summary>
        /// Sets the color of the ship.
        /// </summary>
        /// <param name="color">The new color of the ship.</param>
        public void SetColor(ConsoleColor color)
        {
            this.ShipColor = color;
        }

        /// <summary>
        /// Sets the new position, if there is no collision with the map and any other ship on this map.
        /// </summary>
        /// <param name="x">X - coordinate of the new position.</param>
        /// <param name="y">Y - coordinate of the new position.</param>
        /// <returns>A boolean, which indicates whether the new position was set or not.</returns>
        public bool SetPosition(int x, int y)
        {
            if (!this.CollisionWithMap(x, y) && !this.IsInCollisionWithOtherShips(x, y))
            {
                this.X = x;
                this.Y = y;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Moves to the new position, if there is no collision with the map. Collisions with other ships can occur.
        /// </summary>
        /// <param name="x">X - coordinate of the new position.</param>
        /// <param name="y">Y - coordinate of the new position.</param>
        /// <returns>A boolean, which indicates whether the new position was set or not.</returns>
        public bool Move(int x, int y)
        {
            if (!this.CollisionWithMap(x, y))
            {
                this.X = x;
                this.Y = y;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks, if the ship with the new position collides with the map.
        /// </summary>
        /// <param name="x">X - coordinate of the new position.</param>
        /// <param name="y">Y - coordinate of the new position.</param>
        /// <returns>A boolean, which indicates whether the ship would collide or not.</returns>
        public bool CollisionWithMap(int x, int y)
        {
            if (x >= 0 && (x + this.Width) <= this.GameMap.Width)
            {
                if (y >= 0 && (y + this.Height) <= this.GameMap.Height)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Finds a new position on the map for the ship.
        /// </summary>
        /// <exception cref="NoPlaceForShipException">
        /// Is raised if...
        /// ...there is no place left for the ship.
        /// </exception>
        /// <returns>An integer array, which contains the coordinates of the new position.</returns>
        public int[] FindNewPosition()
        {
            int[,] emptyPositions = this.FindEmptyPositions();

            if (emptyPositions.GetUpperBound(0) + 1 != 0)
            {
                return new int[] { emptyPositions[0, 0], emptyPositions[0, 1] };
            }
            else
            {
                throw new NoPlaceForShipException("There is no place for another ship!");
            }
        }

        /// <summary>
        /// Sets a random position on the map for the ship.
        /// </summary>
        /// <exception cref="NoPlaceForShipException">
        /// Is raised if...
        /// ...there is no place left for the ship.
        /// </exception>
        public void FindRandomPosition()
        {
            int[,] emptyPositions = this.FindEmptyPositions();

            if (emptyPositions.GetUpperBound(0) + 1 != 0)
            {
                int index = this.rand.Next(0, emptyPositions.GetUpperBound(0) + 1);

                if (this.rand.Next(0, 2) == 1 && this.CanRotate(emptyPositions[index, 0], emptyPositions[index, 1]))
                {
                     this.Rotate(emptyPositions[index, 0], emptyPositions[index, 1]);
                }
                else
                {
                    if (!this.SetPosition(emptyPositions[index, 0], emptyPositions[index, 1]))
                    {
                        this.Rotate(emptyPositions[index, 0], emptyPositions[index, 1]);
                    }
                }
            }
            else
            {
                throw new NoPlaceForShipException("There is no place for another ship!");
            }
        }

        /// <summary>
        /// Scans the map and looks for all coordinates where the ship could be placed.
        /// </summary>
        /// <returns>An two-dimensional array, which contains all possible positions.</returns>
        public int[,] FindEmptyPositions()
        {
            int[,] temp = new int[this.GameMap.Width * this.GameMap.Height, 2];
            int index = 0;

            for (int i = 0; i < this.GameMap.Height; i++)
            {
                for (int j = 0; j < this.GameMap.Width; j++)
                {
                    if ((!this.CollisionWithMap(j, i) && !this.IsInCollisionWithOtherShips(j, i)) || this.CanRotate(j, i))
                    {
                        temp[index, 0] = j;
                        temp[index, 1] = i;
                        index++;
                    }
                }
            }

            int[,] emptyPositions = new int[index, 2];
            Array.Copy(temp, emptyPositions, emptyPositions.Length);

            return emptyPositions;
        }

        /// <summary>
        /// Rotates the ship.
        /// </summary>
        /// <returns>A boolean, which indicates whether the rotation was successful or not.</returns>
        public bool Rotate()
        {
            return this.Rotate(this.X, this.Y);
        }

        /// <summary>
        /// Rotates the ship and changes it coordinates.
        /// </summary>
        /// <param name="x">X - coordinate of the new position.</param>
        /// <param name="y">Y - coordinate of the new position.</param>
        /// <returns>A boolean, which indicates whether the rotation was successful or not.</returns>
        public bool Rotate(int x, int y)
        {
            if (this.CanRotate(x, y))
            {
                if (this.Orientation == ShipOrientation.Horizontal)
                {
                    this.Orientation = ShipOrientation.Vertical;
                }
                else
                {
                    this.Orientation = ShipOrientation.Horizontal;
                }

                int temp = this.Width;

                this.Width = this.Height;
                this.Height = temp;

                this.SetPosition(x, y);

                int[,] newArr = new int[this.Width, this.Height];

                for (int i = 0; i < this.Width; i++)
                {
                    for (int j = 0; j < this.Height; j++)
                    {
                        newArr[i, j] = this.ShipMask[j, i];
                    }
                }

                this.ShipMask = newArr;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks, if the ship could be rotated on a designated position.
        /// </summary>
        /// <param name="x">X - coordinate of the designated position.</param>
        /// <param name="y">Y - coordinate of the designated position.</param>
        /// <returns>A boolean, which indicates whether the ship could be rotated or not.</returns>
        public bool CanRotate(int x, int y)
        {
            int tempWidth = this.Width;
            int tempHeight = this.Height;

            bool validRotation = false;

            this.Width = tempHeight;
            this.Height = tempWidth;

            if (!this.CollisionWithMap(x, y) && !this.IsInCollisionWithOtherShips(x, y))
            {
                validRotation = true;
            }

            this.Width = tempWidth;
            this.Height = tempHeight;

            return validRotation;
        }

        /// <summary>
        /// Checks, if the ship would collide with other ships on a designated position.
        /// </summary>
        /// <param name="x">X - coordinate of the designated position.</param>
        /// <param name="y">Y - coordinate of the designated position.</param>
        /// <returns>A boolean, which indicates whether the ship would collide or not.</returns>
        public bool IsInCollisionWithOtherShips(int x, int y)
        {
            Ship[] ships = GameMap.ShipCollection.Ships; // .GetAllShips();

            for (int i = 0; i < GameMap.ShipCollection.SavedShips; i++)
            {
                if (ships[i] != null && ships[i] != this)
                {
                    if (this.IsShipCollision(x, y, ships[i]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks, if the ship would collide with the other ship on a designated position.
        /// </summary>
        /// <param name="x">X - coordinate of the designated position.</param>
        /// <param name="y">Y - coordinate of the designated position.</param>
        /// <param name="s2">Second ship, whose collision with this ship will be checked.</param>
        /// <returns>A boolean, which indicates whether the ship would collide or not.</returns>
        public bool IsShipCollision(int x, int y, Ship s2)
        {
            for (int i = 0; i < this.Height; i++)
            {
                for (int j = 0; j < this.Width; j++)
                {
                    if (s2.IsCollision(x + j, y + i))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks, if the given position would collide with the ship. 
        /// </summary>
        /// <param name="x">X - coordinate of the given position.</param>
        /// <param name="y">Y - coordinate of the given position.</param>
        /// <returns>A boolean, which indicates whether the given position would collide with the ship or not.</returns>
        public bool IsCollision(int x, int y)
        {
            for (int i = -1; i <= this.Height; i++)
            {
                for (int j = -1; j <= this.Width; j++)
                {
                    // ships can border diagonally
                    if (!(i == -1 && j == -1) && !(i == -1 && j == this.Width) && !(i == this.Height && j == -1) && !(i == this.Height && j == this.Width))
                    {
                        if (this.X + j == x && this.Y + i == y)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the position within the ship, where the given coordinates would hit it.
        /// </summary>
        /// <param name="x">X - coordinate of the given position.</param>
        /// <param name="y">Y - coordinate of the given position.</param>
        /// <returns>An integer array, which contains the position. It contains -1 if there was no hit.</returns>
        public int[] GetHitPoint(int x, int y)
        {
            for (int i = 0; i < this.Height; i++)
            {
                for (int j = 0; j < this.Width; j++)
                {
                    if (y == this.Y + i && x == this.X + j)
                    {
                        return new int[] { j, i };
                    }
                }
            }

            return new int[] { -1, -1 };
        }

        /// <summary>
        /// Changes the state to 1 = hit of the part of the ship, which was hit by the given position.
        /// </summary>
        /// <param name="x">X - coordinate of the given position.</param>
        /// <param name="y">Y - coordinate of the given position.</param>
        /// <returns>A boolean, which indicates whether the ship was hit or not.</returns>
        public bool HitAtPoint(int x, int y)
        {
            int[] p = this.GetHitPoint(x, y);

            if (p[0] != -1)
            {
                this.ShipMask[p[0], p[1]] = 1;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks, if all parts of the ship are hit and therefore the ship is destroyed.
        /// </summary>
        /// <returns>A boolean, which indicates whether the ship is destroyed or not.</returns>
        public bool IsDestroyed()
        {
            for (int i = 0; i < this.Width; i++)
            {
                for (int j = 0; j < this.Height; j++)
                {
                    if (this.ShipMask[i, j] == 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Used for displaying the ship on the console.
        /// </summary>
        public void Draw()
        {
            int[] startPoint = this.GameMap.GetDrawingStartPoint();
            string temp = string.Empty;
            ConsoleColor tempC = Console.ForegroundColor;

            Console.ForegroundColor = this.ShipColor;
            
            for (int i = 0; i < this.ShipMask.GetUpperBound(1) + 1; i++)
            {
                Console.SetCursorPosition(startPoint[0] + this.X, startPoint[1] + this.Y + i);
                temp = string.Empty;

                for (int j = 0; j < this.ShipMask.GetUpperBound(0) + 1; j++)
                {
                    if (this.ShipMask[j, i] == 0)
                    {
                        temp += "#";
                    }
                    else if (this.ShipMask[j, i] == 1)
                    {
                        if (this.IsDestroyed())
                        {
                            temp += "S";
                        }
                        else
                        {
                            temp += "X";
                        }
                    }
                }

                Console.Write(temp);
            }            

            Console.ForegroundColor = tempC;
        }

        /// <summary>
        /// Represents the ship-type battle cruiser.
        /// </summary>
        public class BattleCruiser : Ship
        {
            /// <summary> Gets the minimal width of the battle cruiser. </summary>
            public const int MinimalLength = 5;

            /// <summary> Gets the maximal width of the battle cruiser. </summary>
            public const int MaximalLength = 10;

            /// <summary> Gets the default color of the battle cruiser. </summary>
            public const ConsoleColor DefaultColor = ConsoleColor.White;
            
            /// <summary>
            /// Initializes a new instance of the <see cref="BattleCruiser"/> class.
            /// </summary>
            /// <param name="x">X - coordinate of the battle cruiser.</param>
            /// <param name="y">Y - coordinate of the battle cruiser.</param>
            /// <param name="width">Width of the battle cruiser.</param>
            /// <param name="color">Color of the battle cruiser.</param>
            /// <param name="gm">Game map, which is associated with this battle cruiser.</param>
            public BattleCruiser(int x, int y, int width, ConsoleColor color, GameMap gm)
                : base(x, y, width, 1, color, gm)
            {
            }
        }

        /// <summary>
        /// Represents the ship-type cruiser.
        /// </summary>
        public class Cruiser : Ship
        {
            /// <summary> Gets the minimal width of the cruiser. </summary>
            public const int MinimalLength = 4;

            /// <summary> Gets the maximal width of the cruiser. </summary>
            public const int MaximalLength = 8;

            /// <summary> Gets the default color of the cruiser. </summary>
            public const ConsoleColor DefaultColor = ConsoleColor.White;
            
            /// <summary>
            /// Initializes a new instance of the <see cref="Cruiser"/> class.
            /// </summary>
            /// <param name="x">X - coordinate of the cruiser.</param>
            /// <param name="y">Y - coordinate of the cruiser.</param>
            /// <param name="width">Width of the cruiser.</param>
            /// <param name="color">Color of the cruiser.</param>
            /// <param name="gm">Game map, which is associated with this cruiser.</param>
            public Cruiser(int x, int y, int width, ConsoleColor color, GameMap gm)
                : base(x, y, width, 1, color, gm)
            {
            }
        }

        /// <summary>
        /// Represents the ship-type destroyer.
        /// </summary>
        public class Destroyer : Ship
        {
            /// <summary> Gets the minimal width of the destroyer. </summary>
            public const int MinimalLength = 3;

            /// <summary> Gets the maximal width of the destroyer. </summary>
            public const int MaximalLength = 6;

            /// <summary> Gets the default color of the destroyer. </summary>
            public const ConsoleColor DefaultColor = ConsoleColor.White;

            /// <summary>
            /// Initializes a new instance of the <see cref="Destroyer"/> class.
            /// </summary>
            /// <param name="x">X - coordinate of the destroyer.</param>
            /// <param name="y">Y - coordinate of the destroyer.</param>
            /// <param name="width">Width of the destroyer.</param>
            /// <param name="color">Color of the destroyer.</param>
            /// <param name="gm">Game map, which is associated with this destroyer.</param>
            public Destroyer(int x, int y, int width, ConsoleColor color, GameMap gm)
                : base(x, y, width, 1, color, gm)
            {
            }
        }

        /// <summary>
        /// Represents the ship-type submarine.
        /// </summary>
        public class Submarine : Ship
        {
            /// <summary> Gets the minimal width of the submarine. </summary>
            public const int MinimalLength = 2;

            /// <summary> Gets the maximal width of the submarine. </summary>
            public const int MaximalLength = 4;

            /// <summary> Gets the default color of the submarine. </summary>
            public const ConsoleColor DefaultColor = ConsoleColor.White;

            /// <summary>
            /// Initializes a new instance of the <see cref="Submarine"/> class.
            /// </summary>
            /// <param name="x">X - coordinate of the submarine.</param>
            /// <param name="y">Y - coordinate of the submarine.</param>
            /// <param name="width">Width of the submarine.</param>
            /// <param name="color">Color of the submarine.</param>
            /// <param name="gm">Game map, which is associated with this submarine.</param>
            public Submarine(int x, int y, int width, ConsoleColor color, GameMap gm)
                : base(x, y, width, 1, color, gm)
            {
            }
        }
    }
}
