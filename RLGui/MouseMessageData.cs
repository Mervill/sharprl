//Copyright (c) 2013 Shane Baker
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
using RLGui;
using SharpRL;
using System.Drawing;

namespace RLGui
{


    /// <summary>
    /// Contains mouse input information for messages sent to components
    /// </summary>
    public class MouseMessageData : MouseEventData
    {
        /// <summary>
        /// Construct a MouseMessageData object
        /// </summary>
        /// <param name="consolePosition"></param>
        /// <param name="pixelPosition"></param>
        /// <param name="button"></param>
        /// <param name="localPosition"></param>
        public MouseMessageData(Point consolePosition, Point pixelPosition, MouseButton button, Point localPosition)
            :base(consolePosition, pixelPosition, button)
        {
            this.LocalPos = localPosition;
        }

        /// <summary>
        /// Construct a MouseMessageData using an existing MouseEventData object
        /// </summary>
        /// <param name="mouseInfo"></param>
        /// <param name="localPos"></param>
        public MouseMessageData(MouseEventData mouseInfo, Point localPos)
            : base(mouseInfo.ConsoleLocation, mouseInfo.PixelPosition, mouseInfo.Button)
        {
            this.LocalPos = localPos;
        }

        /// <summary>
        /// The mouse position that has been translated to the local space system of the receiving
        /// component
        /// </summary>
        public Point LocalPos { get; private set; }
    }
	
	
	
	
	
	
}
