//-----------------------------------------------------------------------
// <copyright file="Player.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>All derived classes are able to play a match of the game battle ship.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// All derived classes are able to play a match of the game battle ship.
    /// </summary>
    public abstract class Player
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="name">Name of the player.</param>
        /// <param name="map">Map of the player.</param>
        /// <param name="enemyMap">Hostile map of the player.</param>
        public Player(string name, GameMap map, GameMap enemyMap)
        {
            this.Name = name;
            this.Map = map;
            this.EnemyMap = enemyMap;

            this.Map.OnGameMapLost += this.Map_OnGameMapLost;
            this.EnemyMap.OnGameMapLost += this.EnemyMap_OnGameMapLost;

            this.EnemyMap.OnGameMapHit += this.EnemyMap_OnGameMapHit;
            this.EnemyMap.OnGameMapMissed += this.EnemyMap_OnGameMapMissed;
            this.EnemyMap.OnGameMapShipDestroyed += this.EnemyMap_OnGameMapShipDestroyed;

            this.RecessedShips = new ShipCollection(this.Map.AmountBattleCruisers, this.Map.AmountCruisers, this.Map.AmountDestroyers, this.Map.AmountSubmarines);

            this.Reset();
        }

        /// <summary>
        /// Delegate for event OnPlayerHit.
        /// </summary>
        /// <param name="player">The player, who hit.</param>
        /// <param name="x">X - coordinate of the hit square.</param>
        /// <param name="y">Y - coordinate of the hit square.</param>
        public delegate void PlayerHit(Player player, int x, int y);

        /// <summary>
        /// Delegate for event OnPlayerMissed.
        /// </summary>
        /// <param name="player">The player, who missed.</param>
        /// <param name="x">X - coordinate of the missed square.</param>
        /// <param name="y">Y - coordinate of the missed square.</param>
        public delegate void PlayerMissed(Player player, int x, int y);

        /// <summary>
        /// Delegate for event OnPlayerDestroyedShip.
        /// </summary>
        /// <param name="player">The player, who has destroyed a ship..</param>
        /// <param name="s">Ship, which has been destroyed.</param>
        public delegate void PlayerDestroyedShip(Player player, Ship s);

        /// <summary>
        /// Delegate for event OnPlayerLost.
        /// </summary>
        /// <param name="player">The player, who has lost.</param>
        public delegate void PlayerLost(Player player);

        /// <summary>
        /// Delegate for event OnPlayerWon.
        /// </summary>
        /// <param name="player">The player, who has won.</param>
        public delegate void PlayerWon(Player player);

        /// <summary> Is called when the player hit a ship. </summary>
        public event PlayerHit OnPlayerHit;

        /// <summary> Is called when the player missed a ship. </summary>
        public event PlayerMissed OnPlayerMissed;

        /// <summary> Is called when the player destroyed a ship. </summary>
        public event PlayerDestroyedShip OnPlayerDestroyedShip;

        /// <summary> Is called when all ships of the player have been destroyed. </summary>
        public event PlayerLost OnPlayerLost;

        /// <summary> Is called when all ships of the hostile map have been destroyed. </summary>
        public event PlayerWon OnPlayerWon;

        /// <summary>
        /// Gets the name of the player.
        /// </summary>
        /// <value> The name of the player. </value>
        public string Name { get; private set; } 

        /// <summary>
        /// Gets the map of the player.
        /// </summary>
        /// <value> The map of the player. </value>
        public GameMap Map { get; private set; }

        /// <summary>
        /// Gets the hostile map of the player.
        /// </summary>
        /// <value> The hostile map of the player. </value>
        public GameMap EnemyMap { get; private set; }

        /// <summary>
        /// Gets or sets a collection of all ships, which were destroyed by the player.
        /// </summary>
        /// <value> A collection of all ships, which were destroyed by the player. </value>
        protected ShipCollection RecessedShips { get; set; }

        /// <summary>
        /// Gets or sets the last coordinates, which were a hit.
        /// </summary>
        /// <value> The last coordinates, which were a hit.</value>
        protected int[] LastHit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player is currently playing or not.
        /// </summary>
        /// <value> A value indicating whether the player is currently playing or not. </value>
        protected bool IsPlaying { get; set; }

        /// <summary>
        /// Resets the player, which is required before a new game.
        /// </summary>
        public virtual void Reset()
        {
            this.RecessedShips.Reset(this.Map.AmountBattleCruisers, this.Map.AmountCruisers, this.Map.AmountDestroyers, this.Map.AmountSubmarines);

            this.LastHit = null;
            this.IsPlaying = false;
        }

        /// <summary>
        /// Tells the player the last coordinates, which were successful.
        /// </summary>
        /// <param name="x">X - coordinate of the last hit.</param>
        /// <param name="y">Y - coordinate of the last hit.</param>
        public virtual void GotHit(int x, int y)
        {
            this.LastHit = new int[] { x, y };
        }

        /// <summary>
        /// Tells the player the last ship he destroyed.
        /// </summary>
        /// <param name="s">Ship, which got destroyed.</param>
        public virtual void ShipRecessed(Ship s)
        {
            this.LastHit = null;
            this.RecessedShips.AddShip(s);
        }

        /// <summary>
        /// Informs the player that he is currently playing. 
        /// </summary>
        public virtual void StartGame()
        {
            this.IsPlaying = true;
        }

        /// <summary>
        /// Gets the coordinates on the hostile map, where the player assumes a ship.
        /// </summary>
        /// <returns> An integer array, which contains the coordinates. </returns>
        public abstract int[] Move();

        /// <summary>
        /// Gets called when all ships of the hostile map have been destroyed.
        /// </summary>
        private void EnemyMap_OnGameMapLost()
        {
            if (this.OnPlayerWon != null)
            {
                this.OnPlayerWon(this);
            }
        }

        /// <summary>
        /// Gets called when all ships of the player's map have been destroyed.
        /// </summary>
        private void Map_OnGameMapLost()
        {
            if (this.OnPlayerLost != null)
            {
                this.OnPlayerLost(this);
            }
        }

        /// <summary>
        /// Gets called when the player hit a ship.
        /// </summary>
        /// <param name="x">X - coordinate of the hit square.</param>
        /// <param name="y">Y - coordinate of the hit square.</param>
        private void EnemyMap_OnGameMapHit(int x, int y)
        {
            if (this.OnPlayerHit != null)
            {
                this.OnPlayerHit(this, x, y);
            }
        }

        /// <summary>
        /// Gets called when the player missed a ship.
        /// </summary>
        /// <param name="x">X - coordinate of the missed square.</param>
        /// <param name="y">Y - coordinate of the missed square.</param>
        private void EnemyMap_OnGameMapMissed(int x, int y)
        {
            if (this.OnPlayerMissed != null)
            {
                this.OnPlayerMissed(this, x, y);
            }
        }

        /// <summary>
        /// Gets called when the player destroyed a ship.
        /// </summary>
        /// <param name="s">Ship, which has been destroyed.</param>
        private void EnemyMap_OnGameMapShipDestroyed(Ship s)
        {
            if (this.OnPlayerDestroyedShip != null)
            {
                this.OnPlayerDestroyedShip(this, s);
            }
        }
    }
}
