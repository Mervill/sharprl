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
using SharpRL.Framework;

namespace SharpRL
{

    #region Enums

    /// <summary>
    /// Vertical alignment used by various string printing methods
    /// </summary>
    public enum VerticalAlignment
    {
        /// <summary>
        /// String will be aligned to the top
        /// </summary>
        Top,
        /// <summary>
        /// String will be centered between the top and bottom
        /// </summary>
        Center,
        /// <summary>
        /// String will be aligned to the bottom
        /// </summary>
        Bottom
    }

    /// <summary>
    /// Horizontal alignment used by various string printing methods
    /// </summary>
    public enum HorizontalAlignment
    {
        /// <summary>
        /// String will be aligned to the left
        /// </summary>
        Left,
        /// <summary>
        /// String will be centered between the right and the left
        /// </summary>
        Center,
        /// <summary>
        /// String will be aligned to the right
        /// </summary>
        Right
    }

    /// <summary>
    /// Wrapping mode used by various string printing methods
    /// </summary>
    public enum WrappingType
    {
        /// <summary>
        /// No wrapping is performed - characters will be trimmed if too long to fit
        /// </summary>
        None,
        /// <summary>
        /// String is wrapped to new line if too long to fit
        /// </summary>
        Character,
        /// <summary>
        /// String is wrapped to new line if too long to fit, respecting word boundaries (spaces)
        /// </summary>
        Word
    }

    [Flags]
    enum BlitFlags
    {
        UseChar = 1,
        UseFG = 2,
        UseBG = 4,
        UseAll = UseChar | UseFG | UseBG
    }

    #endregion

    /// <summary>
    /// Contains values to use with BlitAlpha.
    /// </summary>
    public struct AlphaMode
    {
        /// <summary>
        /// Determines the foreground color alpha. A value of 0.0 (minimum) would use the destination surface
        /// foreground color.  A value of 1.0 (maximum) would use the source surface foreground color. If the 
        /// source cell is a blank (space), and if CharBleed is not 1.0, then CharBleed is used to determine
        /// the final foreground color instead.
        /// </summary>
        public float FGColorAlpha { get; set; }

        /// <summary>
        /// Determines the background color alpha. A value of 0.0 (minimum) would use the destination surface
        /// background color.  A value of 1.0 (maximum) would use the source surface background color.
        /// </summary>
        public float BGColorAlpha { get; set; }

        /// <summary>
        /// Determines how to blend when the source cell is a blank (space) character. If this is set to 1.0f, 
        /// the destination character will not show through, and FGColorAlpha will be used to determine the foreground
        /// color. Other values for this property behave similar to FGColorAlpha.
        /// </summary>
        public float CharBleed { get; set; }

        /// <summary>
        /// Constructs an AlphaMode instance using the provided value for BGColorAlpha and CharBleed. FGColorAlpha
        /// is set to 1. This method is here to retain backward compatibility when the alpha could only be set
        /// by a single value.
        /// </summary>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static AlphaMode BasicAlpha(float alpha)
        {
            return new AlphaMode() { FGColorAlpha = 1f, BGColorAlpha = alpha, CharBleed = alpha };
        }
    }

    /// <summary>
    /// Base class for Surface objects, exposes various drawing operations
    /// </summary>
    public abstract class Surface
    {
        /// <summary>
        /// Get the size of the surface in characters (number of columns and rows)
        /// </summary>
        public Size Size { get; private set; }

        /// <summary>
        /// Get the width of the surface in characters (number of columns)
        /// </summary>
        public int Width { get { return Size.Width; } }

        /// <summary>
        /// Get the height of the surface in characters (number of rows)
        /// </summary>
        public int Height { get { return Size.Height; } }

        /// <summary>
        /// Shortcut to get the Rectangle bounds of this surface, which
        /// equals new Rectangle(Point.Empty, this.Size)
        /// </summary>
        public Rectangle Rect
        {
            get { return new Rectangle(Point.Empty, Size); }
        }

        /// <summary>
        /// Get or set the default background color.
        /// </summary>
        public Color DefaultBackground { get; set; }

        /// <summary>
        /// Get or set the default foreground color
        /// </summary>
        public Color DefaultForeground { get; set; }

        /// <summary>
        /// Get or set the default character.
        /// </summary>
        public Char DefaultChar { get; set; }

        /// <summary>
        /// Create an offscreen surface of the given size
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        protected Surface(int columns, int rows)
        {
            if (columns <= 0 || rows <= 0)
                throw new ArgumentException("Width and height must be greater than 0");

            Size = new Size(columns, rows);
            
            DefaultBackground = Color.Black;
            DefaultForeground = Color.White;
            DefaultChar = ' ';
        }

