//-----------------------------------------------------------------------
// <copyright file="Menu.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class provides methods to draw and control a menu and its submenus.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class provides methods to draw and control a menu and its submenus.
    /// </summary>
    public class Menu : IDrawable
    {
        /// <summary> Current state of the menu, which is used for navigation. </summary>
        private int state;

        /// <summary> Current size of the game map, which can be altered by the user. </summary>
        private int[] gameMapSize;

        /// <summary> Current size of a battle cruiser. </summary>
        private int battleCruiserLength;

        /// <summary> Current size of a cruiser. </summary>
        private int cruiserLength;

        /// <summary> Current size of a destroyer. </summary>
        private int destroyerLength;

        /// <summary> Current size of a submarine. </summary>
        private int submarineLength;

        /// <summary> Current colors of all ships. </summary>
        private ConsoleColor[] shipsColors;

        /// <summary> Current amount of battle cruisers. </summary>
        private int battleCruiserAmount;

        /// <summary> Current amount of cruisers. </summary>
        private int cruiserAmount;

        /// <summary> Current amount of destroyers. </summary>
        private int destroyerAmount;

        /// <summary> Current amount of submarines. </summary>
        private int submarineAmount;

        /// <summary>
        /// Initializes a new instance of the <see cref="Menu"/> class.
        /// </summary>
        public Menu()
        {
            this.state = 0;

            this.gameMapSize = new int[] { GameMap.MinimalWidth, GameMap.MinimalHeight };

            this.battleCruiserLength = Ship.BattleCruiser.MinimalLength;
            this.cruiserLength = Ship.Cruiser.MinimalLength;
            this.destroyerLength = Ship.Destroyer.MinimalLength;
            this.submarineLength = Ship.Submarine.MinimalLength;

            this.shipsColors = new ConsoleColor[] { Ship.BattleCruiser.DefaultColor, Ship.Cruiser.DefaultColor, Ship.Destroyer.DefaultColor, Ship.Submarine.DefaultColor };

            this.battleCruiserAmount = GameMap.MinimalAmountBattleCruisers;
            this.cruiserAmount = GameMap.MinimalAmountCruisers;
            this.destroyerAmount = GameMap.MinimalAmountDestroyers;
            this.submarineAmount = GameMap.MinimalAmountSubmarines;
        }

        /// <summary> Delegate for event OnMenuClosed. </summary>
        public delegate void MenuClosed();

        /// <summary> Delegate for event OnProgramExit. </summary>
        public delegate void ExitCalled();

        /// <summary>
        /// Delegate for event OnGameMapSizeChanged.
        /// </summary>
        /// <param name="width">New width of the game map.</param>
        /// <param name="height">New height of the game map.</param>
        public delegate void GameMapSizeChanged(int width, int height);

        /// <summary>
        /// Delegate for event OnShipsLengthChanged.
        /// </summary>
        /// <param name="battleCruiserLength">New length of a battle cruiser.</param>
        /// <param name="cruiserLength">New length of a cruiser.</param>
        /// <param name="destroyerLength">New length of a destroyer.</param>
        /// <param name="submarineLength">New length of a submarine.</param>
        public delegate void ShipsLengthChanged(int battleCruiserLength, int cruiserLength, int destroyerLength, int submarineLength);

        /// <summary>
        /// Delegate for event OnShipsColorsChanged.
        /// </summary>
        /// <param name="battleCruiserColor">New color of a battle cruiser.</param>
        /// <param name="cruiserColor">New color of a cruiser.</param>
        /// <param name="destroyerColor">New color of a destroyer.</param>
        /// <param name="submarineColor">New color of a submarine.</param>
        public delegate void ShipsColorsChanged(ConsoleColor battleCruiserColor, ConsoleColor cruiserColor, ConsoleColor destroyerColor, ConsoleColor submarineColor);

        /// <summary>
        /// Delegate for event OnShipsAmountChanged.
        /// </summary>
        /// <param name="battleCruiserAmount">New amount of battle cruisers.</param>
        /// <param name="cruiserAmount">New amount of cruisers.</param>
        /// <param name="destroyerAmount">New amount of destroyers.</param>
        /// <param name="submarineAmount">New amount of submarines.</param>
        public delegate void ShipsAmountChanged(int battleCruiserAmount, int cruiserAmount, int destroyerAmount, int submarineAmount);

        /// <summary> Delegate for event OnPlayerMapShowing. </summary>
        public delegate void PlayerMapShowing();

        /// <summary> Delegate for event OnEnemyMapShowing. </summary>
        public delegate void EnemyMapShowing();

        /// <summary> Is called when the menu has been closed. </summary>
        public event MenuClosed OnMenuClosed;

        /// <summary> Is called when the user has pressed 'X'. </summary>
        public event ExitCalled OnProgramExit;

        /// <summary> Is called when the settings to the game map have been closed. </summary>
        public event GameMapSizeChanged OnGameMapSizeChanged;

        /// <summary> Is called when the settings to the lengths of the ships have been closed. </summary>
        public event ShipsLengthChanged OnShipsLengthChanged;

        /// <summary> Is called when the settings to the colors of the ships have been closed. </summary>
        public event ShipsColorsChanged OnShipsColorsChanged;

        /// <summary> Is called when the settings to the amounts of the ships has been closed. </summary>
        public event ShipsAmountChanged OnShipsAmountChanged;

        /// <summary> Is called when the user has pressed 'P'. </summary>
        public event PlayerMapShowing OnPlayerMapShowing;

        /// <summary> Is called when the user has pressed 'E'. </summary>
        public event EnemyMapShowing OnEnemyMapShowing;

        /// <summary>
        /// This method is used for controlling and navigating the menu by an user input from the console. 
        /// It either change the state to show and close different submenus and options or change properties of the ships and the game map.
        /// </summary>
        public void WaitForUserInput()
        {
            ConsoleKeyInfo cki = Console.ReadKey();
            bool falseInput = true;
            
            while (this.state != 0 && falseInput)
            {
                falseInput = false;

                switch (cki.Key)
                {
                    case ConsoleKey.Escape:
                        if (!(falseInput = this.state != 1))
                        {
                            this.state = 0;

                            if (this.OnMenuClosed != null)
                            {
                                this.OnMenuClosed();
                            }
                        }
                        else if (!(falseInput = !(this.state == 2 || this.state == 3 || this.state == 18 || this.state == 19)))
                        {
                            OpenMenu();
                        }
                        else if (!(falseInput = !(this.state >= 4 && this.state <= 5)))
                        {
                            if (this.OnGameMapSizeChanged != null)
                            {
                                this.OnGameMapSizeChanged(this.gameMapSize[0], this.gameMapSize[1]);
                            }

                            OpenSettings();                            
                        }
                        else if (!(falseInput = !(this.state >= 6 && this.state <= 9)))
                        {
                            if (this.OnShipsLengthChanged != null)
                            {
                                this.OnShipsLengthChanged(this.battleCruiserLength, this.cruiserLength, this.destroyerLength, this.submarineLength);
                            }

                            OpenSettings();
                        }
                        else if (!(falseInput = !(this.state >= 10 && this.state <= 13)))
                        {
                            if (this.OnShipsColorsChanged != null)
                            {
                                this.OnShipsColorsChanged(this.shipsColors[0], this.shipsColors[1], this.shipsColors[2], this.shipsColors[3]);
                            }

                            OpenSettings();
                        }
                        else if (!(falseInput = !(this.state >= 14 && this.state <= 17)))
                        {
                            if (this.OnShipsAmountChanged != null)
                            {
                                this.OnShipsAmountChanged(this.battleCruiserAmount, this.cruiserAmount, this.destroyerAmount, this.submarineAmount);
                            }

                            OpenSettings();
                        }

                        break;
                    case ConsoleKey.H:
                        if (!(falseInput = this.state != 1))
                        {
                            this.ChangeStateAndWaitForUserInput(2);
                        }

                        break;
                    case ConsoleKey.S:
                        if (!(falseInput = this.state != 1))
                        {
                            this.OpenSettings();
                        }
                        else if (!(falseInput = this.state != 3))
                        {
                            this.ChangeStateAndWaitForUserInput(6);
                        }

                        break;
                    case ConsoleKey.P:
                        if (!(falseInput = this.state != 1))
                        {
                            this.state = 18;
                            Console.Clear();
                            this.Draw();

                            if (this.OnPlayerMapShowing != null)
                            {
                                this.OnPlayerMapShowing();
                                this.WaitForUserInput();
                            }
                        }

                        break;
                    case ConsoleKey.E:
                        if (!(falseInput = this.state != 1))
                        {
                            this.state = 19;
                            Console.Clear();
                            this.Draw();

                            if (this.OnEnemyMapShowing != null)
                            {
                                this.OnEnemyMapShowing();
                                this.WaitForUserInput();
                            }
                        }

                        break;
                    case ConsoleKey.R:
                        if (!(falseInput = this.state != 1))
                        {
                            this.state = 0;

                            if (this.OnMenuClosed != null)
                            {
                                this.OnMenuClosed();
                            }
                        }

                        break;
                    case ConsoleKey.X:
                        if (!(falseInput = this.state != 1))
                        {
                            if (this.OnProgramExit != null)
                            {
                                this.OnProgramExit();
                            }
                        }

                        break;
                    case ConsoleKey.G:
                        if (!(falseInput = this.state != 3))
                        {
                            this.ChangeStateAndWaitForUserInput(4);
                        }

                        break;
                    case ConsoleKey.C:
                        if (!(falseInput = this.state != 3))
                        {
                            this.ChangeStateAndWaitForUserInput(10);
                        }

                        break;
                    case ConsoleKey.A:
                        if (!(falseInput = this.state != 3))
                        {
                            this.ChangeStateAndWaitForUserInput(14);
                        }

                        break;
                    case ConsoleKey.UpArrow:
                        // change size of map
                        if (!(falseInput = this.state != 5))
                        {
                            this.ChangeStateAndWaitForUserInput(4);
                        }
                        else if (!(falseInput = this.state != 7)) // change size of ships
                        {
                            this.ChangeStateAndWaitForUserInput(6);
                        }
                        else if (!(falseInput = this.state != 8))
                        {
                            this.ChangeStateAndWaitForUserInput(7);
                        }
                        else if (!(falseInput = this.state != 9))
                        {
                            this.ChangeStateAndWaitForUserInput(8);
                        }
                        else if (!(falseInput = this.state != 11)) // change color of ships
                        {
                            this.ChangeStateAndWaitForUserInput(10);
                        }
                        else if (!(falseInput = this.state != 12))
                        {
                            this.ChangeStateAndWaitForUserInput(11);
                        }
                        else if (!(falseInput = this.state != 13))
                        {
                            this.ChangeStateAndWaitForUserInput(12);
                        }
                        else if (!(falseInput = this.state != 15)) // change amount of ships
                        {
                            this.ChangeStateAndWaitForUserInput(14);
                        }
                        else if (!(falseInput = this.state != 16))
                        {
                            this.ChangeStateAndWaitForUserInput(15);
                        }
                        else if (!(falseInput = this.state != 17))
                        {
                            this.ChangeStateAndWaitForUserInput(16);
                        }

                        break;
                    case ConsoleKey.DownArrow:
                        // change size of map
                        if (!(falseInput = this.state != 4))
                        {
                            this.ChangeStateAndWaitForUserInput(5);
                        }
                        else if (!(falseInput = this.state != 6)) // change size of ships
                        {
                            this.ChangeStateAndWaitForUserInput(7);
                        }
                        else if (!(falseInput = this.state != 7))
                        {
                            this.ChangeStateAndWaitForUserInput(8);
                        }
                        else if (!(falseInput = this.state != 8))
                        {
                            this.ChangeStateAndWaitForUserInput(9);
                        }
                        else if (!(falseInput = this.state != 10)) // change color of ships
                        {
                            this.ChangeStateAndWaitForUserInput(11);
                        }
                        else if (!(falseInput = this.state != 11))
                        {
                            this.ChangeStateAndWaitForUserInput(12);
                        }
                        else if (!(falseInput = this.state != 12))
                        {
                            this.ChangeStateAndWaitForUserInput(13);
                        }
                        else if (!(falseInput = this.state != 14)) // change amount of ships
                        {
                            this.ChangeStateAndWaitForUserInput(15);
                        }
                        else if (!(falseInput = this.state != 15))
                        {
                            this.ChangeStateAndWaitForUserInput(16);
                        }
                        else if (!(falseInput = this.state != 16))
                        {
                            this.ChangeStateAndWaitForUserInput(17);
                        }

                        break;
                    case ConsoleKey.OemPlus:
                        // change size of map
                        if (!(falseInput = this.state != 4))
                        {
                            this.ChangeMapSize(this.gameMapSize[0] + 1, this.gameMapSize[1]);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }
                        else if (!(falseInput = this.state != 5))
                        {
                            this.ChangeMapSize(this.gameMapSize[0], this.gameMapSize[1] + 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }

                        // change size of ships
                        else if (!(falseInput = this.state != 6))
                        {
                            this.ChangeBattleCruiserLength(this.battleCruiserLength + 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }
                        else if (!(falseInput = this.state != 7))
                        {
                            this.ChangeCruiserLength(this.cruiserLength + 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }
                        else if (!(falseInput = this.state != 8))
                        {
                            this.ChangeDestroyerLength(this.destroyerLength + 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }
                        else if (!(falseInput = this.state != 9))
                        {
                            this.ChangeSubmarineLength(this.submarineLength + 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }

                        // change color of ships
                        else if (!(falseInput = this.state != 10))
                        {
                            this.ChangeShipColor(0, (int)this.shipsColors[0] + 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }
                        else if (!(falseInput = this.state != 11))
                        {
                            this.ChangeShipColor(1, (int)this.shipsColors[1] + 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }
                        else if (!(falseInput = this.state != 12))
                        {
                            this.ChangeShipColor(2, (int)this.shipsColors[2] + 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }
                        else if (!(falseInput = this.state != 13))
                        {
                            this.ChangeShipColor(3, (int)this.shipsColors[3] + 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }

                        // change amount of ships
                        else if (!(falseInput = this.state != 14))
                        {
                            this.ChangeBattleCruisersAmount(this.battleCruiserAmount + 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }
                        else if (!(falseInput = this.state != 15))
                        {
                            this.ChangeCruisersAmount(this.cruiserAmount + 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }
                        else if (!(falseInput = this.state != 16))
                        {
                            this.ChangeDestroyersAmount(this.destroyerAmount + 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }
                        else if (!(falseInput = this.state != 17))
                        {
                            this.ChangeSubmarinesAmount(this.submarineAmount + 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }

                        break;
                    case ConsoleKey.OemMinus:
                        // change size of map
                        if (!(falseInput = this.state != 4))
                        {
                            this.ChangeMapSize(this.gameMapSize[0] - 1, this.gameMapSize[1]);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }
                        else if (!(falseInput = this.state != 5))
                        {
                            this.ChangeMapSize(this.gameMapSize[0], this.gameMapSize[1] - 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }
                        
                        // change size of ships
                        else if (!(falseInput = this.state != 6))
                        {
                            this.ChangeBattleCruiserLength(this.battleCruiserLength - 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }
                        else if (!(falseInput = this.state != 7))
                        {
                            this.ChangeCruiserLength(this.cruiserLength - 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }
                        else if (!(falseInput = this.state != 8))
                        {
                            this.ChangeDestroyerLength(this.destroyerLength - 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }
                        else if (!(falseInput = this.state != 9))
                        {
                            this.ChangeSubmarineLength(this.submarineLength - 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }
                        
                        // change color of ships
                        else if (!(falseInput = this.state != 10))
                        {
                            this.ChangeShipColor(0, (int)this.shipsColors[0] - 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }
                        else if (!(falseInput = this.state != 11))
                        {
                            this.ChangeShipColor(1, (int)this.shipsColors[1] - 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }
                        else if (!(falseInput = this.state != 12))
                        {
                            this.ChangeShipColor(2, (int)this.shipsColors[2] - 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }
                        else if (!(falseInput = this.state != 13))
                        {
                            this.ChangeShipColor(3, (int)this.shipsColors[3] - 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }
                        
                        // change amount of ships
                        else if (!(falseInput = this.state != 14))
                        {
                            this.ChangeBattleCruisersAmount(this.battleCruiserAmount - 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }
                        else if (!(falseInput = this.state != 15))
                        {
                            this.ChangeCruisersAmount(this.cruiserAmount - 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }
                        else if (!(falseInput = this.state != 16))
                        {
                            this.ChangeDestroyersAmount(this.destroyerAmount - 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }
                        else if (!(falseInput = this.state != 17))
                        {
                            this.ChangeSubmarinesAmount(this.submarineAmount - 1);
                            this.ChangeStateAndWaitForUserInput(this.state);
                        }

                        break;
                    default:
                        falseInput = true;
                        break;
                }

                if (falseInput)
                {
                    cki = Console.ReadKey();
                }
            }
        }

        /// <summary>
        /// This method opens the menu by changing the state.
        /// </summary>
        public void OpenMenu()
        {
            this.ChangeStateAndWaitForUserInput(1);
        }

        /// <summary>
        /// This method shows the settings by changing the state.
        /// </summary>
        public void OpenSettings()
        {
            this.ChangeStateAndWaitForUserInput(3);
        }

        /// <summary>
        /// Used for displaying the menu on the console.
        /// </summary>
        public void Draw()
        {
            this.DrawHeader();

            this.DrawMenu();
        }

        /// <summary>
        /// This method changes the state, redraws the console and waits for an user input.
        /// </summary>
        /// <param name="newState">The new state of the menu.</param>
        private void ChangeStateAndWaitForUserInput(int newState)
        {
            this.state = newState;
            Console.Clear();
            this.Draw();
            this.WaitForUserInput();
        }

        /// <summary>
        /// Changes the size of the map considering the minimum and maximum values.
        /// </summary>
        /// <param name="width">The new width of the game map.</param>
        /// <param name="height">The new height of the game map.</param>
        private void ChangeMapSize(int width, int height)
        {
            if (width > GameMap.MaximalWidth)
            {
                this.gameMapSize[0] = GameMap.MaximalWidth;
            }
            else if (width < GameMap.MinimalWidth)
            {
                this.gameMapSize[0] = GameMap.MinimalWidth;
            }
            else
            {
                this.gameMapSize[0] = width;
            }

            if (height > GameMap.MaximalHeight)
            {
                this.gameMapSize[1] = GameMap.MaximalHeight;
            }
            else if (height < GameMap.MinimalHeight)
            {
                this.gameMapSize[1] = GameMap.MinimalHeight;
            }
            else
            {
                this.gameMapSize[1] = height;
            }
        }

        /// <summary>
        /// Changes the length of the battle cruiser considering the minimum and maximum values.
        /// </summary>
        /// <param name="length">The new length of the battle cruiser.</param>
        private void ChangeBattleCruiserLength(int length)
        {
            if (length > Ship.BattleCruiser.MaximalLength)
            {
                this.battleCruiserLength = Ship.BattleCruiser.MaximalLength;
            }
            else if (length < Ship.BattleCruiser.MinimalLength)
            {
                this.battleCruiserLength = Ship.BattleCruiser.MinimalLength;
            }
            else
            {
                this.battleCruiserLength = length;
            }
        }

        /// <summary>
        /// Changes the length of the cruiser considering the minimum and maximum values.
        /// </summary>
        /// <param name="length">The new length of the cruiser.</param>
        private void ChangeCruiserLength(int length)
        {
            if (length > Ship.Cruiser.MaximalLength)
            {
                this.cruiserLength = Ship.Cruiser.MaximalLength;
            }
            else if (length < Ship.Cruiser.MinimalLength)
            {
                this.cruiserLength = Ship.Cruiser.MinimalLength;
            }
            else
            {
                this.cruiserLength = length;
            }
        }

        /// <summary>
        /// Changes the length of the destroyer considering the minimum and maximum values.
        /// </summary>
        /// <param name="length">The new length of the destroyer.</param>
        private void ChangeDestroyerLength(int length)
        {
            if (length > Ship.Destroyer.MaximalLength)
            {
                this.destroyerLength = Ship.Destroyer.MaximalLength;
            }
            else if (length < Ship.Destroyer.MinimalLength)
            {
                this.destroyerLength = Ship.Destroyer.MinimalLength;
            }
            else
            {
                this.destroyerLength = length;
            }
        }

        /// <summary>
        /// Changes the length of the submarine considering the minimum and maximum values.
        /// </summary>
        /// <param name="length">The new length of the submarine.</param>
        private void ChangeSubmarineLength(int length)
        {
            if (length > Ship.Submarine.MaximalLength)
            {
                this.submarineLength = Ship.Submarine.MaximalLength;
            }
            else if (length < Ship.Submarine.MinimalLength)
            {
                this.submarineLength = Ship.Submarine.MinimalLength;
            }
            else
            {
                this.submarineLength = length;
            }
        }

        /// <summary>
        /// Changes the color of a ship considering the minimum and maximum values.
        /// The type of ship is addressed by the index of the array.
        /// </summary>
        /// <param name="index">The index of the array of ships.</param>
        /// <param name="color">The new color of the ship.</param>
        private void ChangeShipColor(int index, int color)
        {
            if (color > (int)ConsoleColor.White)
            {
                this.shipsColors[index] = ConsoleColor.White;
            }
            else if (color < (int)ConsoleColor.Black)
            {
                this.shipsColors[index] = ConsoleColor.Black;
            }
            else
            {
                this.shipsColors[index] = (ConsoleColor)color;
            }
        }

        /// <summary>
        /// Changes the amount of battle cruisers considering the minimum and maximum values.
        /// </summary>
        /// <param name="amount">The new amount of battle cruisers.</param>
        private void ChangeBattleCruisersAmount(int amount)
        {
            if (amount > GameMap.MaximalAmountBattleCruisers)
            {
                this.battleCruiserAmount = GameMap.MaximalAmountBattleCruisers;
            }
            else if (amount < GameMap.MinimalAmountBattleCruisers)
            {
                this.battleCruiserAmount = GameMap.MinimalAmountBattleCruisers;
            }
            else
            {
                this.battleCruiserAmount = amount;
            }
        }

        /// <summary>
        /// Changes the amount of cruisers considering the minimum and maximum values.
        /// </summary>
        /// <param name="amount">The new amount of cruisers.</param>
        private void ChangeCruisersAmount(int amount)
        {
            if (amount > GameMap.MaximalAmountCruisers)
            {
                this.cruiserAmount = GameMap.MaximalAmountCruisers;
            }
            else if (amount < GameMap.MinimalAmountCruisers)
            {
                this.cruiserAmount = GameMap.MinimalAmountCruisers;
            }
            else
            {
                this.cruiserAmount = amount;
            }
        }

        /// <summary>
        /// Changes the amount of destroyers considering the minimum and maximum values.
        /// </summary>
        /// <param name="amount">The new amount of destroyers.</param>
        private void ChangeDestroyersAmount(int amount)
        {
            if (amount > GameMap.MaximalAmountDestroyers)
            {
                this.destroyerAmount = GameMap.MaximalAmountDestroyers;
            }
            else if (amount < GameMap.MinimalAmountDestroyers)
            {
                this.destroyerAmount = GameMap.MinimalAmountDestroyers;
            }
            else
            {
                this.destroyerAmount = amount;
            }
        }

        /// <summary>
        /// Changes the amount of submarines considering the minimum and maximum values.
        /// </summary>
        /// <param name="amount">The new amount of submarines.</param>
        private void ChangeSubmarinesAmount(int amount)
        {
            if (amount > GameMap.MaximalAmountSubmarines)
            {
                this.submarineAmount = GameMap.MaximalAmountSubmarines;
            }
            else if (amount < GameMap.MinimalAmountSubmarines)
            {
                this.submarineAmount = GameMap.MinimalAmountSubmarines;
            }
            else
            {
                this.submarineAmount = amount;
            }
        }

        /// <summary>
        /// Draws the header of the menu, which shows useful information to the respective submenus.
        /// </summary>
        private void DrawHeader()
        {
            Console.SetCursorPosition(0, 0);

            if (this.state == 0)
            {
                Console.WriteLine("\n [ESC] Open menu [N] Start new game");
            }
            else if (this.state == 1)
            {
                Console.WriteLine("\n [ESC] Close menu");
            }
            else if (this.state == 2)
            {
                Console.WriteLine("\n [ESC] Close help");
            }
            else if (this.state == 3)
            {
                Console.WriteLine("\n [ESC] Close settings");
            }
            else if (this.state >= 4 && this.state <= 5)
            {
                Console.WriteLine("\n [ESC] Close and apply settings [Up / Down] Navigate [+ / -] Change values");
            }
            else if (this.state >= 6 && this.state <= 9)
            {
                Console.WriteLine("\n [ESC] Close and apply settings [Up / Down] Navigate [+ / -] Change values");
            }
            else if (this.state >= 10 && this.state <= 13)
            {
                Console.WriteLine("\n [ESC] Close and apply settings [Up / Down] Navigate [+ / -] Change values");
            }
            else if (this.state >= 14 && this.state <= 17)
            {
                Console.WriteLine("\n [ESC] Close and apply settings [Up / Down] Navigate [+ / -] Change values");
            }
            else if (this.state == 18)
            {
                Console.WriteLine("\n [ESC] Close view of your map");
            }
            else if (this.state == 19)
            {
                Console.WriteLine("\n [ESC] Close view of your enemy's map");
            }
        }

        /// <summary>
        /// Draws the menu and submenus with the respective options, current values and minimum and maximum values.
        /// </summary>
        private void DrawMenu()
        {
            Console.SetCursorPosition(0, 3);

            if (this.state == 1)
            {
                Console.WriteLine(" [H] Help\n\n [S] Settings\n\n [P] Show your game map\n\n [E] Show map of enemy\n\n [R] Resume\n\n [X] Exit");
            }
            else if (this.state == 2)
            {
                Console.WriteLine(" [H] Help\n");
                Console.WriteLine("     * General\n");
                Console.WriteLine("       Before you can play, you have to arrange all ships on your game map. You");
                Console.WriteLine("       either do this manually or use the generator. After positioning, the ");
                Console.WriteLine("       game proceeds in a series of rounds. In each round you guess some ");
                Console.WriteLine("       coordinates where you assume a hostile ship. Winner is who destroyed all");
                Console.WriteLine("       hostile ships first!\n");

                Console.WriteLine("     * Rules\n");
                Console.WriteLine("       - The beginner of a match will be choosen random.\n");
                Console.WriteLine("       - Ships cannot border directly, but diagonally.");
                Console.WriteLine("       - Ships can touch the edge.\n");
                Console.WriteLine("       - If you hit a ship, you can guess some other coordinates.");
                Console.WriteLine("       - Your turn ends as soon as you hit nothing or an already damaged ship.\n");
            }
            else if (this.state == 3)
            {
                Console.WriteLine(" [S] Settings to\n");
                Console.WriteLine("     [G] Game map\n");
                Console.WriteLine("     [S] Ships (Size)\n");
                Console.WriteLine("     [C] Ships (Color)\n");
                Console.WriteLine("     [A] Ships (Amount)\n");
            }
            else if (this.state >= 4 && this.state <= 5)
            {
                Console.WriteLine(" [S] Settings to game map\n");
                Console.WriteLine("     [G] Game map\n");

                if (this.state == 4)
                {
                    Console.WriteLine("         [*] Width:  {0}  [{1} - {2}]", this.gameMapSize[0], GameMap.MinimalWidth, GameMap.MaximalWidth);
                    Console.WriteLine("\n         [ ] Height: {0}  [{1} - {2}]", this.gameMapSize[1], GameMap.MinimalHeight, GameMap.MaximalHeight);
                }
                else
                {
                    Console.WriteLine("         [ ] Width:  {0}  [{1} - {2}]", this.gameMapSize[0], GameMap.MinimalWidth, GameMap.MaximalWidth);
                    Console.WriteLine("\n         [*] Height: {0}  [{1} - {2}]", this.gameMapSize[1], GameMap.MinimalHeight, GameMap.MaximalHeight);
                }

                Console.WriteLine("\n     -> To apply these settings, you have to start a new game!");
            }
            else if (this.state >= 6 && this.state <= 9)
            {
                Console.WriteLine(" [S] Settings to Ships (Size)\n");
                Console.WriteLine("     [S] Ships (Size)");

                char battlecruiserToken = ' ';
                char cruiserToken = ' ';
                char destroyerToken = ' ';
                char submarineToken = ' ';

                if (this.state == 6)
                {
                    battlecruiserToken = '*';
                }
                else if (this.state == 7)
                {
                    cruiserToken = '*';
                }
                else if (this.state == 8)
                {
                    destroyerToken = '*';
                }
                else if (this.state == 9)
                {
                    submarineToken = '*';
                }

                Console.WriteLine("\n         [{0}] Battle cruiser length:  {1, -2}  [{2} - {3}]", battlecruiserToken, this.battleCruiserLength, Ship.BattleCruiser.MinimalLength, Ship.BattleCruiser.MaximalLength);
                Console.WriteLine("\n         [{0}] Cruiser length:         {1, -2}  [{2} - {3}]", cruiserToken, this.cruiserLength, Ship.Cruiser.MinimalLength, Ship.Cruiser.MaximalLength);
                Console.WriteLine("\n         [{0}] Destroyer length:       {1, -2}  [{2} - {3}]", destroyerToken, this.destroyerLength, Ship.Destroyer.MinimalLength, Ship.Destroyer.MaximalLength);
                Console.WriteLine("\n         [{0}] Submarine length:       {1, -2}  [{2} - {3}]", submarineToken, this.submarineLength, Ship.Submarine.MinimalLength, Ship.Submarine.MaximalLength);
                
                Console.WriteLine("\n     -> To apply these settings, you have to start a new game!");
            }
            else if (this.state >= 10 && this.state <= 13)
            {
                Console.WriteLine(" [S] Settings to Ships (Color)\n");
                Console.WriteLine("     [S] Ships (Color)");

                char battlecruiserToken = ' ';
                char cruiserToken = ' ';
                char destroyerToken = ' ';
                char submarineToken = ' ';

                if (this.state == 10)
                {
                    battlecruiserToken = '*';
                }
                else if (this.state == 11)
                {
                    cruiserToken = '*';
                }
                else if (this.state == 12)
                {
                    destroyerToken = '*';
                }
                else if (this.state == 13)
                {
                    submarineToken = '*';
                }

                Console.Write("\n         [{0}] Battle cruiser: ", battlecruiserToken);
                this.DrawShipInColor(this.shipsColors[0]);
                Console.WriteLine(" {0, -2} - {1, -12} [{2} - {3}]", (int)this.shipsColors[0], this.shipsColors[0].ToString(), (int)ConsoleColor.Black, (int)ConsoleColor.White);

                Console.Write("\n         [{0}] Cruiser:        ", cruiserToken);
                this.DrawShipInColor(this.shipsColors[1]);
                Console.WriteLine(" {0, -2} - {1, -12} [{2} - {3}]", (int)this.shipsColors[1], this.shipsColors[1].ToString(), (int)ConsoleColor.Black, (int)ConsoleColor.White);

                Console.Write("\n         [{0}] Destroyer:      ", destroyerToken);
                this.DrawShipInColor(this.shipsColors[2]);
                Console.WriteLine(" {0, -2} - {1, -12} [{2} - {3}]", (int)this.shipsColors[2], this.shipsColors[2].ToString(), (int)ConsoleColor.Black, (int)ConsoleColor.White);

                Console.Write("\n         [{0}] Submarine:      ", submarineToken);
                this.DrawShipInColor(this.shipsColors[3]);
                Console.WriteLine(" {0, -2} - {1, -12} [{2} - {3}]", (int)this.shipsColors[3], this.shipsColors[3].ToString(), (int)ConsoleColor.Black, (int)ConsoleColor.White);
            }
            else if (this.state >= 14 && this.state <= 17)
            {
                Console.WriteLine(" [S] Settings to Ships (Amount)\n");
                Console.WriteLine("     [S] Ships (Amount)");

                char battlecruiserToken = ' ';
                char cruiserToken = ' ';
                char destroyerToken = ' ';
                char submarineToken = ' ';

                if (this.state == 14)
                {
                    battlecruiserToken = '*';
                }
                else if (this.state == 15)
                {
                    cruiserToken = '*';
                }
                else if (this.state == 16)
                {
                    destroyerToken = '*';
                }
                else if (this.state == 17)
                {
                    submarineToken = '*';
                }

                Console.WriteLine("\n         [{0}] Amount of battle cruisers:  {1, -2}  [{2} - {3}]", battlecruiserToken, this.battleCruiserAmount, GameMap.MinimalAmountBattleCruisers, GameMap.MaximalAmountBattleCruisers);
                Console.WriteLine("\n         [{0}] Amount of cruisers:         {1, -2}  [{2} - {3}]", cruiserToken, this.cruiserAmount, GameMap.MinimalAmountCruisers, GameMap.MaximalAmountCruisers);
                Console.WriteLine("\n         [{0}] Amount of destroyers:       {1, -2}  [{2} - {3}]", destroyerToken, this.destroyerAmount, GameMap.MinimalAmountDestroyers, GameMap.MaximalAmountDestroyers);
                Console.WriteLine("\n         [{0}] Amount of submarines:       {1, -2}  [{2} - {3}]", submarineToken, this.submarineAmount, GameMap.MinimalAmountSubmarines, GameMap.MaximalAmountSubmarines);

                Console.WriteLine("\n     -> To apply these settings, you have to start a new game!");
            }
        }

        /// <summary>
        /// Draws a sample of a ship in a specific color.
        /// </summary>
        /// <param name="c">The color of the ship to be drawn.</param>
        private void DrawShipInColor(ConsoleColor c)
        {
            ConsoleColor tempC = Console.ForegroundColor;

            Console.ForegroundColor = c;

            Console.Write("####");

            Console.ForegroundColor = tempC;
        }
    }
}
