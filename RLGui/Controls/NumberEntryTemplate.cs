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
using System.Drawing;
using SharpRL;
using SharpRL.Toolkit;

namespace RLGui.Controls
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// The following template base properties are ignored when constructing a NumberEntryBox:
    /// InitialText
    /// ValidChars
    /// </remarks>
    public class NumberEntryTemplate : TextEntryTemplate
    {

        public int MaximumValue { get; set; }
        public int MinimumValue { get; set; }
        public int InitialValue { get; set; }

        public override Size CalcSizeToContent()
        {
            int width = NumberOfCharacters;

            int minWidth = Math.Max(MaximumValue.ToString().Length, MinimumValue.ToString().Length);

            width = Math.Max(width, minWidth);

            int height = 1;

            if (HasFrame)
            {
                width += 2;
                height += 2;
            }

            return new Size(width, height);
        }
    }
}
