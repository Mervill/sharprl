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

namespace RLGui.Controls
{



    public class RadioBox : ListBox
    {
        protected bool RadioOnLeft { get; private set; }
        public char RadioSetChar { get; set; }
        public char RadioUnsetChar { get; set; }

        public RadioBox(Point position, RadioBoxTemplate template)
            : base(position, template)
        {
            RadioOnLeft = template.RadioOnLeft;
            RadioSetChar = template.RadioSetChar;
            RadioUnsetChar = template.RadioUnsetChar;

            if (CurrentSelectedIndex < 1)
                CurrentSelectedIndex = 1;
        }

        protected override void DrawContent()
        {
            Pigment pigment;

            for (int i = 0; i < Count; i++)
            {
                char ch;

                if (i == CurrentSelectedIndex)
                    ch = RadioSetChar;
                else
                    ch = RadioUnsetChar;

                if (i == CurrentSelectedIndex)
                    pigment = Pigments.ViewSelected;
                else if (i == CurrentHilightedIndex)
                    pigment = Pigments.ViewMouseOver;
                else
                    pigment = Pigments.ViewNormal;

                string label;

                if (RadioOnLeft)
                    label = string.Format("{0} {1}", ch, this[i].Label);
                else
                    label = string.Format("{0} {1}", this[i].Label, ch);

                DrawingSurface.PrintStringAligned(ClientRect.X, ClientRect.Y + i,
                    label, ClientRect.Width, HAlign, pigment.Foreground, pigment.Background);
            }
        }

    }
}
