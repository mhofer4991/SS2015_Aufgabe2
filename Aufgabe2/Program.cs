//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>The user can use this program to play a match battle ship against the computer.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The user can use this program to play a match battle ship against the computer.
    /// </summary>
    public class Program
    {
        /// <summary> Constant value for the game state at the beginning. </summary>
        private const int GameStateMain = 0;

        /// <summary> Constant value for the game state at a new game. </summary>
        private const int GameStateNewGame = 1;

        /// <summary> Constant value for the game state at a random new game. </summary>
        private const int GameStateNewGameRandom = 2;

        /// <summary> Constant value for the game state when playing a game. </summary>
        private const int GameStatePlaying = 3;

        /// <summary> Constant duration the AI sleeps to make it think. </summary>
        private const int AISleepDuration = 500;

        /// <summary> Menu, where the user can change settings to the game. </summary>
        private static Menu menu;

        /// <summary> First player of a match. </summary>
        private static Player player1;

        /// <summary> Second player of a match. </summary>
        private static Player player2;

        /// <summary> Game map of the first player. </summary>
        private static GameMap player1Map;

        /// <summary> Game map of the second player. </summary>
        private static GameMap player2Map;

        /// <summary> Saves the progress of a game and shows it to the user. </summary>
        private static string gameLog;

        /// <summary> Indicates whether the first or the second player has it's turn. </summary>
        private static bool player1Draw;

        /// <summary> Indicates whether the map of the first or the second player will be drawn. </summary>
        private static bool map1Visible;

        /// <summary> Indicates whether a game is currently running or not. </summary>
        private static bool playing;

        /// <summary> Current size of the game maps. </summary>
        private static int[] gameMapSize;

        /// <summary> Current width of the ships. </summary>
        private static int[] shipsLengths;

        /// <summary> Current color of the ships. </summary>
        private static ConsoleColor[] shipsColors;

        /// <summary> Current amount of the ships. </summary>
        private static int[] shipsAmounts;

        /// <summary> Current state of the game. </summary>
        private static int gameState;

        /// <summary>
        /// This method represents the entry point of the program.
        /// </summary>
        /// <param name="args">Array of command line arguments.</param>
        public static void Main(string[] args)
        {
            menu = new Menu();

            gameMapSize = new int[] { GameMap.MinimalWidth, GameMap.MinimalHeight };

            shipsLengths = new int[] { Ship.BattleCruiser.MinimalLength, Ship.Cruiser.MinimalLength, Ship.Destroyer.MinimalLength, Ship.Submarine.MinimalLength };

            shipsColors = new ConsoleColor[] { Ship.BattleCruiser.DefaultColor, Ship.Cruiser.DefaultColor, Ship.Destroyer.DefaultColor, Ship.Submarine.DefaultColor };

            shipsAmounts = new int[] { GameMap.MinimalAmountBattleCruisers, GameMap.MinimalAmountCruisers, GameMap.MinimalAmountDestroyers, GameMap.MinimalAmountSubmarines };

            // Add menu events.
            menu.OnMenuClosed += Menu_OnMenuClosed;
            menu.OnProgramExit += Menu_OnProgramExit;

            menu.OnGameMapSizeChanged += Menu_OnGameMapSizeChanged;
            menu.OnShipsLengthChanged += Menu_OnShipsLengthChanged;
            menu.OnShipsColorsChanged += Menu_OnShipsColorsChanged;
            menu.OnShipsAmountChanged += Menu_OnShipsAmountChanged;

            menu.OnPlayerMapShowing += Menu_OnPlayerMapShowing;
            menu.OnEnemyMapShowing += Menu_OnEnemyMapShowing;

            // Create players and maps.
            player1Map = new GameMap(10, 10);
            player2Map = new GameMap(10, 10);

            player2 = new AI("Enemy", player2Map, player1Map);
            player1 = new Human(new Human.GlobalInputHandler(CheckInput), "Human", player1Map, player2Map);

            // Set properties of maps.
            player1Map.SetNewPosition(1, 3);
            player2Map.SetNewPosition(1, 3);

            player1Map.SetShipsHiding(false);
            player2Map.SetShipsHiding(true);

            // Add events of players.
            player1.OnPlayerMissed += Player_OnPlayerMissed;
            player2.OnPlayerMissed += Player_OnPlayerMissed;

            player1.OnPlayerHit += Player1_OnPlayerHit;
            player2.OnPlayerHit += Player2_OnPlayerHit;

            player1.OnPlayerDestroyedShip += Player1_OnPlayerDestroyedShip;

            player1.OnPlayerLost += Player1_OnPlayerLost;
            player1.OnPlayerWon += Player1_OnPlayerWon;
            
            // Begin program.
            gameState = GameStateMain;

            map1Visible = true;

            Draw();

            ConsoleKeyInfo cki = Console.ReadKey();

            while (true)
            {
                CheckInput(cki);

                cki = Console.ReadKey();
            }
        }

        /// <summary>
        /// Gets called when a player missed a ship.
        /// </summary>
        /// <param name="player">The player, who missed.</param>
        /// <param name="x">X - coordinate of the missed square.</param>
        /// <param name="y">Y - coordinate of the missed square.</param>
        private static void Player_OnPlayerMissed(Player player, int x, int y)
        {
            GameSounds.PlaySound_HumanMissed();
        }

        /// <summary>
        /// Gets called when the first player hit a hostile ship.
        /// </summary>
        /// <param name="player">The player, who hit.</param>
        /// <param name="x">X - coordinate of the hit square.</param>
        /// <param name="y">Y - coordinate of the hit square.</param>
        private static void Player1_OnPlayerHit(Player player, int x, int y)
        {
            GameSounds.PlaySound_HumanHit();
        }

        /// <summary>
        /// Gets called when the second player hit a hostile ship.
        /// </summary>
        /// <param name="player">The player, who hit.</param>
        /// <param name="x">X - coordinate of the hit square.</param>
        /// <param name="y">Y - coordinate of the hit square.</param>
        private static void Player2_OnPlayerHit(Player player, int x, int y)
        {
            GameSounds.PlaySound_AIHit();
        }

        /// <summary>
        /// Gets called when the first player destroyed a ship.
        /// </summary>
        /// <param name="player">The player, who has destroyed a ship..</param>
        /// <param name="s">Ship, which has been destroyed.</param>
        private static void Player1_OnPlayerDestroyedShip(Player player, Ship s)
        {
            GameSounds.PlaySound_HumanDestroyedShip();
        }

        /// <summary>
        /// Gets called when the first player lost.
        /// </summary>
        /// <param name="player">The player, who lost.</param>
        private static void Player1_OnPlayerLost(Player player)
        {
            GameSounds.PlaySound_HumanLost();
        }

        /// <summary>
        /// Gets called when the first player won.
        /// </summary>
        /// <param name="player">The player, who won.</param>
        private static void Player1_OnPlayerWon(Player player)
        {
            GameSounds.PlaySound_HumanWon();
        }

        /// <summary>
        /// Gets called when the user enters the option [E] in the menu.
        /// </summary>
        private static void Menu_OnEnemyMapShowing()
        {
            player2Map.SetShipsHiding(false);
            player2Map.Draw();
            player2Map.SetShipsHiding(true);
            DrawInfo();
        }

        /// <summary>
        /// Gets called when the user enters the option [P] in the menu.
        /// </summary>
        private static void Menu_OnPlayerMapShowing()
        {
            player1Map.Draw();
            DrawInfo();
        }

        /// <summary>
        /// Gets called when the user changed the settings to the amounts of the ships.
        /// </summary>
        /// <param name="battleCruiserAmount">New amount of battle cruisers.</param>
        /// <param name="cruiserAmount">New amount of cruisers.</param>
        /// <param name="destroyerAmount">New amount of destroyers.</param>
        /// <param name="submarineAmount">New amount of submarines.</param>
        private static void Menu_OnShipsAmountChanged(int battleCruiserAmount, int cruiserAmount, int destroyerAmount, int submarineAmount)
        {
            shipsAmounts[0] = battleCruiserAmount;
            shipsAmounts[1] = cruiserAmount;
            shipsAmounts[2] = destroyerAmount;
            shipsAmounts[3] = submarineAmount;
        }

        /// <summary>
        /// Gets called when the user changed the settings to the colors of the ships.
        /// </summary>
        /// <param name="battleCruiserColor">New color of the battle cruiser.</param>
        /// <param name="cruiserColor">New color of the cruiser.</param>
        /// <param name="destroyerColor">New color of the destroyer.</param>
        /// <param name="submarineColor">New color of the submarine.</param>
        private static void Menu_OnShipsColorsChanged(ConsoleColor battleCruiserColor, ConsoleColor cruiserColor, ConsoleColor destroyerColor, ConsoleColor submarineColor)
        {
            shipsColors[0] = battleCruiserColor;
            shipsColors[1] = cruiserColor;
            shipsColors[2] = destroyerColor;
            shipsColors[3] = submarineColor;

            // The settings can also be applied to the currently running game.
            player1Map.SetShipsColors(shipsColors[0], shipsColors[1], shipsColors[2], shipsColors[3]);
            player2Map.SetShipsColors(shipsColors[0], shipsColors[1], shipsColors[2], shipsColors[3]);

            // Draw();
        }

        /// <summary>
        /// Gets called when the user changed the settings to the widths of the ships.
        /// </summary>
        /// <param name="battleCruiserLength">New width of the battle cruiser.</param>
        /// <param name="cruiserLength">New width of the cruiser.</param>
        /// <param name="destroyerLength">New width of the destroyer.</param>
        /// <param name="submarineLength">New width of the submarine.</param>
        private static void Menu_OnShipsLengthChanged(int battleCruiserLength, int cruiserLength, int destroyerLength, int submarineLength)
        {
            shipsLengths[0] = battleCruiserLength;
            shipsLengths[1] = cruiserLength;
            shipsLengths[2] = destroyerLength;
            shipsLengths[3] = submarineLength;
        }

        /// <summary>
        /// Gets called when the user changed the settings to the size of the game map.
        /// </summary>
        /// <param name="width">New width of the game map.</param>
        /// <param name="height">New height of the game map.</param>
        private static void Menu_OnGameMapSizeChanged(int width, int height)
        {
            gameMapSize[0] = width;
            gameMapSize[1] = height;
        }

        /// <summary>
        /// Gets called when the user wants to exit the program.
        /// </summary>
        private static void Menu_OnProgramExit()
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Gets called when the menu is closed.
        /// </summary>
        private static void Menu_OnMenuClosed()
        {
            Console.Clear();
            Draw();
        }

        /// <summary>
        /// Checks the user input for valid commands.
        /// </summary>
        /// <param name="cki">ConsoleKeyInfo, which will be evaluated.</param>
        private static void CheckInput(ConsoleKeyInfo cki)
        {
            switch (cki.Key)
            {
                case ConsoleKey.Escape:
                    menu.OpenMenu();
                    break;
                case ConsoleKey.N:
                    GenerateNewGame();
                    break;
                case ConsoleKey.R:
                    if (gameState == GameStateNewGame)
                    {
                        player1Map.GenerateNewGameMap();
                        gameState = GameStateNewGameRandom;
                    }
                    
                    if (gameState == GameStateNewGameRandom)
                    {
                        player1Map.RandomizeShips();
                    }

                    Console.Clear();
                    Draw();

                    break;
                case ConsoleKey.Enter:
                    if (gameState == GameStateNewGameRandom)
                    {
                        StartGame();
                    }

                    break;
            }
        }

        /// <summary>
        /// Creates a new game where the user can add ships manually or random.
        /// </summary>
        private static void GenerateNewGame()
        {
            // Resets players and apply all settings to both game maps.
            gameState = GameStateNewGame;

            player2.Reset();
            player1.Reset();

            playing = false;
            gameLog = string.Empty;

            player2Map.SetNewSize(gameMapSize[0], gameMapSize[1]);

            player2Map.SetShipsLenghts(shipsLengths[0], shipsLengths[1], shipsLengths[2], shipsLengths[3]);

            player2Map.SetShipsColors(shipsColors[0], shipsColors[1], shipsColors[2], shipsColors[3]);

            player2Map.SetShipsAmounts(shipsAmounts[0], shipsAmounts[1], shipsAmounts[2], shipsAmounts[3]);

            player2Map.ShipCollection.Reset();

            player1Map.SetNewSize(gameMapSize[0], gameMapSize[1]);

            player1Map.SetShipsLenghts(shipsLengths[0], shipsLengths[1], shipsLengths[2], shipsLengths[3]);

            player1Map.SetShipsColors(shipsColors[0], shipsColors[1], shipsColors[2], shipsColors[3]);

            player1Map.SetShipsAmounts(shipsAmounts[0], shipsAmounts[1], shipsAmounts[2], shipsAmounts[3]);

            player1Map.ShipCollection.Reset();

            Console.Clear();
            Draw();

            // Create the ships of the game maps.
            try
            {
                player2Map.GenerateNewGameMap();

                player2Map.RandomizeShips();

                AddShipsManually(player1Map);
            }
            catch (NoPlaceForShipException)
            {
                // Inform the user that his current settings don't allow enough place for all ships on a map.
                gameState = GameStateMain;
                Console.Clear();
                Draw();

                Console.SetCursorPosition(player1Map.X + player1Map.Width + 7, player1Map.Y);
                Console.WriteLine("ERROR: The game map could not be generated!");
                Console.SetCursorPosition(player1Map.X + player1Map.Width + 7, player1Map.Y + 2);
                Console.WriteLine("To solve the problem you either:");
                Console.SetCursorPosition(player1Map.X + player1Map.Width + 7, player1Map.Y + 4);
                Console.WriteLine("         - increase the map size");
                Console.SetCursorPosition(player1Map.X + player1Map.Width + 7, player1Map.Y + 6);
                Console.WriteLine("         - decrease the amount of the ships");
                Console.SetCursorPosition(player1Map.X + player1Map.Width + 7, player1Map.Y + 8);
                Console.WriteLine("         - decrease the size of the ships");
            }
        }

        /// <summary>
        /// Adds ships manually to a game map by an user input.
        /// </summary>
        /// <param name="gameMap">Game map, where the ships will be placed.</param>
        private static void AddShipsManually(GameMap gameMap)
        {
            // Empty the map.
            gameMap.ResetShipCollection();

            ConsoleKeyInfo cki = new ConsoleKeyInfo();
            Ship s = null;
            int[] pos;

            while (gameMap.ShipCollection.SavedShips < gameMap.ShipCollection.AmountShips && gameState == GameStateNewGame)
            {
                // Create a new ship with a predefined position, which the user then can change.
                if (s == null)
                {
                    if (gameMap.ShipCollection.SavedBattleCruisers < gameMap.AmountBattleCruisers)
                    {
                        s = gameMap.GetNewBattleCruiser();
                    }
                    else if (gameMap.ShipCollection.SavedCruisers < gameMap.AmountCruisers)
                    {
                        s = gameMap.GetNewCruiser();
                    }
                    else if (gameMap.ShipCollection.SavedDestroyers < gameMap.AmountDestroyers)
                    {
                        s = gameMap.GetNewDestroyer();
                    }
                    else
                    {
                        s = gameMap.GetNewSubmarine();
                    }
                }

                gameMap.Draw();

                // Ship has not been added yet, so it has to be drawn extra.
                s.Draw();

                gameMap.SetCursorPosition(s.X, s.Y);

                // Show coordinates, where the ship will be placed.
                Console.SetCursorPosition(player1Map.X + player1Map.Width + 7, player1Map.Y + 1);
                Console.WriteLine(GameMap.GetFieldLabel(s.X, s.Y) + "  ");

                Console.SetCursorPosition(gameMap.GetDrawingStartPoint()[0] + gameMap.CursorX, gameMap.GetDrawingStartPoint()[1] + gameMap.CursorY);
                cki = Console.ReadKey();

                CheckInput(cki);

                pos = gameMap.GetPositionFromCursorInput(cki);

                if (cki.Key == ConsoleKey.Enter)
                {
                    // Check if the ship can be added at this position.
                    if (s.SetPosition(pos[0], pos[1]))
                    {
                        gameMap.ShipCollection.AddShip(s);
                        s = null;

                        if (gameMap.ShipCollection.SavedShips >= gameMap.ShipCollection.AmountShips)
                        {
                            // If all ships have been placed, the game can start.
                            StartGame();
                        }
                    }
                }
                else if (cki.Key == ConsoleKey.Spacebar)
                {
                    s.Rotate();
                }
                else
                {
                    s.Move(pos[0], pos[1]);
                }                
            }
        }

        /// <summary>
        /// Chooses for the first turn a random player and then starts the game.
        /// </summary>
        private static void StartGame()
        {
            player1Draw = new Random().Next(0, 2) == 1;

            gameState = GameStatePlaying;
            playing = true;

            player1.StartGame();
            player2.StartGame();

            while (playing)
            {
                Console.Clear();
                Draw();

                if (player1Draw)
                {
                    MakeMove(player1);
                    player1Draw = false;
                }
                else
                {
                    MakeMove(player2);
                    player1Draw = true;
                }
            }
        }

        /// <summary>
        /// Represents a round of a player. 
        /// </summary>
        /// <param name="player">Player, which makes the round.</param>
        private static void MakeMove(Player player)
        {
            bool missed = false;
            ConsoleKeyInfo cki = new ConsoleKeyInfo();

            gameLog = string.Empty;

            Console.Clear();
            Draw();

            int[] pos = player.Move();

            // As long as the player didn't miss and a new game hasn't been started, the player will be asked for some coordinates.
            while (playing && !missed)
            {
                // Add all actions to the game log.
                if (!player.EnemyMap.HitAtPoint(pos[0], pos[1]))
                {
                    missed = true;
                    gameLog += player.Name + " missed at " + GameMap.GetFieldLabel(pos[0], pos[1]) + ";";
                    gameLog += "Press [Enter] to continue!;";
                } 
                else
                {
                    player.GotHit(pos[0], pos[1]);
                    gameLog += player.Name + " hit at " + GameMap.GetFieldLabel(pos[0], pos[1]) + ";";

                    if (player.EnemyMap.GetShip(pos[0], pos[1]).IsDestroyed())
                    {
                        player.ShipRecessed(player.EnemyMap.GetShip(pos[0], pos[1]));
                        gameLog += player.Name + " recessed ship at " + GameMap.GetFieldLabel(pos[0], pos[1]) + ";";
                    }

                    if (player.EnemyMap.AllShipsDestroyed())
                    {
                        gameLog += ">>>> " + player.Name + " won!!!! <<<<;";
                        playing = false;
                    }
                }

                Console.Clear();
                Draw();

                if (!missed && playing)
                {
                    // Make the game more realistic by letting the AI think.
                    if (player is AI)
                    {
                        System.Threading.Thread.Sleep(AISleepDuration);
                    }

                    pos = player.Move();
                }
            }

            // Waits for the user to press Enter to give the turn to the next player.
            while (playing && cki.Key != ConsoleKey.Enter)
            {
                cki = Console.ReadKey();

                CheckInput(cki);
            }
        }

        /// <summary>
        /// Draws the program on the console.
        /// </summary>
        private static void Draw()
        {
            menu.Draw();

            if (!playing)
            {
                if (map1Visible)
                {
                    player1Map.Draw();
                }
                else
                {
                    player2Map.Draw();
                }
            }
            else
            {
                if (player1Draw)
                {
                    player2Map.Draw();
                }
                else
                {
                    player1Map.Draw();
                }
            }

            // Show infos to the different game states.
            if (gameState == GameStateNewGame)
            {
                Console.SetCursorPosition(player1Map.X + player1Map.Width + 6, player1Map.Y + 3);
                Console.WriteLine(" [R]       randomize the positions");
                Console.SetCursorPosition(player1Map.X + player1Map.Width + 6, player1Map.Y + 5);
                Console.WriteLine(" [Arrows]  manually move the ships");
                Console.SetCursorPosition(player1Map.X + player1Map.Width + 6, player1Map.Y + 7);
                Console.WriteLine(" [Space]   rotate the ships");
                Console.SetCursorPosition(player1Map.X + player1Map.Width + 6, player1Map.Y + 9);
                Console.WriteLine(" [Enter]   confirm the positions");
                Console.SetCursorPosition(player1Map.X + player1Map.Width + 6, player1Map.Y + 11);
                Console.WriteLine(" * Rules: ");
                Console.SetCursorPosition(player1Map.X + player1Map.Width + 6, player1Map.Y + 13);
                Console.WriteLine(" Ships cannot border directly, but diagonally.");
                Console.SetCursorPosition(player1Map.X + player1Map.Width + 6, player1Map.Y + 15);
                Console.WriteLine(" Ships can touch the edge.");
            }
            else if (gameState == GameStateNewGameRandom)
            {
                // If a user randomized the positions, he cannot change the positions manually anymore.
                Console.SetCursorPosition(player1Map.X + player1Map.Width + 6, player1Map.Y + 1);
                Console.WriteLine(" To manually set the positions, press [N].");
                Console.SetCursorPosition(player1Map.X + player1Map.Width + 6, player1Map.Y + 3);
                Console.WriteLine(" [R]       randomize the positions");
                Console.SetCursorPosition(player1Map.X + player1Map.Width + 6, player1Map.Y + 9);
                Console.WriteLine(" [Enter]   confirm the positions");
            }
            else if (gameState == GameStatePlaying)
            {
                // Show game log to the user.
                string[] logs = gameLog.Split(';');

                for (int i = 0; i < logs.Length - 1; i++)
                {
                    Console.SetCursorPosition(player1Map.X + player1Map.Width + 6, player1Map.Y + 3 + (i * 2));
                    Console.WriteLine(" " + logs[i].ToString());
                }
            }
        }

        /// <summary>
        /// Info, which will be shown when the user wants to see his map or the map of the enemy in the settings.
        /// </summary>
        private static void DrawInfo()
        {
            Console.SetCursorPosition(player1Map.GetDrawingStartPoint()[0] + player1Map.Width + 2, player1Map.GetDrawingStartPoint()[1]);
            Console.WriteLine(" Key: ");
            Console.SetCursorPosition(player1Map.GetDrawingStartPoint()[0] + player1Map.Width + 2, player1Map.GetDrawingStartPoint()[1] + 2);
            Console.WriteLine(" <   >  Empty");
            Console.SetCursorPosition(player1Map.GetDrawingStartPoint()[0] + player1Map.Width + 2, player1Map.GetDrawingStartPoint()[1] + 4);
            Console.WriteLine(" < - >  Missed");
            Console.SetCursorPosition(player1Map.GetDrawingStartPoint()[0] + player1Map.Width + 2, player1Map.GetDrawingStartPoint()[1] + 6);
            Console.WriteLine(" < # >  Intact ship");
            Console.SetCursorPosition(player1Map.GetDrawingStartPoint()[0] + player1Map.Width + 2, player1Map.GetDrawingStartPoint()[1] + 8);
            Console.WriteLine(" < X >  Hit ship");
            Console.SetCursorPosition(player1Map.GetDrawingStartPoint()[0] + player1Map.Width + 2, player1Map.GetDrawingStartPoint()[1] + 10);
            Console.WriteLine(" < S >  Sunken ship");
        }
    }
}