        /// <summary>
        /// Get the foreground color at the specified coordinate.  An exception will be thrown
        /// if the coordinate is outside the surface boundaries.
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <returns></returns>
        public Color GetForeground(int cx, int cy)
        {
            if (cx < 0 || cx >= Width || cy < 0 || cy >= Height)
                throw new ArgumentException("Coordinate is out of bounds");

            return GetCellUnchecked(cx, cy).fgColor;
        }

        /// <summary>
        /// Get the background color at the specified coordinate.  An exception will be thrown
        /// if the coordinate is outside the surface boundaries.
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <returns></returns>
        public Color GetBackground(int cx, int cy)
        {
            if (cx < 0 || cx >= Width || cy < 0 || cy >= Height)
                throw new ArgumentException("Coordinate is out of bounds");

            return GetCellUnchecked(cx, cy).bgColor;
        }

        /// <summary>
        /// Get the character at the specified coordinate.  An exception will be thrown
        /// if the coordinate is outside the surface boundaries.
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <returns></returns>
        public char GetChar(int cx, int cy)
        {
            if (cx < 0 || cx >= Width || cy < 0 || cy >= Height)
                throw new ArgumentException("Coordinate is out of bounds");

            return GetCellUnchecked(cx, cy).ch;
        }

        #region Set Cell Methods

        /// <summary>
        /// Set the cell foreground color at the specified position.  Cells outside of the surface
        /// boundaries are ignored.
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="fg"></param>
        public void SetForeground(int cx, int cy, Color fg)
        {
            SetCell(cx, cy, null, fg, null);
        }

        /// <summary>
        /// Set the cell background color at the specified position.  Cells outside of the surface
        /// boundaries are ignored.
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="bg"></param>
        public void SetBackground(int cx, int cy, Color bg)
        {
            SetCell(cx, cy, null, null, bg);
        }

        /// <summary>
        /// Set the cell character at the specified position.  Cells outside of the surface
        /// boundaries are ignored.
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="ch"></param>
        public void SetCharacter(int cx, int cy, char ch)
        {
            SetCell(cx, cy, ch, null, null);
        }

        #endregion

        #region Character Printing

        /// <summary>
        /// Print a character at the specified position using the specified foreground and background colors.
        /// Printing outside the surface boundaries is ignored.
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="ch"></param>
        /// <param name="fg"></param>
        /// <param name="bg"></param>
        public void PrintChar(int cx, int cy, char ch, Color fg, Color bg)
        {
            SetCell(cx, cy, ch, fg, bg);
        }

        /// <summary>
        /// Print a character at the specified position using the specified foreground and default background colors.
        /// Printing outside the surface boundaries is ignored.
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="ch"></param>
        /// <param name="fg"></param>
        public void PrintChar(int cx, int cy, char ch, Color fg)
        {
            PrintChar(cx, cy, ch, fg, DefaultBackground);
        }

        /// <summary>
        /// Print a character at the specified position using the default foreground and background colors.
        /// Printing outside the surface boundaries is ignored.
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="ch"></param>
        public void PrintChar(int cx, int cy, char ch)
        {
            PrintChar(cx, cy, ch, DefaultForeground, DefaultBackground);
        }

        #endregion

        #region String Printing

        /// <summary>
        /// Print a string at the specified position using the specified foreground and background colors.
        /// Any characters falling outside the surface boundaries are clipped.
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="str"></param>
        /// <param name="fg"></param>
        /// <param name="bg"></param>
        public void PrintString(int cx, int cy, string str, Color fg, Color bg)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            for (int i = 0; i < str.Length; i++)
            {
                PrintChar(cx + i, cy, str[i], fg, bg);
            }
        }

        /// <summary>
        /// Print a string at the specified position using the specified foreground and default background colors.
        /// Any characters falling outside the surface boundaries are clipped.
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="str"></param>
        /// <param name="fg"></param>
        public void PrintString(int cx, int cy, string str, Color fg)
        {
            PrintString(cx, cy, str, fg, DefaultBackground);
        }

        /// <summary>
        /// Print a string at the specified position using the default foreground and background colors.
        /// Any characters falling outside the surface boundaries are clipped.
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="str"></param>
        public void PrintString(int cx, int cy, string str)
        {
            PrintString(cx, cy, str, DefaultForeground, DefaultBackground);
        }

