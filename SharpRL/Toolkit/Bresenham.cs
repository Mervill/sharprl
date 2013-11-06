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
using System.Drawing;

namespace SharpRL.Toolkit
{
    /// <summary>
    /// Static utility class for eunmerating points along a line using Bresenham's algorithm
    /// </summary>
    public static class Bresenham
    {
        /// <summary>
        /// Enumerates through each position along a line defined by the starting and ending points.
        /// </summary>
        /// <param name="start">The starting coordinate</param>
        /// <param name="end">The ending coordinate</param>
        /// <returns>Each Point along the line</returns>
        public static IEnumerable<Point> GetLine(Point start, Point end)
        {
            return GetLine(start.X, start.Y, end.X, end.Y);
        }

        /// <summary>
        /// Enumerates through each position along a line defined by the starting and ending points.
        /// </summary>
        /// <param name="x0">Starting x coordinate</param>
        /// <param name="y0">Starting y coordinate</param>
        /// <param name="x1">Ending x coordinate</param>
        /// <param name="y1">Ending y coordinate</param>
        /// <returns>Each Point along the line</returns>
        public static IEnumerable<Point> GetLine(int x0, int y0, int x1, int y1)
        {
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx, sy;

            if (x0 < x1)
                sx = 1;
            else
                sx = -1;

            if (y0 < y1)
                sy = 1;
            else
                sy = -1;

            int err = dx - dy;

            while (true)
            {
                yield return new Point(x0, y0);
                if (x0 == x1 && y0 == y1)
                    break;

                int e2 = 2 * err;

                if (e2 > -dy)
                {
                    err = err - dy;
                    x0 = x0 + sx;
                }

                if (e2 < dx)
                {
                    err = err + dx;
                    y0 = y0 + sy;
                }
            }
        }

    }
}
