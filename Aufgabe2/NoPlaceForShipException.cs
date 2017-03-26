//-----------------------------------------------------------------------
// <copyright file="NoPlaceForShipException.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>Represents an exception, which is thrown, when there is no place for another ship.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an exception, which is thrown, when there is no place for another ship.
    /// </summary>
    public class NoPlaceForShipException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoPlaceForShipException"/> class.
        /// </summary>
        public NoPlaceForShipException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoPlaceForShipException"/> class.
        /// </summary>
        /// <param name="message">The message to be passed on with the exception.</param>
        public NoPlaceForShipException(string message) : base(message)
        {
        }
    }
}
