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

namespace SharpRL
{
    /// <summary>
    /// A simple wrapper that flattens a 2-dimensional array to increase performance.
    /// </summary>
    /// <remarks>
    /// Multi-dimensional arrays are expensive to iterate through in C#, so this is a simple
    /// class to flatten 2D arrays into a 1D array.  Because an Array2d is not a true array type, 
    /// storing structs or other value types here may not have the behavior you expect.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class Array2d<T>
    {
        T[] data;


        /// <summary>
        /// Construct an Array2d with specified dimension sizes
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Array2d(int width, int height)
        {
            Width = width;
            Height = height;

            data = new T[width * height];
        }

        /// <summary>
        /// The size of the "X" dimension
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The size of the "Y" dimensions
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Get the element at the specified x, y position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>The element at that position</returns>
        public T this[int x, int y]
        {
            get
            {
                return data[x + y * Width];
            }
            set
            {
                data[x + y * Width] = value;
            }
        }
    }
}