        /// <summary>
        /// Print the specified string using horizontal alignment within a specified field width.
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="str"></param>
        /// <param name="width"></param>
        /// <param name="hAlign"></param>
        /// <param name="fg"></param>
        /// <param name="bg"></param>
        public void PrintStringAligned(int cx, int cy, string str, int width, HorizontalAlignment hAlign, Color fg, Color bg)
        {
            int nx = cx + GetHorizontalDelta(str.Length, width, hAlign);

            PrintString(nx, cy, str, fg, bg);
        }

        /// <summary>
        /// Print the specified string using horizontal alignment within a specified field width.  The default background 
        /// color will be used
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="str"></param>
        /// <param name="width"></param>
        /// <param name="hAlign"></param>
        /// <param name="fg"></param>
        public void PrintStringAligned(int cx, int cy, string str, int width, HorizontalAlignment hAlign, Color fg)
        {
            PrintStringAligned(cx, cy, str, width, hAlign, fg, DefaultBackground);
        }

        /// <summary>
        /// Print the specified string using horizontal alignment within a specified field width.  The default foreground
        /// and background colors will be used
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="str"></param>
        /// <param name="width"></param>
        /// <param name="hAlign"></param>
        public void PrintStringAligned(int cx, int cy, string str, int width, HorizontalAlignment hAlign)
        {
            PrintStringAligned(cx, cy, str, width, hAlign, DefaultForeground, DefaultBackground);
        }

