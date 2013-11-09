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


    public class CheckButton : Button
    {
        public CheckButton(Point position, CheckButtonTemplate template)
            : base(position, template)
        {
            CheckedChar = template.CheckedChar;
            UnCheckedChar = template.UnCheckedChar;
        }

        public char CheckedChar { get; set; }

        public char UnCheckedChar { get; set; }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if (isChecked == value)
                    return;

                isChecked = value;
            }
        }

        protected override void OnPushReleased()
        {
            base.OnPushReleased();

            if (IsChecked)
                IsChecked = false;
            else
                IsChecked = true;
        }

        protected override void DrawContent()
        {
            var pigment = GetCurrentViewPigment();

            DrawingSurface.DefaultBackground = pigment.Background;
            DrawingSurface.DefaultForeground = pigment.Foreground;

            DrawingSurface.Clear();

            char checkChar;

            if (IsChecked)
                checkChar = CheckedChar;
            else
                checkChar = UnCheckedChar;

            string fullLabel = string.Format("{0} {1}", checkChar, Label);

            DrawingSurface.PrintStringRect(ClientRect, fullLabel, HAlignment, VAlignment);
        }
    }
}
