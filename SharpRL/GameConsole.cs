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
using SRLBackend;
using System.Drawing;

namespace SharpRL
{
    public class GameConsole : IDisposable
    {
        #region Private Fields

        RenderWindow renderer;

        Action<float> OnDraw;

        FontSheet currentFont;

        Size rowcolSize;
        float cellWidth;
        float cellHeight;

        //Queue<TileMap> tileMapQueue = new Queue<TileMap>();

        class TileDrawInfo
        {
            public Point Pos;
            public Point TilePos;
            public Color Color;
        }

        Dictionary<TileSheet, Queue<TileDrawInfo>> tileBatch = new Dictionary<TileSheet, Queue<TileDrawInfo>>();
        

        #endregion


        #region Events

        /// <summary>
        /// Raised when any key on the keyboard is down.  If the key remains down, this event will repeatedly
        /// fire.
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyDown;

        /// <summary>
        /// Raised when a key or series of keys has been pressed which produces a printable (ASCII) character
        /// </summary>
        public event EventHandler<KeyCharEventArgs> KeyChar;

        /// <summary>
        /// Raised when a key on the keyboard has been released.
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyUp;

        /// <summary>
        /// Raised when the mouse pointer has moved while over the window.
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseMove;

        /// <summary>
        /// Raised when one of the mouse buttons has been pressed
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseButtonDown;

        /// <summary>
        /// Raised when one of the mouse buttons has been released.
        /// </summary>
        public event EventHandler<MouseEventArgs> MouseButtonUp;

        #endregion


        #region Properties

        /// <summary>
        /// Get the backend Renderer object for this application
        /// </summary>
        public RenderWindow Renderer { get { return renderer; } }


        /// <summary>
        /// Get the root surface of the console
        /// </summary>
        public RootSurface Root { get; private set; }

        /// <summary>
        /// Get the current Font
        /// </summary>
        public FontSheet CurrentFont { get { return currentFont; } }

        /// <summary>
        /// Get or set whether or not the window can be resized by dragging the border or clicking on the maximize button.
        /// </summary>
        public bool IsWindowResizable
        {
            get
            {
                return renderer.WindowBorder == OpenTK.WindowBorder.Resizable;
            }
            set
            {
                if (value)
                    renderer.WindowBorder = OpenTK.WindowBorder.Resizable;
                else
                    renderer.WindowBorder = OpenTK.WindowBorder.Fixed;
     
            }
        }

        /// <summary>
        /// Get or set the size of the window (client size) in pixels.  If the window changes size, the font characters
        /// will be scaled when rendered to the screen.
        /// </summary>
        public Size WindowSize
        {
            get { return renderer.ClientSize; }
            set { renderer.ClientSize = value; }
        }

        /// <summary>
        /// Get or set the title of the window.
        /// </summary>
        public string WindowTitle
        {
            get { return renderer.Title; }
            set { renderer.Title = value; }
        }


        /// <summary>
        /// Get or set if wait for vertical sync is enabled
        /// </summary>
        public bool VerticalSyncEnabled
        {
            get { return renderer.VSync == OpenTK.VSyncMode.On; }
            set
            {
                if (value)
                    renderer.VSync = OpenTK.VSyncMode.On;
                else
                    renderer.VSync = OpenTK.VSyncMode.Off;
            }
        }

        /// <summary>
        /// Get or set the desired (maximum) frame rate
        /// </summary>
        public int TargetFPS
        {
            get { return (int)renderer.TargetRenderFrequency; }
            set { renderer.TargetRenderFrequency = value; }
        }


        #endregion


        #region CTOR

