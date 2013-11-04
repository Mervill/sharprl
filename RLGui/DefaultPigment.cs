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
using SharpRL;
using System.Drawing;

namespace RLGui
{
    /// <summary>
    /// Pigments used by controls for drawing themselves
    /// </summary>
    public class ControlPigments
    {
        /// <summary>
        /// Creates a ControlPigments object, copying values from the given object
        /// The order in which states are checked for pigment are as follows: Selected, MouseOver, and Normal 
        /// </summary>
        /// <param name="from"></param>
        public ControlPigments(ControlPigments from)
        {
            if (from == null)
                throw new ArgumentNullException("from");

            BorderMouseOver = from.BorderMouseOver;
            BorderNormal = from.BorderNormal;
            BorderSelected = from.BorderSelected;

            ViewMouseOver = from.ViewMouseOver;
            ViewNormal = from.ViewNormal;
            ViewSelected = from.ViewSelected;
        }

        /// <summary>
        /// Construct a ControlPigments object
        /// </summary>
        public ControlPigments()
        {
            BorderMouseOver = Pigment.WhiteBlack;
            BorderNormal = Pigment.WhiteBlack;
            BorderSelected = Pigment.WhiteBlack;

            ViewMouseOver = Pigment.WhiteBlack;
            ViewNormal = Pigment.WhiteBlack;
            ViewSelected = Pigment.WhiteBlack;
        }

        /// <summary>
        /// The border pigment when the mouse is over the control
        /// </summary>
        public Pigment BorderMouseOver { get; set; }

        /// <summary>
        /// The border pigment when no other state applies
        /// </summary>
        public Pigment BorderNormal { get; set; }

        /// <summary>
        /// The border pigment when control is selected - usually in response to a left mouse
        /// button click or down action
        /// </summary>
        public Pigment BorderSelected { get; set; }

        /// <summary>
        /// The view pigment when the mouse is over the control
        /// </summary>
        public Pigment ViewMouseOver { get; set; }

        /// <summary>
        /// The view pigment when no other state applies
        /// </summary>
        public Pigment ViewNormal { get; set; }

        /// <summary>
        /// The view pigment when the control is selected, typically in response to a left
        /// mouse button click or push action
        /// </summary>
        public Pigment ViewSelected { get; set; }
    }


}
