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
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SRLBackend.OTK
{
    class TileShader : ShaderBase<TileVertex>
    {
        private Matrix4 worldView;
        public Matrix4 WorldView
        {
            get { return worldView; }
            set { worldView = value; }
        }

        private Texture2D texture;
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public TileShader()
            : base(vSource, fSource, TileVertex.Definition)
        {
            WorldView = Matrix4.Identity;
            Texture = null;
            GL.UseProgram(ProgramHandle);
            GL.Uniform1(UniformLocation("texture"), 0);
        }

        public override void SetState()
        {
            base.SetState();
            GL.UniformMatrix4(UniformLocation("worldView"), true, ref worldView);

            GL.BindTexture(TextureTarget.Texture2D, Texture.Handle);
            GL.ActiveTexture(TextureUnit.Texture0);

            GL.Uniform1(UniformLocation("texture"), 0);
        }



        static string vSource = @"
#version 110

attribute vec2 inPos;
attribute vec2 inTexcoord;
attribute vec4 inColor;

uniform mat4 worldView;

varying vec2 texcoord;
varying vec4 color;

void main()
{
    gl_Position = vec4(inPos, 0, 1) * worldView;
    texcoord = inTexcoord;
    color = inColor;
}

";

        static string fSource = @"
#version 110

uniform sampler2D texture;

varying vec2 texcoord;
varying vec4 color;

void main()
{
    gl_FragColor = texture2D(texture, texcoord) * color;
}

";
    }
}
