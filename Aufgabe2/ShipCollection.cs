//-----------------------------------------------------------------------
// <copyright file="ShipCollection.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents a collection of all different forms of ships.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class represents a collection of all different forms of ships.
    /// </summary>
    public class ShipCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShipCollection"/> class.
        /// </summary>
        /// <param name="amountBattleCruisers">The amount of battle cruisers.</param>
        /// <param name="amountCruisers">The amount of cruisers.</param>
        /// <param name="amountDestroyers">The amount of destroyers.</param>
        /// <param name="amountSubmarines">The amount of submarines.</param>
        public ShipCollection(int amountBattleCruisers, int amountCruisers, int amountDestroyers, int amountSubmarines)
        {
            this.Reset(amountBattleCruisers, amountCruisers, amountDestroyers, amountSubmarines);
        }

        /// <summary>
        /// Gets the amount of battle cruisers.
        /// </summary>
        /// <value>The amount of battle cruisers.</value>
        public int AmountBattleCruisers { get; private set; }

        /// <summary>
        /// Gets all saved battle cruisers.
        /// </summary>
        /// <value>An array with all saved battle cruisers.</value>
        public Ship.BattleCruiser[] BattleCruisers { get; private set; }

        /// <summary>
        /// Gets the amount of saved battle cruisers.
        /// </summary>
        /// <value>The amount of saved battle cruisers.</value>
        public int SavedBattleCruisers { get; private set; }

        /// <summary>
        /// Gets the amount of cruisers.
        /// </summary>
        /// <value>The amount of cruisers.</value>
        public int AmountCruisers { get; private set; }

        /// <summary>
        /// Gets all saved cruisers.
        /// </summary>
        /// <value>An array with all saved cruisers.</value>
        public Ship.Cruiser[] Cruisers { get; private set; }

        /// <summary>
        /// Gets the amount of saved cruisers.
        /// </summary>
        /// <value>The amount of saved cruisers.</value>
        public int SavedCruisers { get; private set; }

        /// <summary>
        /// Gets the amount of destroyers.
        /// </summary>
        /// <value>The amount of destroyers.</value>
        public int AmountDestroyers { get; private set; }

        /// <summary>
        /// Gets all saved destroyers.
        /// </summary>
        /// <value>An array with all saved destroyers.</value>
        public Ship.Destroyer[] Destroyers { get; private set; }

        /// <summary>
        /// Gets the amount of saved destroyers.
        /// </summary>
        /// <value>The amount of saved destroyers.</value>
        public int SavedDestroyers { get; private set; }

        /// <summary>
        /// Gets the amount of submarines.
        /// </summary>
        /// <value>The amount of submarines.</value>
        public int AmountSubmarines { get; private set; }

        /// <summary>
        /// Gets all saved submarines.
        /// </summary>
        /// <value>An array with all saved submarines.</value>
        public Ship.Submarine[] Submarines { get; private set; }

        /// <summary>
        /// Gets the amount of saved submarines.
        /// </summary>
        /// <value>The amount of saved submarines.</value>
        public int SavedSubmarines { get; private set; }

        /// <summary>
        /// Gets the amount of all ships.
        /// </summary>
        /// <value>The amount of all ships.</value>
        public int AmountShips { get; private set; }

        /// <summary>
        /// Gets all saved ships.
        /// </summary>
        /// <value>An array with all saved ships.</value>
        public Ship[] Ships { get; private set; }

        /// <summary>
        /// Gets the amount of all saved ships.
        /// </summary>
        /// <value>The amount of all saved ships.</value>
        public int SavedShips { get; private set; }

        /// <summary>
        /// Empties all arrays and reinitializes them with new sizes, which are given by the parameters.
        /// </summary>
        /// <param name="amountBattleCruisers">The new amount of battle cruisers.</param>
        /// <param name="amountCruisers">The new amount of cruisers.</param>
        /// <param name="amountDestroyers">The new amount of destroyers.</param>
        /// <param name="amountSubmarines">The new amount of submarines.</param>
        public void Reset(int amountBattleCruisers, int amountCruisers, int amountDestroyers, int amountSubmarines)
        {
            this.AmountBattleCruisers = amountBattleCruisers;
            this.AmountCruisers = amountCruisers;
            this.AmountDestroyers = amountDestroyers;
            this.AmountSubmarines = amountSubmarines;

            this.AmountShips = this.AmountBattleCruisers + this.AmountCruisers + this.AmountDestroyers + this.AmountSubmarines;

            this.BattleCruisers = new Ship.BattleCruiser[this.AmountBattleCruisers];
            this.Cruisers = new Ship.Cruiser[this.AmountCruisers];
            this.Destroyers = new Ship.Destroyer[this.AmountDestroyers];
            this.Submarines = new Ship.Submarine[this.AmountSubmarines];

            this.Ships = new Ship[this.AmountShips];

            this.SavedBattleCruisers = 0;
            this.SavedCruisers = 0;
            this.SavedDestroyers = 0;
            this.SavedSubmarines = 0;

            this.SavedShips = 0;
        }

        /// <summary>
        /// Empties all arrays and reinitializes them with the same sizes.
        /// </summary>
        public void Reset()
        {
            this.Reset(this.AmountBattleCruisers, this.AmountCruisers, this.AmountDestroyers, this.AmountSubmarines);
        }

        /// <summary>
        /// Adds a battle cruiser.
        /// </summary>
        /// <param name="bc">Battle cruiser, which will be added.</param>
        /// <returns>A boolean, which indicates whether the adding was successfully or not.</returns>
        public bool AddBattleCruiser(Ship.BattleCruiser bc)
        {
            if (this.SavedBattleCruisers < this.AmountBattleCruisers && this.SavedShips < this.AmountShips)
            {
                this.BattleCruisers[this.SavedBattleCruisers] = bc;
                this.SavedBattleCruisers++;

                this.Ships[this.SavedShips] = bc;
                this.SavedShips++;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a cruiser.
        /// </summary>
        /// <param name="c">Cruiser, which will be added.</param>
        /// <returns>A boolean, which indicates whether the adding was successfully or not.</returns>
        public bool AddCruiser(Ship.Cruiser c)
        {
            if (this.SavedCruisers < this.AmountCruisers)
            {
                this.Cruisers[this.SavedCruisers] = c;
                this.SavedCruisers++;

                this.Ships[this.SavedShips] = c;
                this.SavedShips++;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a destroyer.
        /// </summary>
        /// <param name="d">Destroyer, which will be added.</param>
        /// <returns>A boolean, which indicates whether the adding was successfully or not.</returns>
        public bool AddDestroyer(Ship.Destroyer d)
        {
            if (this.SavedDestroyers < this.AmountDestroyers)
            {
                this.Destroyers[this.SavedDestroyers] = d;
                this.SavedDestroyers++;

                this.Ships[this.SavedShips] = d;
                this.SavedShips++;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a submarine.
        /// </summary>
        /// <param name="s">Submarine, which will be added.</param>
        /// <returns>A boolean, which indicates whether the adding was successfully or not.</returns>
        public bool AddSubmarine(Ship.Submarine s)
        {
            if (this.SavedSubmarines < this.AmountSubmarines)
            {
                this.Submarines[this.SavedSubmarines] = s;
                this.SavedSubmarines++;

                this.Ships[this.SavedShips] = s;
                this.SavedShips++;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a ship.
        /// </summary>
        /// <param name="s">Ship, which will be added.</param>
        /// <returns>A boolean, which indicates whether the adding was successfully or not.</returns>
        public bool AddShip(Ship s)
        {
            if (s is Ship.BattleCruiser)
            {
                return this.AddBattleCruiser((Ship.BattleCruiser)s);
            }
            else if (s is Ship.Cruiser)
            {
                return this.AddCruiser((Ship.Cruiser)s);
            }
            else if (s is Ship.Destroyer)
            {
                return this.AddDestroyer((Ship.Destroyer)s);
            }
            else
            {
                return this.AddSubmarine((Ship.Submarine)s);
            }
        }
    }
}
