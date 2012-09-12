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
using System.Drawing;

namespace SharpRL
{
    /// <summary>
    /// Represents a texture atlas of regular-sized images for use as tiles or sprites
    /// </summary>
    public class TileSheet
    {

        #region Properties

        /// <summary>
        /// The size of each tile in pixels
        /// </summary>
        public Size TileSize { get; private set; }

        /// <summary>
        ///  The width of each tile in pixels
        /// </summary>
        public int TileWidth { get { return TileSize.Width; } }

        /// <summary>
        /// The height of each tile in pixels
        /// </summary>
        public int TileHeight { get { return TileSize.Height; } }

        private int handle;
        /// <summary>
        /// The OpenGL handle of the internally stored texture assigned by the backend
        /// </summary>
        public int Handle { get { return handle; } }

        #endregion


        #region CTOR

        /// <summary>
        /// Create a TileSheet given a valid GameConsole, path to the source bitmap image, and size of the
        /// tiles in pixels
        /// </summary>
        /// <param name="gameConsole"></param>
        /// <param name="fileName"></param>
        /// <param name="tileWidth"></param>
        /// <param name="tileHeight"></param>
        public TileSheet(GameConsole gameConsole, string fileName, int tileWidth, int tileHeight)
        {
            if (gameConsole == null)
                throw new ArgumentNullException("gameConsole");

            using (Bitmap bitmap = new Bitmap(fileName))
            {
                Initialize(gameConsole, bitmap, tileWidth, tileHeight);
            }
        }

        /// <summary>
        /// Create a TileSheet given a valid GameConsole, a System.Drawing.Bitmap, and size of the
        /// tiles in pixels
        /// </summary>
        /// <param name="gameConsole"></param>
        /// <param name="bitmap"></param>
        /// <param name="tileSize"></param>
        public TileSheet(GameConsole gameConsole, Bitmap bitmap, int tileWidth, int tileHeight)
        {
            Initialize(gameConsole, bitmap, tileWidth, tileHeight);
        }

        private void Initialize(GameConsole gameConsole, Bitmap bitmap, int tileWidth, int tileHeight)
        {
            this.image = new Bitmap(bitmap);

            gameConsole.Renderer.RegisterImageResource(image, out handle);

            numberOfColumns = image.Width / tileWidth;
            numberOfRows = image.Height / tileHeight;

            TileSize = new Size(tileWidth, tileHeight);
        }

        #endregion


        #region Private Fields

        internal int numberOfColumns;
        internal int numberOfRows;
        internal Bitmap image;

        #endregion
    }
}
