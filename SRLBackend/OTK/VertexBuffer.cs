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
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace SRLBackend.OTK
{
    public class VertexBuffer<T> where T:struct
    {
        int handle;
        int vertSizeInBytes;

        public int Handle { get { return handle; } }

        public VertexBuffer(T[] data, BufferUsageHint hint)
        {
            GL.GenBuffers(1, out handle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle);

            vertSizeInBytes = Marshal.SizeOf(typeof(T));
            IntPtr szPtr = new IntPtr(vertSizeInBytes * data.Length);

            GL.BufferData(BufferTarget.ArrayBuffer, szPtr, data, hint);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle);
        }

        public void ResetData(T[] data, int count)
        {
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, new IntPtr(vertSizeInBytes * count),
                data);
        }
    }
}
