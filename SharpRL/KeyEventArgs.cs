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

namespace SharpRL
{
    /// <summary>
    /// Event args that contains keyboard key information
    /// </summary>
    public class KeyEventArgs : EventArgs
    {
        /// <summary>
        /// The KeyCode of the key
        /// </summary>
        public KeyCode Key { get; set; }

        /// <summary>
        /// True if either of the ALT keys was pressed
        /// </summary>
        public bool Alt { get; set; }

        /// <summary>
        /// True if the left Ctrl key was pressed
        /// </summary>
        public bool LeftCtrl { get; set; }

        /// <summary>
        /// True if the right Ctrl key was pressed
        /// </summary>
        public bool RightCtrl { get; set; }

        /// <summary>
        /// True if either of the Shift keys was pressed
        /// </summary>
        public bool Shift { get; set; }
    }

    /// <summary>
    /// Event args that contain the ASCII code for a pressed key
    /// </summary>
    public class KeyCharEventArgs : EventArgs
    {
        /// <summary>
        /// The ASCII code of the pressed key
        /// </summary>
        public char Char { get; set; }
    }
}
