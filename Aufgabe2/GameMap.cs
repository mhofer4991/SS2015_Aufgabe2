//-----------------------------------------------------------------------
// <copyright file="GameMap.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents the game map.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class represents the game map.
    /// </summary>
    public class GameMap : IDrawable
    {
        /// <summary> Gets the constant value for a empty field. </summary>
        public const int FieldEmpty = 0;

        /// <summary> Gets the constant value for a hit field. </summary>
        public const int FieldHit = 1;

        /// <summary> Gets the constant value for a missed field. </summary>
        public const int FieldMissed = 2;

        /// <summary> Gets the constant value for a destroyed field. </summary>
        public const int FieldDestroyed = 3;

        /// <summary> Gets the minimal width of the game map. </summary>
        public const int MinimalWidth = 10;

        /// <summary> Gets the minimal height of the game map. </summary>
        public const int MinimalHeight = 10;

        /// <summary> Gets the maximal width of the game map. </summary>
        public const int MaximalWidth = 26;

        /// <summary> Gets the maximal height of the game map. </summary>
        public const int MaximalHeight = 18;

        /// <summary> Gets the minimal amount of battle cruisers. </summary>
        public const int MinimalAmountBattleCruisers = 1;

        /// <summary> Gets the minimal amount of cruisers. </summary>
        public const int MinimalAmountCruisers = 2;

        /// <summary> Gets the minimal amount of destroyers. </summary>
        public const int MinimalAmountDestroyers = 3;

        /// <summary> Gets the minimal amount of submarines. </summary>
        public const int MinimalAmountSubmarines = 5;

        /// <summary> Gets the maximal amount of battle cruisers. </summary>
        public const int MaximalAmountBattleCruisers = 4;

        /// <summary> Gets the maximal amount of cruisers. </summary>
        public const int MaximalAmountCruisers = 8;

        /// <summary> Gets the maximal amount of destroyers. </summary>
        public const int MaximalAmountDestroyers = 10;

        /// <summary> Gets the maximal amount of submarines. </summary>
        public const int MaximalAmountSubmarines = 12;

        /// <summary> Decides, if all ships are visible. </summary>
        private bool hidingShips;

        /// <summary> Color of the battle cruiser. </summary>
        private ConsoleColor battleCruiserColor;

        /// <summary> Color of the cruiser. </summary>
        private ConsoleColor cruiserColor;

        /// <summary> Color of the destroyer. </summary>
        private ConsoleColor destroyerColor;

        /// <summary> Color of the submarine. </summary>
        private ConsoleColor submarineColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameMap"/> class.
        /// </summary>
        public GameMap() : this(MinimalWidth, MinimalHeight)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameMap"/> class.
        /// </summary>
        /// <param name="width">The width of the game map.</param>
        /// <param name="height">The height of the game map.</param>
        public GameMap(int width, int height)
        {
            if (!this.SetNewSize(width, height))
            {
                this.SetNewSize(MinimalWidth, MinimalHeight);
            }

            this.X = 0;
            this.Y = 0;
            this.CursorX = 0;
            this.CursorY = 0;

            this.Rand = new Random();
            this.hidingShips = false;

            this.SetShipsAmounts(MinimalAmountBattleCruisers, MinimalAmountCruisers, MinimalAmountDestroyers, MinimalAmountSubmarines);

            this.SetShipsLenghts(Ship.BattleCruiser.MinimalLength, Ship.Cruiser.MinimalLength, Ship.Destroyer.MinimalLength, Ship.Submarine.MinimalLength);

            this.ShipCollection = new ShipCollection(this.AmountBattleCruisers, this.AmountCruisers, this.AmountDestroyers, this.AmountSubmarines);

            this.SetShipsColors(Ship.BattleCruiser.DefaultColor, Ship.Cruiser.DefaultColor, Ship.Destroyer.DefaultColor, Ship.Submarine.DefaultColor);
        }

        /// <summary>
        /// Delegate for event OnGameMapHit.
        /// </summary>
        /// <param name="x">X - coordinate of the hit square.</param>
        /// <param name="y">Y - coordinate of the hit square.</param>
        public delegate void GameMapHit(int x, int y);

        /// <summary>
        /// Delegate for event OnGameMapMissed.
        /// </summary>
        /// <param name="x">X - coordinate of the missed square.</param>
        /// <param name="y">Y - coordinate of the missed square.</param>
        public delegate void GameMapMissed(int x, int y);

        /// <summary>
        /// Delegate for event OnGameMapShipDestroyed.
        /// </summary>
        /// <param name="s">Ship, which has been destroyed.</param>
        public delegate void GameMapShipDestroyed(Ship s);

        /// <summary> Delegate for event OnGameMapLost. </summary>
        public delegate void GameMapLost();

        /// <summary> Is called when some coordinates were a hit. </summary>
        public event GameMapHit OnGameMapHit;

        /// <summary> Is called when some coordinates were a failure. </summary>
        public event GameMapMissed OnGameMapMissed;

        /// <summary> Is called when some ship has been destroyed. </summary>
        public event GameMapShipDestroyed OnGameMapShipDestroyed;

        /// <summary> Is called, when all ships has been destroyed. </summary>
        public event GameMapLost OnGameMapLost;

        /// <summary>
        /// Gets the width of the game map.
        /// </summary>
        /// <value> The width of the game map.</value>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height of the game map.
        /// </summary>
        /// <value> The height of the game map.</value>
        public int Height { get; private set; }

        /// <summary>
        /// Gets the field of game map as an integer array.
        /// </summary>
        /// <value> The field represented as an integer array. </value>
        public int[,] Field { get; private set; }

        /// <summary>
        /// Gets the X - position of the game map.
        /// </summary>
        /// <value> The X - position of the game map. </value>
        public int X { get; private set; }

        /// <summary>
        /// Gets the Y - position of the game map.
        /// </summary>
        /// <value> The Y - position of the game map. </value>
        public int Y { get; private set; }

        /// <summary>
        /// Gets the X - position of the cursor within the game map.
        /// </summary>
        /// <value> The X - position of the cursor in the game map. </value>
        public int CursorX { get; private set; }

        /// <summary>
        /// Gets the Y - position of the cursor within the game map.
        /// </summary>
        /// <value> The Y - position of the cursor in the game map. </value>
        public int CursorY { get; private set; }

        /// <summary>
        /// Gets the collection of ships of the game map.
        /// </summary>
        /// <value> The collection of ships of the game map. </value>
        public ShipCollection ShipCollection { get; private set; }

        /// <summary>
        /// Gets the current amount of battle cruisers.
        /// </summary>
        /// <value> The current amount of battle cruisers. </value>
        public int AmountBattleCruisers { get; private set; }

        /// <summary>
        /// Gets the current amount of cruisers.
        /// </summary>
        /// <value> The current amount of cruisers. </value>
        public int AmountCruisers { get; private set; }

        /// <summary>
        /// Gets the current amount of destroyers.
        /// </summary>
        /// <value> The current amount of destroyers. </value>
        public int AmountDestroyers { get; private set; }

        /// <summary>
        /// Gets the current amount of submarines.
        /// </summary>
        /// <value> The current amount of submarines. </value>
        public int AmountSubmarines { get; private set; }

        /// <summary>
        /// Gets the current width of the battle cruiser.
        /// </summary>
        /// <value> The current width of the battle cruiser. </value>
        public int BattleCruiserLength { get; private set; }

        /// <summary>
        /// Gets the current width of the cruiser.
        /// </summary>
        /// <value> The current width of the cruiser. </value>
        public int CruiserLength { get; private set; }

        /// <summary>
        /// Gets the current width of the destroyer.
        /// </summary>
        /// <value> The current width of the destroyer. </value>
        public int DestroyerLength { get; private set; }

        /// <summary>
        /// Gets the current width of the submarine.
        /// </summary>
        /// <value> The current width of the submarine. </value>
        public int SubmarineLength { get; private set; }

        /// <summary>
        /// Gets the Random instance of the game map.
        /// </summary>
        /// <value> The Random instance of the game map.</value>
        public Random Rand { get; private set; }

        /// <summary>
        /// Converts some numeric X and Y - coordinates into a valid field label for the game map.
        /// </summary>
        /// <param name="x">Some X - coordinate.</param>
        /// <param name="y">Some Y - coordinate.</param>
        /// <returns>A valid field label for the game map.</returns>
        public static string GetFieldLabel(int x, int y)
        {
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

            return alpha[x] + " : " + (y + 1).ToString();
        }

        /// <summary>
        /// Sets a new amount for the different forms of ships.
        /// </summary>
        /// <param name="battleCruiserAmount">New amount of battle cruisers.</param>
        /// <param name="cruiserAmount">New amount of cruisers.</param>
        /// <param name="destroyerAmount">New amount of destroyers.</param>
        /// <param name="submarineAmount">New amount of submarines.</param>
        public void SetShipsAmounts(int battleCruiserAmount, int cruiserAmount, int destroyerAmount, int submarineAmount)
        {
            this.AmountBattleCruisers = battleCruiserAmount;
            this.AmountCruisers = cruiserAmount;
            this.AmountDestroyers = destroyerAmount;
            this.AmountSubmarines = submarineAmount;
        }

        /// <summary>
        /// Sets a new width for the different forms of ships.
        /// </summary>
        /// <param name="battleCruiserLength">New width of battle cruisers.</param>
        /// <param name="cruiserLength">New width of cruisers.</param>
        /// <param name="destroyerLength">New width of destroyers.</param>
        /// <param name="submarineLength">New width of submarines.</param>
        public void SetShipsLenghts(int battleCruiserLength, int cruiserLength, int destroyerLength, int submarineLength)
        {
            this.BattleCruiserLength = battleCruiserLength;
            this.CruiserLength = cruiserLength;
            this.DestroyerLength = destroyerLength;
            this.SubmarineLength = submarineLength;
        }

        /// <summary>
        /// Sets a new color for the different forms of ships.
        /// </summary>
        /// <param name="battleCruiserColor">New color of battle cruisers.</param>
        /// <param name="cruiserColor">New color of cruisers.</param>
        /// <param name="destroyerColor">New color of destroyers.</param>
        /// <param name="submarineColor">New color of submarines.</param>
        public void SetShipsColors(ConsoleColor battleCruiserColor, ConsoleColor cruiserColor, ConsoleColor destroyerColor, ConsoleColor submarineColor)
        {
            this.battleCruiserColor = battleCruiserColor;
            this.cruiserColor = cruiserColor;
            this.destroyerColor = destroyerColor;
            this.submarineColor = submarineColor;
            
            for (int i = 0; i < this.ShipCollection.SavedShips; i++)
            {
                if (this.ShipCollection.Ships[i] is Ship.BattleCruiser)
                {
                    this.ShipCollection.Ships[i].SetColor(this.battleCruiserColor);
                }
                else if (this.ShipCollection.Ships[i] is Ship.Cruiser)
                {
                    this.ShipCollection.Ships[i].SetColor(this.cruiserColor);
                }
                else if (this.ShipCollection.Ships[i] is Ship.Destroyer)
                {
                    this.ShipCollection.Ships[i].SetColor(this.destroyerColor);
                }
                else if (this.ShipCollection.Ships[i] is Ship.Submarine)
                {
                    this.ShipCollection.Ships[i].SetColor(this.submarineColor);
                }
            }
        }

        /// <summary>
        /// Gets a new battle cruiser, which is associated to this game map.
        /// </summary>
        /// <returns>A new battle cruiser associated to this game map.</returns>
        public Ship.BattleCruiser GetNewBattleCruiser()
        {
            return new Ship.BattleCruiser(0, 0, this.BattleCruiserLength, this.battleCruiserColor, this);
        }

        /// <summary>
        /// Gets a new cruiser, which is associated to this game map.
        /// </summary>
        /// <returns>A new cruiser associated to this game map.</returns>
        public Ship.Cruiser GetNewCruiser()
        {
            return new Ship.Cruiser(0, 0, this.CruiserLength, this.cruiserColor, this);
        }

        /// <summary>
        /// Gets a new destroyer, which is associated to this game map.
        /// </summary>
        /// <returns>A new destroyer associated to this game map.</returns>
        public Ship.Destroyer GetNewDestroyer()
        {
            return new Ship.Destroyer(0, 0, this.DestroyerLength, this.destroyerColor, this);
        }

        /// <summary>
        /// Gets a new submarine, which is associated to this game map.
        /// </summary>
        /// <returns>A new submarine associated to this game map.</returns>
        public Ship.Submarine GetNewSubmarine()
        {
            return new Ship.Submarine(0, 0, this.SubmarineLength, this.submarineColor, this);
        }

        /// <summary>
        /// Sets the cursor position within the game map.
        /// </summary>
        /// <param name="x">X - coordinate of the cursor.</param>
        /// <param name="y">Y - coordinate of the cursor.</param>
        public void SetCursorPosition(int x, int y)
        {
            this.CursorX = x;
            this.CursorY = y;
        }

        /// <summary>
        /// Resets the ship collection of the game map.
        /// </summary>
        public void ResetShipCollection()
        {
            this.ShipCollection.Reset(this.AmountBattleCruisers, this.AmountCruisers, this.AmountDestroyers, this.AmountSubmarines);
        }

        /// <summary>
        /// Resets the ship collection and adds new ships.
        /// </summary>
        /// <exception cref="NoPlaceForShipException">
        /// Is raised if...
        /// ...there is no place left for another ship.
        /// </exception>
        public void GenerateNewGameMap()
        {
            this.ResetShipCollection();

            for (int i = 0; i < this.AmountBattleCruisers; i++)
            {
                this.ShipCollection.AddBattleCruiser(this.GetNewBattleCruiser());
            }

            for (int i = 0; i < this.AmountCruisers; i++)
            {
                this.ShipCollection.AddCruiser(this.GetNewCruiser());
            }

            for (int i = 0; i < this.AmountDestroyers; i++)
            {
                this.ShipCollection.AddDestroyer(this.GetNewDestroyer());
            }

            for (int i = 0; i < this.AmountSubmarines; i++)
            {
                this.ShipCollection.AddSubmarine(this.GetNewSubmarine());
            }
        }

        /// <summary>
        /// Gets the position of the first field A:1.
        /// </summary>
        /// <returns>An integer array, which contains the coordinates.</returns>
        public int[] GetDrawingStartPoint()
        {
            return new int[] { this.X + 4, this.Y + 2 };
        }

        /// <summary>
        /// Sets the size of the game map.
        /// </summary>
        /// <param name="width">New width of the game map.</param>
        /// <param name="height">New height of the game map.</param>
        /// <returns>A boolean, which indicates whether the new size was set or not.</returns>
        public bool SetNewSize(int width, int height)
        {
            if (width >= MinimalWidth && width <= MaximalWidth)
            {
                if (height >= MinimalHeight && height <= MaximalHeight)
                {
                    this.Width = width;
                    this.Height = height;

                    this.Field = new int[width, height];

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Sets the position of the game map.
        /// </summary>
        /// <param name="x">New X - coordinate of the game map.</param>
        /// <param name="y">New Y - coordinate of the game map.</param>
        /// <returns>A boolean, which indicates whether the new position was set or not.</returns>
        public bool SetNewPosition(int x, int y)
        {
            if (x >= 0)
            {
                if (y >= 0)
                {
                    this.X = x;
                    this.Y = y;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Finds a random position for every ship in this game map.
        /// </summary>
        /// <exception cref="NoPlaceForShipException">
        /// Is raised if...
        /// ...there is no place left for another ship.
        /// </exception>
        public void RandomizeShips()
        {
            try
            {
                Ship[] ships = this.ShipCollection.Ships;

                this.ShipCollection.Reset();

                int index = this.Rand.Next(0, ships.Length);

                while (this.ShipCollection.SavedShips != this.ShipCollection.AmountShips)
                {
                    this.ShipCollection.AddShip(ships[index]);

                    ships[index].FindRandomPosition();

                    ships = ships.Except(new Ship[] { ships[index] }).ToArray();

                    index = this.Rand.Next(0, ships.Length);
                }
            }
            catch (NoPlaceForShipException)
            {
                // It may be possible to arrange all ships in an organized order, but with randomized positions it can happen that there is no place left for some ships.
                this.GenerateNewGameMap();
            }
        }

        /// <summary>
        /// Checks, if the given coordinates hit any ship and changes the field state at the given position.
        /// </summary>
        /// <param name="x">X - coordinate of the given position.</param>
        /// <param name="y">Y - coordinate of the given position.</param>
        /// <returns>A boolean, which indicates whether the ship was hit or not.</returns>
        public bool HitAtPoint(int x, int y)
        {
            Ship[] ships = ShipCollection.Ships;
            bool wasHit = false;

            for (int i = 0; i < ships.Length; i++)
            {
                // If a ship was hit, the state of this field will be changed to 1. 
                if (ships[i].HitAtPoint(x, y))
                {
                    wasHit = true;

                    // If the ship is already hit at this point, it doesn't count anymore and the player missed the ship.
                    if (this.Field[x, y] != GameMap.FieldEmpty)
                    {
                        if (this.OnGameMapMissed != null)
                        {
                            this.OnGameMapMissed(x, y);
                        }

                        return false;
                    }

                    this.Field[x, y] = GameMap.FieldHit;

                    if (this.OnGameMapHit != null)
                    {
                        this.OnGameMapHit(x, y);
                    }

                    // If the ship is also destroyed, the state of all fields containing this ship will be changed to 3.
                    if (ships[i].IsDestroyed())
                    {
                        for (int j = 0; j < ships[i].Height; j++)
                        {
                            for (int k = 0; k < ships[i].Width; k++)
                            {
                                this.Field[ships[i].X + k, ships[i].Y + j] = GameMap.FieldDestroyed;
                            }
                        }

                        if (this.OnGameMapShipDestroyed != null)
                        {
                            this.OnGameMapShipDestroyed(ships[i]);
                        }

                        // If all ships of this map are destroyed, the event OnGameMapLost will be called.
                        if (this.AllShipsDestroyed() && this.OnGameMapLost != null)
                        {
                            this.OnGameMapLost();
                        }
                    }

                    return true;
                }
            }

            // If no ship was hit, the state of this field will be changed to 2. 
            if (!wasHit)
            {
                this.Field[x, y] = GameMap.FieldMissed;

                if (this.OnGameMapMissed != null)
                {
                    this.OnGameMapMissed(x, y);
                }
            }

            return false;
        }

        /// <summary>
        /// If a ship was found at the given position, it will be returned.
        /// </summary>
        /// <param name="x">X - coordinate of the given position.</param>
        /// <param name="y">Y - coordinate of the given position.</param>
        /// <returns>An instance of a ship or null, if no ship was found.</returns>
        public Ship GetShip(int x, int y)
        {
            Ship[] ships = ShipCollection.Ships;

            for (int i = 0; i < ships.Length; i++)
            {
                if (ships[i].GetHitPoint(x, y)[0] != -1)
                {
                    return ships[i];
                }
            }

            return null;
        }

        /// <summary>
        /// If set to true, only hit and and sunken ships will be showed.
        /// </summary>
        /// <param name="b">A boolean, which will be assigned to hidingShips.</param>
        public void SetShipsHiding(bool b)
        {
            this.hidingShips = b;
        }

        /// <summary>
        /// Returns the cursor position after an user input.
        /// Approved ConsoleKeyInfo's are LeftArrow, UpArrow, RightArrow and DownArrow.
        /// </summary>
        /// <param name="cki">ConsoleKeyInfo, which will be processed.</param>
        /// <returns>An integer array, which contains the cursor position.</returns>
        public int[] GetPositionFromCursorInput(ConsoleKeyInfo cki)
        {
            int sX = this.GetDrawingStartPoint()[0];
            int sY = this.GetDrawingStartPoint()[1];

            int[] pos = new int[] { sX + this.CursorX, sY + this.CursorY };

            bool validInput = true;

            switch (cki.Key)
            {
                case ConsoleKey.LeftArrow:
                    if (pos[0] - 1 >= sX)
                    {
                        pos[0]--;
                    }

                    break;
                case ConsoleKey.UpArrow:
                    if (pos[1] - 1 >= sY)
                    {
                        pos[1]--;
                    }

                    break;
                case ConsoleKey.RightArrow:
                    if (pos[0] + 1 <= (sX + this.Width - 1))
                    {
                        pos[0]++;
                    }

                    break;
                case ConsoleKey.DownArrow:
                    if (pos[1] + 1 <= (sY + this.Height - 1))
                    {
                        pos[1]++;
                    }

                    break;
                default:
                    validInput = false;
                    break;
            }

            if (validInput)
            {
                this.Draw();
            }

            pos[0] -= sX;
            pos[1] -= sY;
            this.CursorX = pos[0];
            this.CursorY = pos[1];

            return pos;
        }

        /// <summary>
        /// Used for displaying the ship on the console.
        /// </summary>
        public void Draw()
        {
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            string temp = string.Empty;

            Console.SetCursorPosition(this.X, this.Y);

            for (int i = 0; i < this.Width; i++)
            {
                temp += alpha[i];
            }

            Console.Write("    " + temp);

            Console.SetCursorPosition(this.X, this.Y + 1);
            temp = string.Empty;

            for (int i = 0; i < this.Width; i++)
            {
                temp += "-";
            }

            Console.Write("   +" + temp + "+");
            Console.SetCursorPosition(this.X, this.Y + this.Height + 2);
            Console.Write("   +" + temp + "+");

            for (int i = 0; i < this.Height; i++)
            {
                Console.SetCursorPosition(this.X, this.Y + 2 + i);
                temp = string.Empty;

                Console.Write(" {0, 2}|", i + 1);

                for (int j = 0; j < this.Width; j++)
                {
                    if (this.Field[j, i] == GameMap.FieldEmpty || (this.Field[j, i] == GameMap.FieldDestroyed && !this.hidingShips))
                    {
                        Console.Write(" "); // empty
                    }
                    else if (this.Field[j, i] == GameMap.FieldHit)
                    {
                        if (this.hidingShips)
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.Write("X");
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        else
                        {
                            Console.Write(" ");
                        }
                    }
                    else if (this.Field[j, i] == GameMap.FieldMissed)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("-"); // missed
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else if (this.Field[j, i] == GameMap.FieldDestroyed)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write("S"); // Sunken
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
                
                Console.Write("|");
            }

            if (!this.hidingShips)
            {
                Ship[] ships = this.ShipCollection.Ships;

                for (int i = 0; i < this.ShipCollection.SavedShips; i++)
                {
                    ships[i].Draw();
                }
            }
        }

        /// <summary>
        /// Checks, if all ships are destroyed.
        /// </summary>
        /// <returns>A boolean, which indicates whether all ships are destroyed or not.</returns>
        public bool AllShipsDestroyed()
        {
            Ship[] ships = this.ShipCollection.Ships;

            for (int i = 0; i < ships.Length; i++)
            {
                if (!ships[i].IsDestroyed())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
