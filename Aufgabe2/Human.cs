//-----------------------------------------------------------------------
// <copyright file="Human.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>Represents a battle ship player, which will be controlled by an user input.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a battle ship player, which will be controlled by an user input.
    /// </summary>
    public class Human : Player
    {
        /// <summary>
        /// Represents a method, which checks for globally available commands. 
        /// </summary>
        private GlobalInputHandler globalInputHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="Human"/> class.
        /// </summary>
        /// <param name="globalInputHandler">Input handler to look for global commands.</param>
        /// <param name="name">Name of the human.</param>
        /// <param name="map">Map of the human.</param>
        /// <param name="enemyMap">Hostile map of the human.</param>
        public Human(GlobalInputHandler globalInputHandler, string name, GameMap map, GameMap enemyMap)
            : base(name, map, enemyMap)
        {
            this.globalInputHandler = globalInputHandler;
        }

        /// <summary>
        /// Delegate, which passes user inputs to an input handler to check them for globally available commands like ESC.  
        /// </summary>
        /// <param name="cki">ConsoleKeyInfo, which will be passed.</param>
        public delegate void GlobalInputHandler(ConsoleKeyInfo cki);

        /// <summary>
        /// Gets the coordinates on the hostile map, where the human assumes a ship.
        /// </summary>
        /// <returns> An integer array, which contains the coordinates. </returns>
        public override int[] Move()
        {
            ConsoleKeyInfo cki = new ConsoleKeyInfo();

            while (this.IsPlaying && cki.Key != ConsoleKey.Enter)
            {
                Console.SetCursorPosition(EnemyMap.X + EnemyMap.Width + 6, EnemyMap.Y + 1);
                Console.WriteLine(" {0, -6} Press [Enter] to confirm the position!", GameMap.GetFieldLabel(EnemyMap.CursorX, EnemyMap.CursorY));

                Console.SetCursorPosition(EnemyMap.GetDrawingStartPoint()[0] + EnemyMap.CursorX, EnemyMap.GetDrawingStartPoint()[1] + EnemyMap.CursorY);
                cki = Console.ReadKey();

                this.globalInputHandler(cki);

                EnemyMap.GetPositionFromCursorInput(cki);
            }

            return new int[] { EnemyMap.CursorX, EnemyMap.CursorY };
        }
    }
}
