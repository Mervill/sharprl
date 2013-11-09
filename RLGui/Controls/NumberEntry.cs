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


    public class NumberEntry : TextEntry
    {
        protected int MinValue { get; set; }

        protected int MaxValue { get; set; }



        public int CurrentValue { get; private set; }

        public NumberEntry(Point position, NumberEntryTemplate template)
            : base(position, template)
        {
            template.InitialText = template.InitialValue.ToString();

            if (IsValidText(template.InitialText))
            {
                CurrentText = template.InitialText;
                ValidatedText = template.InitialText;
            }
            else
            {
                CurrentText = template.MinimumValue.ToString();
                ValidatedText = CurrentText;
            }

            MinValue = template.MinimumValue;
            MaxValue = template.MaximumValue;


            MaximumCharacters = ClientRect.Width;
            CharValidation = RLGui.Controls.CharValidationFlags.Numbers;
        }

        protected override bool IsValidChar(char character)
        {
            bool valid = base.IsValidChar(character);

            if (!valid)
            {
                if (character == '-' && MinValue < 0)
                    valid = true;
            }

            return valid;
        }

        protected override bool IsValidText(string entry)
        {
            bool valid = base.IsValidText(entry);

            if (valid)
            {
                int val;

                if (!int.TryParse(entry, out val))
                {
                    return false;
                }

                if (val < MinValue || val > MaxValue)
                    return false;

                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool TryCommit()
        {
            if (base.TryCommit())
            {
                CurrentValue = int.Parse(ValidatedText);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
