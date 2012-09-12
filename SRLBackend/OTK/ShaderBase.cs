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
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace SRLBackend.OTK
{
    public abstract class ShaderBase<T> where T:struct
    {
        int vertexShaderHandle;
        int fragmentShaderHandle;
        int shaderProgramHandle;

        VertexDefinition vertexDefinition;

        protected ShaderBase(string vShaderSrc, string fShaderSrc, VertexDefinition vertexDef)
        {
            CreateShaders(vShaderSrc, fShaderSrc);

            shaderProgramHandle = GL.CreateProgram();

            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, fragmentShaderHandle);

            GL.LinkProgram(shaderProgramHandle);

            string programInfoLog;
            GL.GetProgramInfoLog(shaderProgramHandle, out programInfoLog);
            System.Diagnostics.Debug.WriteLine(programInfoLog);


            vertexDefinition = vertexDef;
        }

        public virtual void SetState()
        {
            GL.UseProgram(shaderProgramHandle);

            foreach (var att in vertexDefinition)
            {
                GL.VertexAttribPointer(AttributeLocation(att.Name),
                    att.NumValues,
                    att.ValueType,
                    false,
                    vertexDefinition.Stride,
                    att.Offset);

                GL.EnableVertexAttribArray(AttributeLocation(att.Name));
            }
        }

        public virtual void ClearState()
        {
            GL.UseProgram(0);
        }

        private Dictionary<string, int> uniformLocations = new Dictionary<string, int>();
        private Dictionary<string, int> attributeLocations = new Dictionary<string, int>();

        protected int UniformLocation(string name)
        {
            if (uniformLocations.ContainsKey(name))
                return uniformLocations[name];

            int loc = GL.GetUniformLocation(ProgramHandle, name);
            uniformLocations.Add(name, loc);



            return loc;
        }

        protected int AttributeLocation(string name)
        {
            if (attributeLocations.ContainsKey(name))
                return attributeLocations[name];

            int loc = GL.GetAttribLocation(ProgramHandle, name);
            attributeLocations.Add(name, loc);
            return loc;
        }

        protected int ProgramHandle { get { return shaderProgramHandle; } }

        void CreateShaders(string vShaderSource, string fShaderSource)
        {
            vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vertexShaderHandle, vShaderSource);
            GL.ShaderSource(fragmentShaderHandle, fShaderSource);

            GL.CompileShader(vertexShaderHandle);
            GL.CompileShader(fragmentShaderHandle);

            string programInfoLog;

            GL.GetShaderInfoLog(vertexShaderHandle, out programInfoLog);
            System.Diagnostics.Debug.WriteLine(programInfoLog);

            GL.GetShaderInfoLog(fragmentShaderHandle, out programInfoLog);
            System.Diagnostics.Debug.WriteLine(programInfoLog);
        }
    }
}
