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
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using SRLBackend.OTK;

namespace SRLBackend
{
    public class RenderWindow : GameWindow
    {
        TileBatch tileBatch;
        GlyphBatch glyphBatch;
    
        public Matrix4 WorldView { get; set; }

        public RenderWindow(int width, int height, string windowTitle)
            : base(width, height, GraphicsMode.Default, windowTitle, GameWindowFlags.Default, DisplayDevice.Default,
            2, 0, GraphicsContextFlags.Default)
        {
            GL.Viewport(0, 0, ClientSize.Width, ClientSize.Height);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            WorldView = Matrix4.CreateOrthographicOffCenter(0, ClientSize.Width, ClientSize.Height, 0, 0, 100);

            tileBatch = new TileBatch();
            glyphBatch = new GlyphBatch();

            WindowBorder = OpenTK.WindowBorder.Fixed;
        }

        public void Clear(System.Drawing.Color color)
        {
            GL.ClearColor(color);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            Location = Point.Empty;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, ClientSize.Width, ClientSize.Height);

            WorldView = Matrix4.CreateOrthographicOffCenter(0, ClientSize.Width, ClientSize.Height, 0, 0, 100);


        }

        Dictionary<int, Texture2D> textures = new Dictionary<int, Texture2D>();
        Texture2D currentTexture;

        public void RegisterImageResource(Bitmap bitmap, out int handle)
        {
            Texture2D texture = new Texture2D(bitmap);
            handle = texture.Handle;
            textures.Add(handle, texture);
        }

        bool glyphsBegun;

        public void BeginGlyphs(int imageID)
        {
            if (tilesBegun)
            {
                tileBatch.End();
            }

            if (glyphsBegun)
            {
                glyphBatch.End();
            }

            glyphsBegun = true;

            glyphBatch.WorldView = WorldView;

            if (!textures.ContainsKey(imageID))
                throw new ArgumentException("Image ID not found, register image resource first");

            Texture2D texture = textures[imageID];
            currentTexture = texture;

            glyphBatch.Begin(texture);

        }

        public void DrawGlyph(RectangleF destRect, RectangleF srcRect, Color fgColor, Color bgColor)
        {
            if (!glyphsBegun)
                throw new ArgumentException("Must call BeginGlyphs before calling DrawGlyph");

            glyphBatch.Draw(destRect, srcRect, fgColor, bgColor);
        }

        public void EndGlyphs()
        {
            if (!glyphsBegun)
                return;

            glyphBatch.End();

            glyphsBegun = false;
        }

        bool tilesBegun;

        public void BeginTiles(int imageID)
        {
            if (glyphsBegun)
            {
                glyphBatch.End();
            }

            if (tilesBegun)
            {
                tileBatch.End();
            }

            tilesBegun = true;

            tileBatch.WorldView = WorldView;

            if (!textures.ContainsKey(imageID))
                throw new ArgumentException("Image ID not found, register image resource first");

            Texture2D texture = textures[imageID];
            currentTexture = texture;

            tileBatch.Begin(texture);
        }

        public void DrawTile(RectangleF destRect, RectangleF srcRect, Color color)
        {
            if (!tilesBegun)
                throw new ArgumentException("Must call BeginTiles before calling DrawTile");

            tileBatch.Draw(destRect, srcRect, color);
        }

        public void EndTiles()
        {
            if (!tilesBegun)
                return;

            tileBatch.End();

            tilesBegun = false;
        }
    }
}
