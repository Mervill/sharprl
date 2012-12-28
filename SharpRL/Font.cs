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

namespace SharpRL
{
    #region Enums

    /// <summary>
    /// The type of layout of the font
    /// </summary>
    public enum FontLayout
    {
        /// <summary>
        /// Increasing codes from left to right
        /// </summary>
        InRow,

        /// <summary>
        /// Increasing codes from top to bottom
        /// </summary>
        InColumn,
    }

    /// <summary>
    /// Determines how the characters will be blended when drawn on the screen
    /// </summary>
    public enum FontFormat
    {
        /// <summary>
        /// A key color is determined by the pixel at position 0,0.  All pixels with that color
        /// are set as fully transparent, all other pixels fully opaque
        /// </summary>
        NoAA,

        /// <summary>
        /// Transparency determined by the alpha channel of the image.
        /// </summary>
        AlphaAA,

        /// <summary>
        /// Transparency is determined by the level of the red component.
        /// </summary>
        GreyscaleAA
    }

    /// <summary>
    /// Special characters used by the default fonts
    /// </summary>
    public enum SpecialChar : byte
    {
#pragma warning disable 1591
        HorizontalLine = 196,
        VerticalLine = 179,
        NorthEastLine = 191,
        NorthWestLine = 218,
        SouthEastLine = 217,
        SouthWestLine = 192,

        DoubleHorzLine = 205,
        DoubleVertLine = 186,
        DoubleNorthEast = 187,
        DoubleNorthWest = 201,
        DoubleSouthEast = 188,
        DoubleSouthWest = 200,

        TeeWest = 180,
        TeeEast = 195,
        TeeNorth = 193,
        TeeSouth = 194,

        DoubleTeeWest = 185,
        DoubleTeeEast = 204,
        DoubleTeeNorth = 202,
        DoubleTeeSouth = 203,

        CrossLines = 197,
        DoubleCrossLines = 206,

        Block1 = 176,
        Block2 = 177,
        Block3 = 178,

        ArrowNorth = 24,
        ArrowSouth = 25,
        ArrowEast = 26,
        ArrowWest = 27,

        ArrowNorthNoTail = 30,
        ArrowSouthNoTail = 31,
        ArrowEastNoTail = 16,
        ArrowWestNoTail = 17,

        DoubleArrowHorz = 29,
        DoubleArrowVert = 18,

        CheckBoxUnset = 224,
        CheckBoxSet = 225,
        RadioUnset = 9,
        RadioSet = 10,

        SubpixelNorthWest = 226,
        SubpixelNorthEast = 227,
        SubpixelNorth = 228,
        SubpixelSouthEast = 229,
        SubpixelDiagonal = 230,
        SubpixelEast = 231,
        SubpixelSouthWest = 232,

        Smilie = 1,
        SmilieInverse = 2,
        Heart = 3,
        Diamond = 4,
        Club = 5,
        Spade = 6,
        Bullet = 7,
        BulletInverse = 8,
        Male = 11,
        Female = 12,
        Note = 13,
        NoteDouble = 14,
        Light = 15,
        ExclamationDouble = 19,
        Pilcrow = 20,
        Section = 21,
        Pound = 156,
        Multiplication = 158,
        Function = 159,
        Reserved = 169,
        Half = 171,
        OneQuarter = 172,
        Copyright = 184,
        Cent = 189,
        Yen = 190,
        Currency = 207,
        ThreeQuarters = 243,
        Division = 246,
        Umlaut = 249,
        Power1 = 251,
        Power3 = 252,
        Power2 = 253,
        BulletSquare = 254
#pragma warning restore 1591
    }

    #endregion

    /// <summary>
    /// Represents a set of font glyphs, and provides a mapping from an ASCII code to each glyph
    /// </summary>
    public class FontSheet
    {
        #region Properties

        /// <summary>
        /// The width of each character in pixels, before any scaling done by the console.
        /// </summary>
        public int CharacterWidth { get; private set; }

        /// <summary>
        /// The height of each character in pixels, before any scaling done by the console.
        /// </summary>
        public int CharacterHeight { get; private set; }

        private int handle;
        /// <summary>
        /// Get the backend handle associated with the font image
        /// </summary>
        public int Handle { get { return handle; } }

        #endregion


        #region CTOR

        /// <summary>
        /// Create a Font given the path to an image file
        /// </summary>
        /// <param name="gameConsole"></param>
        /// <param name="fileName"></param>
        /// <param name="format"></param>
        /// <param name="layout"></param>
        public FontSheet(GameConsole gameConsole, string fileName, FontFormat format, FontLayout layout)
        {
            if (gameConsole == null)
                throw new ArgumentNullException("gameConsole");

            using (Bitmap bitmap = new Bitmap(fileName))
            {
                Initialize(gameConsole, bitmap, format, layout);
            }
        }

        /// <summary>
        /// Creat a Font object from a System.Drawing.Bitmap
        /// </summary>
        /// <param name="gameConsole"></param>
        /// <param name="bitmap">The provided bitmap is copied, so it can be disposed after calling this method</param>
        /// <param name="format"></param>
        /// <param name="layout"></param>
        public FontSheet(GameConsole gameConsole, Bitmap bitmap, FontFormat format, FontLayout layout)
        {
            if (gameConsole == null)
                throw new ArgumentNullException("gameConsole");
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            Initialize(gameConsole, bitmap, format, layout);
        }