        /// <summary>
        /// Prints the string within the specified rectangle, with the specified alignments and wrapping.  The default
        /// background and foreground colors are used.  Any characters falling outside the surface boundaries
        /// are clipped.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="str"></param>
        /// <param name="hAlign"></param>
        /// <param name="vAlign"></param>
        /// <param name="wrapping"></param>
        public void PrintStringRect(Rectangle rect, string str, HorizontalAlignment hAlign, VerticalAlignment vAlign,
            WrappingType wrapping)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            switch (wrapping)
            {
                case WrappingType.None:

                    int nx = rect.X + GetHorizontalDelta(str.Length, rect.Width, hAlign);
                    int ny = rect.Y + GetVerticalDelta(1, rect.Height, vAlign);

                    PrintString(nx, ny, str);
                    break;

                case WrappingType.Character:
                    string[] stringlist = GetCharWrappedLines(str, rect.Width);

                    for (int i = 0; i < stringlist.Length; i++)
                    {
                        str = stringlist[i];

                        nx = rect.X + GetHorizontalDelta(str.Length, rect.Width, hAlign);
                        ny = i + rect.Y + GetVerticalDelta(stringlist.Length, rect.Height, vAlign);

                        if (ny >= rect.Top && ny < rect.Bottom)
                        {
                            PrintString(nx, ny, str);
                        }

                    }
                    break;

                case WrappingType.Word:
                    stringlist = GetWordWrappedLines(str, rect.Width);

                    for (int i = 0; i < stringlist.Length; i++)
                    {
                        str = stringlist[i];

                        nx = rect.X + GetHorizontalDelta(str.Length, rect.Width, hAlign);
                        ny = i + rect.Y + GetVerticalDelta(stringlist.Length, rect.Height, vAlign);

                        if (ny >= rect.Top && ny < rect.Bottom)
                        {
                            PrintString(nx, ny, str);
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// Prints the string within the specified rectangle, using the specified wrapping and horizontal aligment.
        /// Uses the default VerticalAlignment.Top and default background and foreground colors.
        /// Characters outside the surface boundaries are clipped.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="str"></param>
        /// <param name="hAlign"></param>
        public void PrintStringRect(Rectangle rect, string str, HorizontalAlignment hAlign)
        {
            PrintStringRect(rect, str, hAlign, VerticalAlignment.Top);
        }

        /// <summary>
        /// Prints the string within the specified rectangle, with the specified alignments.  No wrapping is performed.
        /// Uses the default background and foreground colors.  Characters outside the surface boundaries are clipped.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="str"></param>
        /// <param name="hAlign"></param>
        /// <param name="vAlign"></param>
        public void PrintStringRect(Rectangle rect, string str, HorizontalAlignment hAlign, VerticalAlignment vAlign)
        {
            PrintStringRect(rect, str, hAlign, vAlign, WrappingType.None);
        }

        #endregion

        #region Line Drawing Methods

        /// <summary>
        /// Draw a horizontal line using the specified character
        /// Characters falling outside the surface boundaries are clipped.
        /// </summary>
        /// <param name="leftX"></param>
        /// <param name="leftY"></param>
        /// <param name="length"></param>
        /// <param name="ch"></param>
        /// <param name="fg"></param>
        /// <param name="bg"></param>
        public void DrawHorizontalLine(int leftX, int leftY, int length, char ch, Color fg, Color bg)
        {
            for (int i = 0; i < length; i++)
            {
                PrintChar(leftX + i, leftY, ch, fg, bg);
            }
        }

        /// <summary>
        /// Draw a horizontal line using special characters, assuming one of the default font mappings (or similar) is being used.
        /// Characters falling outside the surface boundaries are clipped.
        /// </summary>
        /// <param name="leftX"></param>
        /// <param name="leftY"></param>
        /// <param name="length"></param>
        /// <param name="fg"></param>
        /// <param name="bg"></param>
        public void DrawHorizontalLine(int leftX, int leftY, int length, Color fg, Color bg)
        {
            DrawHorizontalLine(leftX, leftY, length, (char)SpecialChar.HorizontalLine, fg, bg);
        }

        /// <summary>
        /// Draw a horizontal line using special characters, assuming one of the default font mappings (or similar) is being used.
        /// The default background is used. Characters falling outside the surface boundaries are clipped.
        /// </summary>
        /// <param name="leftX"></param>
        /// <param name="leftY"></param>
        /// <param name="length"></param>
        /// <param name="fg"></param>
        public void DrawHorizontalLine(int leftX, int leftY, int length, Color fg)
        {
            DrawHorizontalLine(leftX, leftY, length, fg, DefaultBackground);
        }

        /// <summary>
        /// Draw a horizontal line using special characters, assuming one of the default font mappings (or similar) is being used.
        /// The default foreground and background colors are used.
        /// Characters falling outside the surface boundaries are clipped.
        /// </summary>
        /// <param name="leftX"></param>
        /// <param name="leftY"></param>
        /// <param name="length"></param>
        public void DrawHorizontalLine(int leftX, int leftY, int length)
        {
            DrawHorizontalLine(leftX, leftY, length, DefaultForeground, DefaultBackground);
        }

        /// <summary>
        /// Draw a vertical line using the specified character
        /// Characters falling outside the surface boundaries are clipped.
        /// </summary>
        /// <param name="topX"></param>
        /// <param name="topY"></param>
        /// <param name="length"></param>
        /// <param name="ch"></param>
        /// <param name="fg"></param>
        /// <param name="bg"></param>
        public void DrawVerticalLine(int topX, int topY, int length, char ch, Color fg, Color bg)
        {
            for (int i = 0; i < length; i++)
            {
                PrintChar(topX, topY + i, ch, fg, bg);
            }
        }

        /// <summary>
        /// Draw a vertical line using special characters, assuming one of the default font mappings (or similar) is being used.
        /// Characters falling outside the surface boundaries are clipped.
        /// </summary>
        /// <param name="topX"></param>
        /// <param name="topY"></param>
        /// <param name="length"></param>
        /// <param name="fg"></param>
        /// <param name="bg"></param>
        public void DrawVerticalLine(int topX, int topY, int length, Color fg, Color bg)
        {
            DrawVerticalLine(topX, topY, length, (char)SpecialChar.VerticalLine, fg, bg);
        }

        /// <summary>
        /// Draw a vertical line using special characters, assuming one of the default font mappings (or similar) is being used.
        /// The default background color is used.  Characters falling outside the surface boundaries are clipped.
        /// </summary>
        /// <param name="topX"></param>
        /// <param name="topY"></param>
        /// <param name="length"></param>
        /// <param name="fg"></param>
        public void DrawVerticalLine(int topX, int topY, int length, Color fg)
        {
            DrawVerticalLine(topX, topY, length, fg, DefaultBackground);
        }

        /// <summary>
        /// Draw a vertical line using special characters, assuming one of the default font mappings (or similar) is being used.
        /// The default foreground and background colors are used.
        /// Characters falling outside the surface boundaries are clipped.
        /// </summary>
        /// <param name="topX"></param>
        /// <param name="topY"></param>
        /// <param name="length"></param>
        public void DrawVerticalLine(int topX, int topY, int length)
        {
            DrawVerticalLine(topX, topY, length, DefaultForeground, DefaultBackground);
        }

        #endregion

        #region Other Drawing Methods

        /// <summary>
        /// Clears the entire surface, using the default colors and character.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Fills the specified rectangle with the character and colors specified.  Characters outside the
        /// surface boundaries are clipped.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="ch"></param>
        /// <param name="fg"></param>
        /// <param name="bg"></param>
        public void Fill(Rectangle rect, char ch, Color fg, Color bg)
        {
            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                for (int x = rect.Left; x < rect.Right; x++)
                {
                    SetCell(x, y, ch, fg, bg);
                }
            }
        }

        /// <summary>
        /// Fills a rectangle with the specified character using the default foreground and background colors.
        /// Characters falling outside the surface boundaries are clipped.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="ch"></param>
        public void Fill(Rectangle rect, char ch)
        {
            Fill(rect, ch, DefaultForeground, DefaultBackground);
        }

        /// <summary>
        /// Fills a rectangle with the specified character and foreground color and the default background color.
        /// Characters falling outside the surface boundaries are clipped.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="ch"></param>
        /// <param name="fg"></param>
        public void Fill(Rectangle rect, char ch, Color fg)
        {
            Fill(rect, ch, fg, DefaultBackground);
        }

        /// <summary>
        /// Draws a frame using special characters.
        /// If title is not null or empty, then this string is printed a the top left corner of the frame.
        /// Characters falling outside the surface boundaries are clipped.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="title"></param>
        /// <param name="clear">If true, clears the region inside the frame with the given back color</param>
        /// <param name="fore"></param>
        /// <param name="back"></param>
        /// <param name="frameDef">Characters used to draw frame, or default if null</param>
        public void DrawFrame(Rectangle rect, string title, bool clear, Color fore, Color back, FrameDefinition frameDef = null)
        {
            if(frameDef == null)
                frameDef = Surface.DefaultFrame;

            if (clear)
            {
                Fill(rect, ' ', fore, back);
            }

            DrawHorizontalLine(rect.Left, rect.Top, rect.Width - 1, frameDef.HorizontalLine, fore, back);
            DrawHorizontalLine(rect.Left, rect.Bottom - 1, rect.Width - 1, frameDef.HorizontalLine, fore, back);

            DrawVerticalLine(rect.Left, rect.Top, rect.Height - 1, frameDef.VerticalLine, fore, back);
            DrawVerticalLine(rect.Right - 1, rect.Top, rect.Height - 1, frameDef.VerticalLine, fore, back);

            PrintChar(rect.Left, rect.Top, frameDef.CornerUpperLeft, fore, back);
            PrintChar(rect.Right - 1, rect.Top, frameDef.CornerUpperRight, fore, back);
            PrintChar(rect.Left, rect.Bottom - 1, frameDef.CornerLowerLeft, fore, back);
            PrintChar(rect.Right - 1, rect.Bottom - 1, frameDef.CornerLowerRight, fore, back);

            if (!string.IsNullOrEmpty(title))
            {
                PrintString(rect.Left + 1, rect.Top, title, back, fore);
            }
        }

        /// <summary>
        /// Draws a frame using special characters.
        /// If title is not null or empty, then this string is printed a the top left corner of the frame.
        /// Characters falling outside the surface boundaries are clipped.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="title"></param>
        /// <param name="clear">If true, clears the region inside the frame</param>
        /// <param name="frameDef">Characters used to draw frame, or default if null</param>
        public void DrawFrame(Rectangle rect, string title = null, bool clear = false, FrameDefinition frameDef = null)
        {
            DrawFrame(rect, title, clear, DefaultForeground, DefaultBackground);
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Copies the entire source surface to the destination, ignoring all source cells having
        /// the default character and colors of the source surface.  Areas wich would fall outside the destination
        /// surface are clipped.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="destX"></param>
        /// <param name="destY"></param>
        public static void BlitWithMask(Surface src, Surface dest, int destX, int destY)
        {
            if (src == null)
                throw new ArgumentNullException("src");
            if (dest == null)
                throw new ArgumentNullException("dest");

            BlitWithMask(src, new Rectangle(0, 0, src.Width, src.Height), dest, destX, destY);
        }

        /// <summary>
        /// Copies a portion of the source surface to the destination, ignoring all source cells having
        /// the default character and colors of the source surface.  Areas wich would fall outside the destination
        /// surface are clipped.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="srcRect"></param>
        /// <param name="destX"></param>
        /// <param name="destY"></param>
        public static void BlitWithMask(Surface src, Rectangle srcRect, Surface dest, int destX, int destY)
        {
            if (src == null)
                throw new ArgumentNullException("src");
            if (dest == null)
                throw new ArgumentNullException("dest");

            BlitWithMask(src, srcRect, dest, destX, destY, src.DefaultChar, src.DefaultForeground, src.DefaultBackground);
        }

        /// <summary>
        /// Copies a portion of the source surface to the destination, ignoring all cells having
        /// the mask values.  Areas wich would fall outside the destination
        /// surface are clipped.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="srcRect"></param>
        /// <param name="destX"></param>
        /// <param name="destY"></param>
        /// <param name="maskChar"></param>
        /// <param name="maskFG"></param>
        /// <param name="maskBG"></param>
        public static void BlitWithMask(Surface src, Rectangle srcRect, Surface dest, int destX, int destY,
            char maskChar, Color maskFG, Color maskBG)
        {
            if (src == null)
                throw new ArgumentNullException("src");
            if (dest == null)
                throw new ArgumentNullException("dest");

            Rectangle blitRect = new Rectangle(destX, destY, srcRect.Width, srcRect.Height);
            int deltax = srcRect.Left - blitRect.Left;
            int deltay = srcRect.Top - blitRect.Top;

            blitRect = Rectangle.Intersect(blitRect, new Rectangle(0, 0, dest.Width, dest.Height));

            //bool dstIsRoot = dest is RootSurface;
            //RootSurface dstAsRoot = dest as RootSurface;
            Cell srcCell;

            for (int y = blitRect.Top; y < blitRect.Bottom; y++)
            {
                for (int x = blitRect.Left; x < blitRect.Right; x++)
                {
                    int sx = deltax + x;
                    int sy = deltay + y;

                    srcCell = src.GetCellUnchecked(sx, sy);

                    if (srcCell.ch != maskChar)
                    {
                        dest.SetCell(x, y, srcCell.ch, srcCell.fgColor, srcCell.bgColor);
                    }

                    //if (src.cells[sx + sy * src.Width].ch != maskChar)
                    //{
                    //    dest.cells[x + y * dest.Width].bgColor = src.cells[sx + sy * src.Width].bgColor;
                    //    dest.cells[x + y * dest.Width].fgColor = src.cells[sx + sy * src.Width].fgColor;
                    //    dest.cells[x + y * dest.Width].ch = src.cells[sx + sy * src.Width].ch;

                    //    if (dstIsRoot)
                    //    {
                    //        dstAsRoot.dirty[x + y * dest.Width] = 1;
                    //    }
                    //}
                }
            }
        }

        /// <summary>
        /// Copies a portion of the source surface to the destination surface.
        /// Areas wich would fall outside the destination surface are clipped.
        /// </summary>
        /// <param name="src">The source of the blit, normally an off-screen surface (but can be the root surface)</param>
        /// <param name="dest">The destination surface, often the root surface (but can be an off-screen surface)</param>
        /// <param name="srcRect">The area of the source surface that will be copied</param>
        /// <param name="destX">The destination X coordinate (in character coordinates)</param>
        /// <param name="destY">The destination Y coordinate (in character coordinates)</param>
        public static void Blit(Surface src, Rectangle srcRect, Surface dest, int destX, int destY)
        {
            if (src == null)
                throw new ArgumentNullException("src");
            if (dest == null)
                throw new ArgumentNullException("dest");

            Cell srcCell;

            Rectangle blitRect = new Rectangle(destX, destY, srcRect.Width, srcRect.Height);
            int deltax = srcRect.Left - blitRect.Left;
            int deltay = srcRect.Top - blitRect.Top;

            blitRect = Rectangle.Intersect(blitRect, new Rectangle(0, 0, dest.Width, dest.Height));

            //bool dstIsRoot = dest is RootSurface;
            //RootSurface dstAsRoot = dest as RootSurface;

            for (int y = blitRect.Top; y < blitRect.Bottom; y++)
            {
                for (int x = blitRect.Left; x < blitRect.Right; x++)
                {
                    int sx = deltax + x;
                    int sy = deltay + y;

                    srcCell = src.GetCellUnchecked(sx, sy);

                    //int di = x + y * dest.Width;
                    //int si = sx + sy * src.Width;

                    //dest.cells[di] = src.cells[si];
                    dest.SetCellUnchecked(x, y, srcCell.ch, srcCell.fgColor, srcCell.bgColor);

                    //if (dstIsRoot)
                    //{
                    //    dstAsRoot.dirty[x + y * dest.Width] = 1;
                    //}
                }
            }
        }

        /// <summary>
        /// Copies the entire source surface to the destination surface using simulated alpha transparency.
        /// Areas which fall outside the destination surface are clipped.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="destX"></param>
        /// <param name="destY"></param>
        /// <param name="alpha"></param>
        public static void BlitAlpha(Surface src, Surface dest, int destX, int destY, float alpha)
        {
            if (src == null)
                throw new ArgumentNullException("src");
            if (dest == null)
                throw new ArgumentNullException("dest");

            BlitAlpha(src, src.Rect, dest, destX, destY, alpha);
        }

        /// <summary>
        /// Copies a portion of the source surface to the destination surface using simulated alpha transparency.
        /// Areas wich would fall outside the destination surface are clipped.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="srcRect"></param>
        /// <param name="destX"></param>
        /// <param name="destY"></param>
        /// <param name="alpha">Alpha from 0 to 1 inclusive, where 0 is fully transparent and 1 is fully opaque</param>
        public static void BlitAlpha(Surface src, Rectangle srcRect, Surface dest, int destX, int destY, float alpha)
        {
            if (src == null)
                throw new ArgumentNullException("src");
            if (dest == null)
                throw new ArgumentNullException("dest");

            BlitAlpha(src, srcRect, dest, destX, destY, AlphaMode.BasicAlpha(alpha));

            //Cell srcCell, dstCell;
            //Color backCol, foreCol;
            //char ch;

            //Rectangle blitRect = new Rectangle(destX, destY, srcRect.Width, srcRect.Height);
            //int deltax = srcRect.Left - blitRect.Left;
            //int deltay = srcRect.Top - blitRect.Top;

            //blitRect = Rectangle.Intersect(blitRect, new Rectangle(0, 0, dest.Width, dest.Height));

            //for (int y = blitRect.Top; y < blitRect.Bottom; y++)
            //{
            //    for (int x = blitRect.Left; x < blitRect.Right; x++)
            //    {
            //        int sx = deltax + x;
            //        int sy = deltay + y;

            //        srcCell = src.GetCellUnchecked(sx, sy);
            //        dstCell = dest.GetCellUnchecked(x, y);

            //        backCol = ColorHelper.Lerp(dstCell.bgColor, srcCell.bgColor, alpha);

            //        if (srcCell.ch == ' ')
            //        {
            //            foreCol = ColorHelper.Lerp(dstCell.fgColor, srcCell.bgColor, alpha);
            //            ch = dstCell.ch;
            //        }
            //        else
            //        {
            //            foreCol = srcCell.fgColor;
            //            //foreCol = ColorHelper.Lerp(dstCell.fgColor, srcCell.bgColor, alpha);
            //            ch = srcCell.ch;
            //        }

            //        dest.SetCellUnchecked(x, y, ch, foreCol, backCol);
            //    }
            //}
        }

        /// <summary>
        /// Copies a portion of the source surface to the destination surface using simulated alpha transparency.
        /// Areas wich would fall outside the destination surface are clipped.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="srcRect"></param>
        /// <param name="dest"></param>
        /// <param name="destX"></param>
        /// <param name="destY"></param>
        /// <param name="mode">The alpha blending values to use</param>
        public static void BlitAlpha(Surface src, Rectangle srcRect, Surface dest, int destX, int destY, AlphaMode mode)
        {
            if (src == null)
                throw new ArgumentNullException("src");
            if (dest == null)
                throw new ArgumentNullException("dest");

            Cell srcCell, dstCell;
            Color backCol, foreCol;
            char ch;

            Rectangle blitRect = new Rectangle(destX, destY, srcRect.Width, srcRect.Height);
            int deltax = srcRect.Left - blitRect.Left;
            int deltay = srcRect.Top - blitRect.Top;

            blitRect = Rectangle.Intersect(blitRect, new Rectangle(0, 0, dest.Width, dest.Height));

            for (int y = blitRect.Top; y < blitRect.Bottom; y++)
            {
                for (int x = blitRect.Left; x < blitRect.Right; x++)
                {
                    int sx = deltax + x;
                    int sy = deltay + y;

                    srcCell = src.GetCellUnchecked(sx, sy);
                    dstCell = dest.GetCellUnchecked(x, y);

                    if (mode.BGColorAlpha == 0f)
                        backCol = dstCell.bgColor;
                    else if (mode.BGColorAlpha == 1f)
                        backCol = srcCell.bgColor;
                    else
                        backCol = ColorHelper.Lerp(dstCell.bgColor, srcCell.bgColor, mode.BGColorAlpha);

                    if (srcCell.ch == ' ' && mode.CharBleed != 1)
                    {
                        foreCol = ColorHelper.Lerp(dstCell.fgColor, srcCell.bgColor, mode.CharBleed);
                        ch = dstCell.ch;
                    }
                    else
                    {
                        //foreCol = srcCell.fgColor;
                        foreCol = ColorHelper.Lerp(dstCell.fgColor, srcCell.fgColor, mode.FGColorAlpha);
                        ch = srcCell.ch;
                    }

                    dest.SetCellUnchecked(x, y, ch, foreCol, backCol);
                }
            }
        }

        /// <summary>
        /// Copies the entire source surface to the destination surface.
        /// Areas wich would fall outside the destination surface are clipped.
        /// </summary>
        /// <param name="src">The source of the blit, normally an off-screen surface (but can be the root surface)</param>
        /// <param name="dest">The destination surface, often the root surface (but can be an off-screen surface)</param>
        /// <param name="destX">The destination X coordinate (in character coordinates)</param>
        /// <param name="destY">The destination Y coordinate (in character coordinates)</param>
        public static void Blit(Surface src, Surface dest, int destX, int destY)
        {
            if (src == null)
                throw new ArgumentNullException("src");
            if (dest == null)
                throw new ArgumentNullException("dest");

            Blit(src, new Rectangle(0, 0, src.Width, src.Height), dest, destX, destY);
        }

        #endregion

        #region Private

        static FrameDefinition DefaultFrame = new FrameDefinition();

        internal abstract Cell GetCellUnchecked(int cx, int cy);

        internal abstract void SetCellUnchecked(int cx, int cy, char? ch, Color? fg, Color? bg);

        internal void SetCell(int cx, int cy, char? ch, Color? fg, Color? bg)
        {
            if (cx < 0 || cx >= Width || cy < 0 || cy >= Height)
                return;

            SetCellUnchecked(cx, cy, ch, fg, bg);
        }

        internal struct Cell
        {
            public char ch;
            public Color fgColor;
            public Color bgColor;
        }

        internal int GetHorizontalDelta(int strLength, int width, HorizontalAlignment hAlign)
        {
            int dx;

            switch (hAlign)
            {
                case HorizontalAlignment.Left:
                    dx = 0;
                    break;

                case HorizontalAlignment.Center:
                    dx = (width - strLength) / 2;
                    break;

                case HorizontalAlignment.Right:
                default:
                    dx = (width - strLength);
                    break;
            }

            return dx;
        }

        internal int GetVerticalDelta(int numLines, int height, VerticalAlignment vAlign)
        {
            int dy;

            switch (vAlign)
            {
                case VerticalAlignment.Top:
                    dy = 0;
                    break;

                case VerticalAlignment.Center:
                    dy = (height - numLines) / 2;
                    break;

                case VerticalAlignment.Bottom:
                default:
                    dy = height - numLines;
                    break;
            }

            return dy;
        }

        internal string[] GetCharWrappedLines(string str, int maxWidth)
        {
            List<string> stringlist = new List<string>();

            int count = 0;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                builder.Append(str[i]);
                count++;
                if (count >= maxWidth)
                {
                    count = 0;
                    stringlist.Add(builder.ToString());
                    builder.Clear();
                }
            }
            stringlist.Add(builder.ToString());

            return stringlist.ToArray();
        }

        internal string[] GetWordWrappedLines(string str, int width)
        {
            List<string> lines = new List<string>();

            string[] words = Explode(str);
            int currlength = 0;
            StringBuilder currentLine = new StringBuilder();

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length + currlength > width)
                {
                    if (currlength > 0)
                    {
                        lines.Add(currentLine.ToString());
                        currentLine.Clear();
                        currlength = 0;
                    }

                    if (words[i].Length > width)
                    {
                        string[] cwlines = GetCharWrappedLines(words[i], width);
                        foreach (string s in cwlines)
                        {
                            lines.Add(s);
                        }

                        currlength = 0;
                    }
                    else
                    {
                        currentLine.Append(words[i] + ' ');
                        currlength += words[i].Length + 1;
                    }
                }
                else
                {
                    currentLine.Append(words[i] + ' ');
                    currlength += words[i].Length + 1;
                }
            }

            if (currentLine.Length > 0)
                lines.Add(currentLine.ToString());

            return lines.ToArray();
        }

        internal string[] Explode(string str)
        {
            List<string> stringList = new List<string>();
            int currIndex = 0;
            StringBuilder builder = new StringBuilder();

            while (true)
            {
                while (currIndex < str.Length && char.IsWhiteSpace(str[currIndex]))
                {
                    currIndex++;
                }

                while (currIndex < str.Length && !char.IsWhiteSpace(str[currIndex]))
                {
                    builder.Append(str[currIndex]);
                    currIndex++;
                }
                stringList.Add(builder.ToString());
                builder.Clear();

                if (currIndex >= str.Length)
                    break;
            }

            return stringList.ToArray();
        }
        #endregion
    }




}
