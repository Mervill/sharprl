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
using System.Runtime.InteropServices;
using OpenTK.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SRLBackend.OTK
{
    [StructLayout(LayoutKind.Sequential)]
    struct TileVertex
    {
        public TileVertex(float x, float y, float tx, float ty, Color4 color)
        {
            position.X = x;
            position.Y = y;
            texcoord.X = tx;
            texcoord.Y = ty;
            this.color = color;
        }

        public Vector2 position;
        public Vector2 texcoord;
        public Color4 color;

        public static VertexDefinition Definition;

        static TileVertex()
        {
            Definition = new VertexDefinition(32,
                new VertexAttribute("inPos", 2, VertexAttribPointerType.Float, 0),
                new VertexAttribute("inTexcoord", 2, VertexAttribPointerType.Float, 8),
                new VertexAttribute("inColor", 4, VertexAttribPointerType.Float, 16)
            );
        }
    }
}
