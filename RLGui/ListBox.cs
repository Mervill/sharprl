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

    /// <summary>
    /// Data used by ListBox and MenuBox items.
    /// </summary>
    public class ItemData
    {
        /// <summary>
        /// Construct an ItemData object
        /// </summary>
        /// <param name="label"></param>
        /// <param name="tooltip"></param>
        public ItemData(string label, string tooltip)
            : this(label, tooltip, null)
        { }

        /// <summary>
        /// Construct an ItemData object
        /// </summary>
        /// <param name="label"></param>
        public ItemData(string label)
            : this(label, null, null)
        { }


        /// <summary>
        /// Construct an ItemData object
        /// </summary>
        /// <param name="label"></param>
        /// <param name="tooltip"></param>
        /// <param name="userData"></param>
        public ItemData(string label, string tooltip, object userData)
        {
            Label = label;
            Tooltip = tooltip;
            UserData = userData;
        }

        /// <summary>
        /// The text shown for the list item
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The Tooltip text shown for this item, or null or empty string for none.
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Contains any other special data attached to this list item
        /// </summary>
        public Object UserData { get; set; }
    }

    public class ListBoxTemplate : ControlTemplate
    {
        public ListBoxTemplate(IEnumerable<ItemData> items)
        {
            InitialSelected = -1;

            if (items != null)
                Items = new List<ItemData>(items);
            else
                Items = new List<ItemData>();

            HAlign = HorizontalAlignment.Left;

            Pigments = new ControlPigments()
            {
                ViewMouseOver = new Pigment(Color.Gold, Color.Black),
                ViewSelected = new Pigment(Color.Black, Color.White)
            };
        }

        public ListBoxTemplate()
            :this(null)
        {
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

    public class ListItemEventArgs : EventArgs
    {
        public ListItemEventArgs()
        { }

        public ListItemEventArgs(int index)
        {
            this.ItemIndex = index;
        }

        public int ItemIndex { get; set; }
    }

    public class ListBox : Control
    {
        private List<ItemData> items;

        public event EventHandler<ListItemEventArgs> SelectedItemChanged;
        public event EventHandler<ListItemEventArgs> MouseOverItemChanged;
        
        public ListBox(Point position, ListBoxTemplate template)
            :base(position, template)
        {
            items = template.Items;
            HAlign = template.HAlign;

            CurrentSelected = template.InitialSelected;
            CurrentMouseOver = -1;

            if (CurrentSelected < 0 || CurrentSelected >= items.Count)
                CurrentSelected = -1;
        }

        public HorizontalAlignment HAlign { get; set; }

        public int CurrentSelected { get; private set; }

        public int CurrentMouseOver { get; private set; }

        public ItemData this[int index]
        {
            get
            {
                return this.items[index];
            }
        }

        public int Count
        {
            get { return items.Count; }
        }

        protected int GetItemIndexAt(Point localPos)
        {
            if (ViewRect.Contains(localPos))
            {
                return localPos.Y - ViewRect.Y;
            }
            else
            {
                return -1;
            }
        }

        protected internal override void OnMouseMove(MouseMessageData mouseInfo)
        {
            base.OnMouseMove(mouseInfo);

            int index = GetItemIndexAt(mouseInfo.LocalPos);

            if (CurrentMouseOver != index)
            {
                CurrentMouseOver = index;
                if (MouseOverItemChanged != null)
                    MouseOverItemChanged(this, new ListItemEventArgs(index));
            }
        }

        protected internal override void OnMouseLeave()
        {
            base.OnMouseLeave();

            CurrentMouseOver = -1;
        }

        protected internal override void OnMouseButtonDown(MouseMessageData mouseInfo)
        {
            base.OnMouseButtonDown(mouseInfo);

            if (mouseInfo.Button == MouseButton.Left)
            {
                int index = GetItemIndexAt(mouseInfo.LocalPos);

                if (index != -1)
                {
                    if (CurrentSelected != index)
                    {
                        CurrentSelected = index;

                        if (SelectedItemChanged != null)
                            SelectedItemChanged(this, new ListItemEventArgs(index));
                    }
                }
            }
        }

        public override string ToolTipText
        {
            get
            {
                if (CurrentMouseOver != -1)
                {
                    return items[CurrentMouseOver].Tooltip;
                }
                else
                {
                    return base.ToolTipText;
                }
            }
        }

        protected override void DrawContent()
        {
            Pigment pigment;

            for (int i = 0; i < Count; i++)
            {
                if (i == CurrentSelected)
                    pigment = Pigments.ViewSelected;
                else if (i == CurrentMouseOver)
                    pigment = Pigments.ViewMouseOver;
                else
                    pigment = Pigments.ViewNormal;

                DrawingSurface.PrintStringAligned(ViewRect.X, ViewRect.Y + i,
                    items[i].Label, ViewRect.Width, HAlign, pigment.Foreground, pigment.Background);
            }
        }
    }
}