        /// <summary>
        /// Create a game console
        /// </summary>
        /// <param name="numColumns">Number of horizontal characters in the console</param>
        /// <param name="numberRows">Number of vertical characters in the console</param>
        /// <param name="windowTitle">Title of the window</param>
        /// <param name="windowWidth">Width, in pixels, of the client area of the window</param>
        /// <param name="windowHeight">Height, in pixels, of the client area of the window</param>
        public GameConsole(int numColumns, int numberRows, string windowTitle, int windowWidth, int windowHeight)
        {
            renderer = new RenderWindow(windowWidth, windowHeight, windowTitle);

            rowcolSize = new Size(numColumns, numberRows);
            cellWidth = windowWidth / (float)rowcolSize.Width;
            cellHeight = windowHeight / (float)rowcolSize.Height;

            Root = new RootSurface(rowcolSize.Width, rowcolSize.Height, this);

            renderer.Keyboard.KeyRepeat = true;

            renderer.Keyboard.KeyDown += new EventHandler<OpenTK.Input.KeyboardKeyEventArgs>(renderWindow_KeyDown);
            renderer.Keyboard.KeyUp += new EventHandler<OpenTK.Input.KeyboardKeyEventArgs>(renderWindow_KeyUp);
            renderer.KeyPress += new EventHandler<OpenTK.KeyPressEventArgs>(renderWindow_KeyPress);

            renderer.Mouse.Move += new EventHandler<OpenTK.Input.MouseMoveEventArgs>(Mouse_Move);
            renderer.Mouse.ButtonDown += new EventHandler<OpenTK.Input.MouseButtonEventArgs>(Mouse_ButtonDown);
            renderer.Mouse.ButtonUp += new EventHandler<OpenTK.Input.MouseButtonEventArgs>(Mouse_ButtonUp);
        }

        #endregion


        #region Modification Methods

        /// <summary>
        /// Set the font used by this console, and by default resizes the window according to the base font character size.  If the window
        /// is not resized, then the font glyphs will be scaled appropriately
        /// </summary>
        /// <param name="font"></param>
        /// <param name="resizeWindowToFit"></param>
        public void SetFont(FontSheet font, bool resizeWindowToFit = true)
        {
            currentFont = font;

            if (resizeWindowToFit)
            {
                renderer.ClientSize = new Size(rowcolSize.Width * font.CharacterWidth, rowcolSize.Height * font.CharacterHeight);
            }
        }

        /// <summary>
        /// Set the number of character columns and rows, and by default resizes the window according to the base font character size.
        /// </summary>
        /// <param name="numColumns"></param>
        /// <param name="numRows"></param>
        public void SetRowsColumns(int numColumns, int numRows, bool resizeWindowToFit = true)
        {
            rowcolSize = new Size(numColumns, numRows);

            Root = new RootSurface(rowcolSize.Width, rowcolSize.Height, this);

            if (resizeWindowToFit)
            {
                renderer.ClientSize = new Size(rowcolSize.Width * currentFont.CharacterWidth, 
                    rowcolSize.Height * currentFont.CharacterHeight);
            }

            cellWidth = renderer.ClientSize.Width / (float)rowcolSize.Width;
            cellHeight = renderer.ClientSize.Height / (float)rowcolSize.Height;
        }

        #endregion


        #region Utility Methods

        /// <summary>
        /// Get the pixel coordinates of the given character position, taking into account any scaling.  Useful, for example, when
        /// aligning tiles to console coordinates on a window that has been resized.
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <returns></returns>
        public Point GetPixelLocation(int cx, int cy)
        {
            Point pos = new Point();
            pos.X = (int)(cx * cellWidth);
            pos.Y = (int)(cy * cellHeight);

            return pos;
        }

        /// <summary>
        /// Get the size of the current font characters in pixels, taking into account any scaling.
        /// </summary>
        /// <returns></returns>
        public SizeF EffectiveCharacterSize()
        {
            SizeF sz = new SizeF(cellWidth, cellHeight);
            return sz;
        }

        #endregion


        #region Game Loop

        /// <summary>
        /// Start the main loop, providing a callback method that will get invoked each loop iteration.  Optionally
        /// set the target frame rate.
        /// </summary>
        /// <param name="OnDraw"></param>
        /// <param name="targetFPS"></param>
        public void Run(Action<float> OnDraw, int targetFPS = 0)
        {
            this.OnDraw = OnDraw;

            renderer.Resize += new EventHandler<EventArgs>(renderer_Resize);
            renderer.RenderFrame += new EventHandler<OpenTK.FrameEventArgs>(Tick);

            renderer.Run(0, targetFPS);
        }

        /// <summary>
        /// Immediately stop the game loop and close the application window.  GameWindow.Dispose should get called
        /// after calling Exit.
        /// </summary>
        public void Exit()
        {
            renderer.Exit();
        }


        private void Tick(object sender, OpenTK.FrameEventArgs e)
        {
            if (OnDraw != null)
                OnDraw((float)e.Time);

            Flush();

            renderer.SwapBuffers();
        }

        #endregion


        #region Event Handlers

