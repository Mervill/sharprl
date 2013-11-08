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



    public class ListItemEventArgs : EventArgs
    {
        public ListItemEventArgs(int index, ItemData item)
        {
            ItemIndex = index;
            Item = item;
        }

        public int ItemIndex { get; set; }

        public ItemData Item { get; set; }
    }

    public class ListBox : Control
    {
        private List<ItemData> items;

        public event EventHandler<ListItemEventArgs> SelectedItemChanged;
        public event EventHandler<ListItemEventArgs> HilightedItemChanged;
        
        public ListBox(Point position, ListBoxTemplate template)
            :base(position, template)
        {
            if (template.Items == null || template.Items.Count == 0)
                throw new ArgumentException("ListBoxTemplate.Items must contain one or more items");

            items = template.Items;
            HAlign = template.HAlign;

            CurrentSelected = template.InitialSelected;
            CurrentHilighted = -1;

            if (CurrentSelected < 0 || CurrentSelected >= items.Count)
                CurrentSelected = -1;
        }

        public HorizontalAlignment HAlign { get; set; }

        public int CurrentSelected { get; protected set; }

        public int CurrentHilighted { get; private set; }

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

        protected void DoChangeSelected(int index)
        {
            if (CurrentSelected == index)
                return;

            CurrentSelected = index;
            OnSelectedItemChanged();
        }

        protected void DoChangeHilighted(int index)
        {
            if (CurrentHilighted == index)
                return;


            CurrentHilighted = index;

            OnMouseOverItem();

        }

        protected virtual int GetItemIndexAt(Point localPos)
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

        protected internal override void OnKeyDown(KeyRawEventData keyInfo)
        {
            base.OnKeyDown(keyInfo);

            if (keyInfo.Key == KeyCode.Down)
            {
                int next = CurrentSelected + 1;
                if (next >= Count)
                {
                    next = 0;
                }
                DoChangeSelected(next);
            }
            else if (keyInfo.Key == KeyCode.Up)
            {
                int next = CurrentSelected - 1;
                if (next < 0)
                {
                    next = Count - 1;
                }
                DoChangeSelected(next);
            }
        }

        protected internal override void OnMouseMove(MouseMessageData mouseInfo)
        {
            base.OnMouseMove(mouseInfo);

            int index = GetItemIndexAt(mouseInfo.LocalPos);

            if (CurrentHilighted != index)
            {
                CurrentHilighted = index;

                OnMouseOverItem();
            }
        }

        protected virtual void OnMouseOverItem()
        {
            if (HilightedItemChanged != null)
                HilightedItemChanged(this, new ListItemEventArgs(CurrentHilighted, items[CurrentHilighted]));
        }

        protected internal override void OnMouseLeave()
        {
            base.OnMouseLeave();

            CurrentHilighted = -1;
        }

        protected override void OnPushed()
        {
            base.OnPushed();

            if (IsMouseOver)
            {
                int index = GetItemIndexAt(MousePosition);

                if (index != -1)
                {
                    DoChangeSelected(index);
                }
            }
        }

        protected virtual void OnSelectedItemChanged()
        {
            if (SelectedItemChanged != null)
                SelectedItemChanged(this, new ListItemEventArgs(CurrentSelected, items[CurrentSelected]));
        }

        public override string ToolTipText
        {
            get
            {
                if (CurrentHilighted != -1)
                {
                    return items[CurrentHilighted].Tooltip;
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
                else if (i == CurrentHilighted)
                    pigment = Pigments.ViewMouseOver;
                else
                    pigment = Pigments.ViewNormal;

                DrawingSurface.PrintStringAligned(ViewRect.X, ViewRect.Y + i,
                    items[i].Label, ViewRect.Width, HAlign, pigment.Foreground, pigment.Background);
            }
        }
    }
}
