//Copyright (c) 2012 Shane Baker
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpRL.Framework
{
    /// <summary>
    /// Field of view methods
    /// </summary>
    public enum FOVMethod
    {
        /// <summary>
        /// A simple recursive shadow casting algorithm
        /// </summary>
        RecursiveShadowcasting,
        /// <summary>
        /// Based on Mingos' Restrictive Precise Angle Shadowcasting algorithm
        /// </summary>
        MRPAS
    }

    /// <summary>
    /// The shape of the field of view limits.
    /// </summary>
    public enum RangeLimitShape
    {
        Square,
        Octagon,
        Circle
    }

    /// <summary>
    /// The FieldOfView class uses this interface to query transparency and set the visible state of cells.  A map (2-dimensional array) of
    /// objects that implement this interface is used to create a FieldOfView instance.
    /// </summary>
    public interface IFovCell
    {
        /// <summary>
        /// Should be true if the cell is transparent, false if opaque
        /// </summary>
        bool IsTransparent { get; }

        /// <summary>
        /// Should get or set the visibility state of this cell.
        /// </summary>
        bool IsVisible { get; set; }
    }

    /// <summary>
    /// Contains methods to compute a field-of-view from an Array2d of IFOVCell objects.
    /// </summary>
    /// <remarks>
    /// To use the FieldOfView class, you must first implement an IFovCell interface and create an Array2d of them
    /// to be used as the map.  A simple IFovCell implementation might looks something like this:
    /// <code>
    ///protected class Cell : IFovCell
    ///{
    ///       public Cell(bool isWall)
    ///       {
    ///            this.IsWall = isWall;
    ///            visible = false;
    ///
    ///        }
    ///
    ///        bool visible;
    ///
    ///        public bool IsWall
    ///        {
    ///            get;
    ///            set;
    ///        }
    ///
    ///        public bool IsTransparent
    ///        {
    ///            get
    ///            {
    ///               if (IsWall)
    ///                    return false;
    ///                else
    ///                    return true;
    ///            }
    ///        }
    ///
    ///        public bool IsVisible
    ///        {
    ///            get
    ///            {
    ///                return visible;
    ///            }
    ///            set
    ///            {
    ///                visible = value;
    ///            }
    ///        }
    ///
    ///}
    /// </code>
    /// <para/>FieldOfView uses an array of these cells to query if a cells is transparent, and to set the visibility state
    /// of each cell accordingly.<para/>
    /// Look at ExampleBase.cs and Example4.cs in the Examples project for examples of use.
    /// </remarks>
    public class FieldOfView<TFovCell> where TFovCell : IFovCell
    {
        /// <summary>
        /// Construct a FieldOfView instance given a amp of IFovCell objects
        /// </summary>
        /// <param name="inGrid"></param>
        public FieldOfView(Array2d<TFovCell> inGrid)
        {
            grid = inGrid;
        }

        /// <summary>
        /// Computes the field of view using the specified method.  This will modify the visible state of each cell
        /// using the IFovCell.IsVisible setter.
        /// </summary>
        /// <param name="viewpointX">The point of view X coordinate (e.g. player position)</param>
        /// <param name="viewpointY">The point of view Y coordinate (e.g. player position)</param>
        /// <param name="maxRange">The maximum range of the field of view.</param>
        /// <param name="lightWalls">True if walls within the field-of-view should be set as visible</param>
        /// <param name="method">The method to be used to compute the field of view</param>
        /// <param name="rangeShape">The shape of the boundary between cells within maxRange and those outside maxRange</param>
        public void ComputeFov(int viewpointX, int viewpointY, int maxRange, bool lightWalls, FOVMethod method, RangeLimitShape rangeShape = RangeLimitShape.Circle)
        {
            ClearFov();
            switch (method)
            {
                case FOVMethod.MRPAS:
                    if (mrps == null)
                        mrps = new MRPAS<TFovCell>(this);
                    mrps.ComputeFOV(viewpointX, viewpointY, maxRange, lightWalls, rangeShape);
                    break;

                case FOVMethod.RecursiveShadowcasting:
                    if (shadowCaster == null)
                        shadowCaster = new ShadowCasting<TFovCell>(this);
                    shadowCaster.ComputeFOV(viewpointX, viewpointY, maxRange, lightWalls, rangeShape);
                    break;
            }
        }

        protected void ClearFov()
        {
            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    grid[x, y].IsVisible = false;
                }
            }
        }

        ShadowCasting<TFovCell> shadowCaster;
        MRPAS<TFovCell> mrps;

        internal Array2d<TFovCell> grid;
    }
}