        void renderer_Resize(object sender, EventArgs e)
        {
            cellWidth = renderer.ClientSize.Width / (float)rowcolSize.Width;
            cellHeight = renderer.ClientSize.Height / (float)rowcolSize.Height;
        }

        bool[] currentMouseButtons = new bool[3];

        void Mouse_ButtonUp(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            MouseButton mb = MouseButton.None;

            switch (e.Button)
            {
                case OpenTK.Input.MouseButton.Left:
                    mb = MouseButton.Left;
                    currentMouseButtons[0] = false;
                    break;

                case OpenTK.Input.MouseButton.Middle:
                    mb = MouseButton.Middle;
                    currentMouseButtons[1] = false;
                    break;

                case OpenTK.Input.MouseButton.Right:
                    mb = MouseButton.Right;
                    currentMouseButtons[2] = false;
                    break;
            }

            if (MouseButtonUp != null)
            {
                MouseEventArgs args = new MouseEventArgs();

                args.Button = mb;

                args.PX = e.X;
                args.PY = e.Y;
                args.CX = (int)(e.X / cellWidth);
                args.CY = (int)(e.Y / cellHeight);

                MouseButtonUp(this, args);
            }
        }

        void Mouse_ButtonDown(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {

            MouseButton mb = MouseButton.None;

            switch (e.Button)
            {
                case OpenTK.Input.MouseButton.Left:
                    mb = MouseButton.Left;
                    currentMouseButtons[0] = true;
                    break;

                case OpenTK.Input.MouseButton.Middle:
                    mb = MouseButton.Middle;
                    currentMouseButtons[1] = true;
                    break;

                case OpenTK.Input.MouseButton.Right:
                    mb = MouseButton.Right;
                    currentMouseButtons[2] = true;
                    break;
            }

            if (MouseButtonDown != null)
            {
                MouseEventArgs args = new MouseEventArgs();

                args.Button = mb;

                args.PX = e.X;
                args.PY = e.Y;
                args.CX = (int)(e.X / cellWidth);
                args.CY = (int)(e.Y / cellHeight);

                MouseButtonDown(this, args);
            }

        }

        void Mouse_Move(object sender, OpenTK.Input.MouseMoveEventArgs e)
        {
            if (MouseMove != null)
            {
                MouseEventArgs args = new MouseEventArgs();

                args.PX = e.X;
                args.PY = e.Y;
                args.CX = (int)(e.X / cellWidth);
                args.CY = (int)(e.Y / cellHeight);

                if (currentMouseButtons[0])
                    args.Button = MouseButton.Left;
                else if (currentMouseButtons[1])
                    args.Button = MouseButton.Middle;
                else if (currentMouseButtons[2])
                    args.Button = MouseButton.Right;
            }
        }

        void renderWindow_KeyPress(object sender, OpenTK.KeyPressEventArgs e)
        {
            if (KeyChar != null)
            {
                KeyCharEventArgs args = new KeyCharEventArgs();
                args.Char = e.KeyChar;

                KeyChar(this, args);
            }
        }

        void renderWindow_KeyUp(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (KeyUp != null)
            {
                KeyEventArgs args = KeyboardConverter.Convert(e);
                KeyUp(this, args);
            }
        }

        void renderWindow_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (KeyDown != null)
            {
                KeyEventArgs args = KeyboardConverter.Convert(e);
                KeyDown(this, args);
            }
        }
        #endregion


        #region Rendering

        /// <summary>
        /// Draws a specified tile from a tile sheet to the screen.  All tile drawing calls are batched and
        /// flushed to the render window at the end of the render loop - this means tiles will always
        /// be drawn on top of the console characters no matter the order of the DrawTile calls.
        /// </summary>
        /// <param name="tiles"></param>
        /// <param name="destX"></param>
        /// <param name="destY"></param>
        /// <param name="tilePosX"></param>
        /// <param name="tilePosY"></param>
        /// <param name="tint"></param>
        public void DrawTile(TileSheet tiles, int destX, int destY, int tilePosX, int tilePosY, Color tint)
        {
            TileDrawInfo drawInfo = new TileDrawInfo();

            drawInfo.Pos.X = destX;
            drawInfo.Pos.Y = destY;
            drawInfo.TilePos.X = tilePosX;
            drawInfo.TilePos.Y = tilePosY;
            drawInfo.Color = tint;

            if (!tileBatch.ContainsKey(tiles))
            {
                tileBatch.Add(tiles, new Queue<TileDrawInfo>());
            }

            tileBatch[tiles].Enqueue(drawInfo);
        }

