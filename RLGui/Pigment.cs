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

using SharpRL;
using System.Drawing;

namespace RLGui
{

    /// <summary>
    /// Stores forground and background colors in a convenient
    /// single immutable data type
    /// </summary>
    public class Pigment
    {
        /// <summary>
        /// Construct a Pigment given foreground and background colors and background flag
        /// </summary>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        public Pigment(Color foreground, Color background)
        {
            fgColor = foreground;
            bgColor = background;
        }

        /// <summary>
        /// Get the foreground color
        /// </summary>
        public Color Foreground
        {
            get { return fgColor; }
        }

        /// <summary>
        /// Get the background color
        /// </summary>
        public Color Background
        {
            get { return bgColor; }
        }

        /// <summary>
        /// Swaps a Pigments's foreground and background.  Returns a new Pigment instance,
        /// this instance is unchanged.
        /// </summary>
        /// <returns></returns>
        public Pigment Invert()
        {
            return new Pigment(Background, Foreground);
        }

        /// <summary>
        /// Returns a new Pigment by replacing the foreground color.  This isntance remains
        /// unchanged.
        /// </summary>
        /// <param name="newFGColor"></param>
        /// <returns></returns>
        public Pigment ReplaceForeground(Color newFGColor)
        {
            return new Pigment(newFGColor, Background);
        }

        /// <summary>
        /// Returns a new Pigment by replacing the background color.  This isntance remains
        /// unchanged.
        /// </summary>
        /// <param name="newBGColor"></param>
        /// <returns></returns>
        public Pigment ReplaceBackground(Color newBGColor)
        {
            return new Pigment(Foreground, newBGColor);
        }

        /// <summary>
        /// Returns a string representation of the Pigment
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0},{1}", Foreground.ToString(), Background.ToString());
        }

        private readonly Color fgColor;
        private readonly Color bgColor;


        public static Pigment WhiteBlack { get; private set; }
        public static Pigment BlackWhite { get; private set; }

        static Pigment()
        {
            WhiteBlack = new Pigment(Color.White, Color.Black);
            BlackWhite = new Pigment(Color.Black, Color.White);
        }
    }

}
