//-----------------------------------------------------------------------
// <copyright file="GameSounds.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class provides static methods to play sound effects for different events.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class provides static methods to play sound effects for different events.
    /// </summary>
    public static class GameSounds
    {
        /// <summary>
        /// Is played when the human hit a hostile ship.
        /// </summary>
        public static void PlaySound_HumanHit()
        {
            Console.Beep(600, 200);
            Console.Beep(400, 200);
        }

        /// <summary>
        /// Is played when the human hit a hostile ship.
        /// </summary>
        public static void PlaySound_HumanMissed()
        {
            Console.Beep(200, 200);
        }

        /// <summary>
        /// Is played when the AI hit a hostile ship.
        /// </summary>
        public static void PlaySound_AIHit()
        {
            Console.Beep(1200, 200);
            Console.Beep(800, 200);
        }

        /// <summary>
        /// Is played when the human destroyed a ship.
        /// </summary>
        public static void PlaySound_HumanDestroyedShip()
        {
            Console.Beep(600, 150);
            Console.Beep(500, 150);
            Console.Beep(800, 200);
        }

        /// <summary>
        /// Is played when the human won the game.
        /// </summary>
        public static void PlaySound_HumanWon()
        {
            Console.Beep(800, 200);
            Console.Beep(400, 200);
            Console.Beep(800, 200);
            Console.Beep(400, 200);
            Console.Beep(800, 200);
        }

        /// <summary>
        /// Is played when the human lost the game.
        /// </summary>
        public static void PlaySound_HumanLost()
        {
            Console.Beep(500, 500);
            Console.Beep(300, 250);
            Console.Beep(400, 500);
            Console.Beep(200, 1000);
        }
    }
}
