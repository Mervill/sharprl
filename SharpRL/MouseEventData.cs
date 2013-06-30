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
using System.Drawing;

namespace SharpRL
{


    /// <summary>
    /// Holds information about a mouse input event
    /// </summary>
    public class MouseEventData
    {
        /// <summary>
        /// Construct a MouseEventData object
        /// </summary>
        /// <param name="consolePos"></param>
        /// <param name="pixelPos"></param>
        /// <param name="button"></param>
        public MouseEventData(Point consolePos, Point pixelPos, MouseButton button)
        {
            this.pixelLocation = pixelPos;
            this.consoleLocation = consolePos;
            this.Button = button;
        }

        /// <summary>
        /// Construct a MouseEventData object
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="px"></param>
        /// <param name="py"></param>
        /// <param name="button"></param>
        public MouseEventData(int cx, int cy, int px, int py, MouseButton button)
            :this(new Point(cx,cy), new Point(px,py), button)
        {
        }

        private Point pixelLocation;
        private Point consoleLocation;

        /// <summary>
        /// The pixel coordinate of the mouse pointer
        /// </summary>
        public Point PixelPosition
        {
            get
            {
                return pixelLocation;
            }
            private set
            {
                pixelLocation = value;
            }
        }

        /// <summary>
        /// The console (character) coordinate of the mouse pointer
        /// </summary>
        public Point ConsoleLocation
        {
            get
            {
                return consoleLocation;
            }
            private set
            {
                consoleLocation = value;
            }
        }

        /// <summary>
        /// The pixel X coordinate of the mouse pointer
        /// </summary>
        public int PX
        {
            get { return pixelLocation.X; }
        }

        /// <summary>
        /// The pixel Y coordinate of the mouse pointer
        /// </summary>
        public int PY
        {
            get { return pixelLocation.Y; }
        }

        /// <summary>
        /// The console (character) X coordinate of the mouse pointer
        /// </summary>
        public int CX
        {
            get { return consoleLocation.X; }
        }

        /// <summary>
        /// The console (character) Y coordinate of the mouse pointer
        /// </summary>
        public int CY
        {
            get { return consoleLocation.Y; }
        }

        /// <summary>
        /// Which mouse button is involved in the event
        /// </summary>
        public MouseButton Button { get; private set; }
    }
}
