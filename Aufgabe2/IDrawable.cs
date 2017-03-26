//-----------------------------------------------------------------------
// <copyright file="IDrawable.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>Classes, which implement this interface, can be drawn on the console.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Classes, which implement this interface, can be drawn on the console.
    /// </summary>
    public interface IDrawable
    {
        /// <summary>
        /// Method to display a class on the console.
        /// </summary>
        void Draw();
    }
}
