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

namespace RLGui
{
    public class ItemData
    {
        public ItemData(string label, string tooltip)
            : this(label, tooltip, null)
        { }

        public ItemData(string label)
            : this(label, null, null)
        { }

        public ItemData(string label, string tooltip, object userData)
        {
            Label = label;
            Tooltip = tooltip;
            UserData = userData;
        }

        public string Label { get; set; }

        public string Tooltip { get; set; }

        public Object UserData { get; set; }
    }

    public class ListBoxTemplate : ControlTemplate
    {
        public ListBoxTemplate()
        {
            InitialSelected = -1;
            Items = new List<ItemData>();
            HAlign = HorizontalAlignment.Left;
        }

        public int InitialSelected { get; set; }

        public List<ItemData> Items { get; private set; }

        public HorizontalAlignment HAlign { get; set; }

        public override Size CalcSizeToContent()
        {
            int width, height;

            height = Items.Count;
            width = 1;

            foreach (var itm in Items)
            {
                if (itm.Label.Length > width)
                    width = itm.Label.Length;
            }


            if (HasFrame)
            {
                width += 2;
                height += 2;
            }

            return new Size(width, height);
        }
    }



    public class ListBox : Control
    {
        private List<ItemData> items;

        public ListBox(Point position, ListBoxTemplate template)
            :base(position, template)
        {
            items = template.Items;
            HAlign = template.HAlign;

            CurrentSelected = template.InitialSelected;

            if (CurrentSelected < 0 || CurrentSelected >= items.Count)
                CurrentSelected = -1;
        }

        public HorizontalAlignment HAlign { get; set; }

        public int CurrentSelected { get; private set; }

        public ItemData this[int index]
        {
            get
            {
                return this.items[index];
            }
        }

        protected override void DrawContent()
        {
            throw new System.NotImplementedException();
        }
    }
}