        private void Initialize(GameConsole gameConsle, Bitmap bitmap, FontFormat format, FontLayout layout)
        {
            if (layout == FontLayout.InColumn || layout == FontLayout.InRow)
            {
                numberOfColumns = 16;
                numberOfRows = 16;
            }

            CharacterWidth = bitmap.Width / numberOfColumns;
            CharacterHeight = bitmap.Height / numberOfRows;

            charMap = new Point[numberOfRows * numberOfColumns];
            MapConsecutiveAsciiCodes((char)0, numberOfRows * numberOfColumns, 0, 0, layout);

            this.image = ProcessImage(bitmap, format);



            gameConsle.Renderer.RegisterImageResource(image, out handle);
        }

        #endregion


        #region Mapping Methods

        /// <summary>
        /// Set a custom mapping for a single ascii code.
        /// </summary>
        /// <param name="asciiCode"></param>
        /// <param name="fontCharX">The X coordinate of the character sprite in characters (not pixels)</param>
        /// <param name="fontCharY">The Y coordinate of the character sprite in characters (not pixels)</param>
        public void MapAsciiCode(char asciiCode, int fontCharX, int fontCharY)
        {
            if (fontCharX < 0 || fontCharX >= numberOfColumns)
                throw new ArgumentOutOfRangeException("fontCharX");
            if (fontCharY < 0 || fontCharY >= numberOfRows)
                throw new ArgumentOutOfRangeException("fontCharY");

            charMap[asciiCode].X = fontCharX;
            charMap[asciiCode].Y = fontCharY;
        }

        /// <summary>
        /// Set a series of consecutive mappings.
        /// </summary>
        /// <param name="firstAsciiCode">The starting ascii code to map</param>
        /// <param name="numberOfCodes">Number of consecutive codes to map</param>
        /// <param name="startFontCharX">The starting X coordinate of the sprite in characters (not pixels)</param>
        /// <param name="startFontCharY">The starting Y coordinate of the sprite in characters (not pixels)</param>
        /// <param name="layout"></param>
        public void MapConsecutiveAsciiCodes(char firstAsciiCode, int numberOfCodes, int startFontCharX,
            int startFontCharY, FontLayout layout)
        {
            if (startFontCharX < 0 || startFontCharX >= numberOfColumns)
                throw new ArgumentOutOfRangeException("fontCharX");
            if (startFontCharY < 0 || startFontCharY >= numberOfRows)
                throw new ArgumentOutOfRangeException("fontCharY");

            int x = startFontCharX;
            int y = startFontCharY;

            for (int i = 0; i < numberOfCodes; i++)
            {
                MapAsciiCode((char)(firstAsciiCode + i), x, y);

                if (layout == FontLayout.InColumn)
                {
                    y++;
                    if (y >= numberOfRows)
                    {
                        y = 0;
                        x++;
                        if (x >= numberOfColumns)
                            return;
                    }
                }
                else
                {
                    x++;
                    if (x >= numberOfColumns)
                    {
                        x = 0;
                        y++;
                        if (y >= numberOfRows)
                            return;
                    }
                }
            }

        }

        /// <summary>
        /// Maps ascii codes in provided string to consecutive character sprites.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startFontCharX">The starting X coordinate of the sprite in characters (not pixels)</param>
        /// <param name="startFontCharY">The starting Y coordinate of the sprite in characters (not pixels)</param>
        /// <param name="layout"></param>
        public void MapString(string str, int startFontCharX, int startFontCharY, FontLayout layout)
        {
            if (startFontCharX < 0 || startFontCharX >= numberOfColumns)
                throw new ArgumentOutOfRangeException("fontCharX");
            if (startFontCharY < 0 || startFontCharY >= numberOfRows)
                throw new ArgumentOutOfRangeException("fontCharY");

            int x = startFontCharX;
            int y = startFontCharY;

            for (int i = 0; i < str.Length; i++)
            {
                MapAsciiCode(str[i], x, y);

                if (layout == FontLayout.InColumn)
                {
                    y++;
                    if (y >= numberOfRows)
                    {
                        y = 0;
                        x++;
                        if (x >= numberOfColumns)
                            return;
                    }
                }
                else
                {
                    x++;
                    if (x >= numberOfColumns)
                    {
                        x = 0;
                        y++;
                        if (y >= numberOfRows)
                            return;
                    }
                }
            }
        }

        #endregion


        #region Private

        internal Point GetCharPos(char c)
        {
            return charMap[c];
        }
        internal int numberOfColumns;
        internal int numberOfRows;
        internal Bitmap image;
        
        Point[] charMap;


        private Bitmap ProcessImage(Bitmap bitmap, FontFormat type)
        {
            if (type == FontFormat.AlphaAA)
            {
                if (bitmap.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                    throw new ArgumentException("Image must have an alpha channel to use AlphaAA font type");

                return new Bitmap(bitmap);
            }

            Bitmap img2 = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Color key = bitmap.GetPixel(0, 0);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    Color dest;

                    if (type == FontFormat.NoAA)
                    {
                        if (c == key)
                        {
                            dest = Color.FromArgb(0, 0, 0, 0);
                        }
                        else
                        {
                            dest = Color.FromArgb(255, 255, 255, 255);
                        }
                    }
                    else
                    {
                        dest = Color.FromArgb(c.R, c.R, c.R, c.R);
                    }

                    img2.SetPixel(x, y, dest);
                }
            }

            return img2;
        }

        #endregion

    }
}