        /// <summary>
        /// Draws a specified tile from a tile sheet to the screen.  All tile drawing calls are batched and
        /// flushed to the render window at the end of the render loop - this means tiles will always
        /// be drawn on top of the console characters no matter the order of the DrawTile calls.
        /// </summary>
        /// <param name="tiles"></param>
        /// <param name="destX"></param>
        /// <param name="destY"></param>
        /// <param name="tilePosX"></param>
        /// <param name="tilePosY"></param>
        public void DrawTile(TileSheet tiles, int destX, int destY, int tilePosX, int tilePosY)
        {
            DrawTile(tiles, destX, destY, tilePosX, tilePosY, Color.White);
        }

        /// <summary>
        /// Draws a specified tile from a tile sheet to the screen.  All tile drawing calls are batched and
        /// flushed to the render window at the end of the render loop - this means tiles will always
        /// be drawn on top of the console characters no matter the order of the DrawTile calls.
        /// </summary>
        /// <param name="tiles"></param>
        /// <param name="destCoord"></param>
        /// <param name="tilePos"></param>
        public void DrawTile(TileSheet tiles, Point destCoord, Point tilePos)
        {
            DrawTile(tiles, destCoord.X, destCoord.Y, tilePos.X, tilePos.Y);
        }

        /// <summary>
        /// Draws a specified tile from a tile sheet to the screen.  All tile drawing calls are batched and
        /// flushed to the render window at the end of the render loop - this means tiles will always
        /// be drawn on top of the console characters no matter the order of the DrawTile calls.
        /// </summary>
        /// <param name="tiles"></param>
        /// <param name="destCoord">The position in pixels where the tile will be drawn</param>
        /// <param name="tilePos">The position of the tile in the tile sheet, in tile coordinates</param>
        /// <param name="tint">A color used to tint the drawn tile</param>
        public void DrawTile(TileSheet tiles, Point destCoord, Point tilePos, Color tint)
        {
            DrawTile(tiles, destCoord.X, destCoord.Y, tilePos.X, tilePos.Y, tint);
        }

        RectangleF dst, src;

        internal void Clear(Color color)
        {
            renderer.Clear(color);

        }

        private void Flush()
        {
            if (currentFont != null)
            {

                renderer.BeginGlyphs(currentFont.Handle);

                for (int y = 0; y < Root.Size.Height; y++)
                {
                    for (int x = 0; x < Root.Size.Width; x++)
                    {
                        if (Root.dirty[x + y * Root.Size.Width] == 1)
                        {
                            var cell = Root.cells[x + y * Root.Size.Width];

                            RenderGlyph(x, y, currentFont.GetCharPos(cell.ch),
                                cell.fgColor,
                                cell.bgColor);

                        }
                    }
                }
                renderer.EndGlyphs();
            }

            RenderTiles();
        }

        private void RenderTiles()
        {
            foreach (var tk in tileBatch)
            {
                var queue = tk.Value;
                var handle = tk.Key.Handle;

                dst.Size = tk.Key.TileSize;
                src.Size = tk.Key.TileSize;

                renderer.BeginTiles(handle);
                while (queue.Count > 0)
                {
                    var tInfo = queue.Dequeue();
                    dst.X = tInfo.Pos.X;
                    dst.Y = tInfo.Pos.Y;

                    src.X = tInfo.TilePos.X * tk.Key.TileWidth;
                    src.Y = tInfo.TilePos.Y * tk.Key.TileHeight;

                    renderer.DrawTile(dst, src, tInfo.Color);
                }
                renderer.EndTiles();
            }
        }

        private void RenderGlyph(int cx, int cy, Point gPos, Color fg, Color bg)
        {
            dst.X = cx * cellWidth;
            dst.Y = cy * cellHeight;
            dst.Width = cellWidth;
            dst.Height = cellHeight;

            src.X = gPos.X * currentFont.CharacterWidth;
            src.Y = gPos.Y * currentFont.CharacterHeight;

            src.Width = currentFont.CharacterWidth;
            src.Height = currentFont.CharacterHeight;

            renderer.DrawGlyph(dst, src, fg, bg);
        }

        #endregion


        #region Cleanup

        public void Dispose()
        {
            renderer.Dispose();
        }

        #endregion
    }
}
