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
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SRLBackend.OTK
{
    class GlyphBatch
    {
        const int MaxQuads = 2000;

        GlyphVertex[] verts = new GlyphVertex[MaxQuads * 4];
        VertexBuffer<GlyphVertex> vertexBuffer;
        Texture2D currentTexture;

        public Matrix4 WorldView { get; set; }

        int currentQuadCount;
        GlyphShader shader;

        public GlyphBatch()
        {
            shader = new GlyphShader();
            WorldView = Matrix4.Identity;

            vertexBuffer = new VertexBuffer<GlyphVertex>(verts, BufferUsageHint.DynamicDraw);
        }

        bool hasBegun;
        public void Begin(Texture2D texture)
        {
            hasBegun = true;
            currentQuadCount = 0;
            currentTexture = texture;
        }

        int dl, dt, dr, db;
        int sl, st, sr, sb;
        public void Draw(RectangleF destRect, RectangleF srcRect, Color fgColor, Color bgColor)
        {
            if (!hasBegun)
                throw new InvalidOperationException("Must call Begin before drawing");

            if (currentQuadCount >= MaxQuads)
            {
                Flush();
                currentQuadCount = 0;
            }

            int startIndex = currentQuadCount * 4;

            dl = (int)destRect.Left;
            dt = (int)destRect.Top;
            dr = (int)destRect.Right;
            db = (int)destRect.Bottom;

            sl = (int)srcRect.Left;
            st = (int)srcRect.Top;
            sr = (int)srcRect.Right;
            sb = (int)srcRect.Bottom;

            verts[startIndex].position.X = dl;
            verts[startIndex].position.Y = dt;
            verts[startIndex].texcoord.X = sl / (float)currentTexture.Width;
            verts[startIndex].texcoord.Y = st / (float)currentTexture.Height;

            verts[startIndex + 1].position.X = dr;
            verts[startIndex + 1].position.Y = dt;
            verts[startIndex + 1].texcoord.X = sr / (float)currentTexture.Width;
            verts[startIndex + 1].texcoord.Y = st / (float)currentTexture.Height;

            verts[startIndex + 2].position.X = dr;
            verts[startIndex + 2].position.Y = db;
            verts[startIndex + 2].texcoord.X = sr / (float)currentTexture.Width;
            verts[startIndex + 2].texcoord.Y = sb / (float)currentTexture.Height;

            verts[startIndex + 3].position.X = dl;
            verts[startIndex + 3].position.Y = db;
            verts[startIndex + 3].texcoord.X = sl / (float)currentTexture.Width;
            verts[startIndex + 3].texcoord.Y = sb / (float)currentTexture.Height;

            //verts[startIndex].position.X = destRect.X;
            //verts[startIndex].position.Y = destRect.Y;
            //verts[startIndex].texcoord.X = srcRect.X / (float)currentTexture.Width;
            //verts[startIndex].texcoord.Y = srcRect.Y / (float)currentTexture.Height;

            //verts[startIndex + 1].position.X = destRect.Right;
            //verts[startIndex + 1].position.Y = destRect.Y;
            //verts[startIndex + 1].texcoord.X = srcRect.Right / (float)currentTexture.Width;
            //verts[startIndex + 1].texcoord.Y = srcRect.Y / (float)currentTexture.Height;

            //verts[startIndex + 2].position.X = destRect.Right;
            //verts[startIndex + 2].position.Y = destRect.Bottom;
            //verts[startIndex + 2].texcoord.X = srcRect.Right / (float)currentTexture.Width;
            //verts[startIndex + 2].texcoord.Y = srcRect.Bottom / (float)currentTexture.Height;

            //verts[startIndex + 3].position.X = destRect.X;
            //verts[startIndex + 3].position.Y = destRect.Bottom;
            //verts[startIndex + 3].texcoord.X = srcRect.X / (float)currentTexture.Width;
            //verts[startIndex + 3].texcoord.Y = srcRect.Bottom / (float)currentTexture.Height;


            for (int i = 0; i < 4; i++)
            {
                verts[startIndex + i].fgColor.R = fgColor.R / 255f;
                verts[startIndex + i].fgColor.G = fgColor.G / 255f;
                verts[startIndex + i].fgColor.B = fgColor.B / 255f;
                verts[startIndex + i].fgColor.A = fgColor.A / 255f;

                verts[startIndex + i].bgColor.R = bgColor.R / 255f;
                verts[startIndex + i].bgColor.G = bgColor.G / 255f;
                verts[startIndex + i].bgColor.B = bgColor.B / 255f;
                verts[startIndex + i].bgColor.A = bgColor.A / 255f;
            }

            currentQuadCount++;
        }

        public void End()
        {
            if (!hasBegun)
                return;

            hasBegun = false;

            Flush();
        }

        private void Flush()
        {
            shader.Texture = currentTexture;
            shader.WorldView = WorldView;

            vertexBuffer.Bind();
            vertexBuffer.ResetData(verts, currentQuadCount * 4);

            shader.SetState();

            GL.DrawArrays(BeginMode.Quads, 0, currentQuadCount * 4);

            shader.ClearState();
        }
    }
}
