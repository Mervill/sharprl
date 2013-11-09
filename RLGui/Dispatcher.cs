//Copyright (c) 2013 Shane Baker
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
using SharpRL;
using System.Drawing;
using RLGui.Controls;
using SharpRL.Toolkit;

namespace RLGui
{
    /// <summary>
    /// Provides automatic messages routing from the GameConsole to a View. Use this class
    /// when the order of message handling between views and the rest of the application
    /// doesn't need to specially managed.
    /// </summary>
    public class Dispatcher
    {
        public Dispatcher(GameConsole console, View view)
        {
            this.console = console;
            this.view = view;
        }


        public void Start()
        {
            if (isRunning)
                return;

            isRunning = true;

            console.KeyChar += new EventHandler<EventArgs<KeyCharEventData>>(console_KeyChar);
            console.KeyDown += new EventHandler<EventArgs<KeyRawEventData>>(console_KeyDown);
            console.KeyUp += new EventHandler<EventArgs<KeyRawEventData>>(console_KeyUp);
            console.MouseButtonDown += new EventHandler<EventArgs<MouseEventData>>(console_MouseButtonDown);
            console.MouseButtonUp += new EventHandler<EventArgs<MouseEventData>>(console_MouseButtonUp);
            console.MouseMove += new EventHandler<EventArgs<MouseEventData>>(console_MouseMove);
            console.Drawing += new EventHandler<EventArgs<float>>(console_Drawing);
        }

        public void Stop()
        {
            if (!isRunning)
                return;

            isRunning = false;

            console.KeyChar -= console_KeyChar;
            console.KeyDown -= console_KeyDown;
            console.KeyUp -= console_KeyUp;
            console.MouseButtonDown -= console_MouseButtonDown;
            console.MouseButtonUp -= console_MouseButtonUp;
            console.MouseMove -= console_MouseMove;
            console.Drawing -= console_Drawing;
        }



        bool isRunning;
        GameConsole console;
        View view;


        private void console_Drawing(object sender, EventArgs<float> e)
        {
            view.OnUpdate(e.Value);
        }

        private void console_MouseMove(object sender, EventArgs<MouseEventData> e)
        {
            view.OnMouseMove(e.Value);
        }

        private void console_MouseButtonUp(object sender, EventArgs<MouseEventData> e)
        {
            view.OnMouseButtonUp(e.Value);
        }

        private void console_MouseButtonDown(object sender, EventArgs<MouseEventData> e)
        {
            view.OnMouseButtonDown(e.Value);
        }

        private void console_KeyUp(object sender, EventArgs<KeyRawEventData> e)
        {
            view.OnKeyUp(e.Value);
        }

        private void console_KeyDown(object sender, EventArgs<KeyRawEventData> e)
        {
            view.OnKeyDown(e.Value);
        }

        private void console_KeyChar(object sender, EventArgs<KeyCharEventData> e)
        {
            view.OnKeyChar(e.Value);
        }






    }
}
