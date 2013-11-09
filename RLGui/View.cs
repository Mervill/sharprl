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


    public class View
    {

        public View(Rectangle viewport, Surface renderTo, ToolTip toolTip = null)
        {

            this.viewPort = viewport;
            drawingSurface = new MemorySurface(viewport.Width, viewport.Height);
            this.renderTo = renderTo;

            HoverRestTime = 1f;

            if (toolTip == null)
                this.ToolTip = new DefaultToolTip();
            else
                this.ToolTip = toolTip;

            ToolTip.Manager = this;

            coordDelta = new Size(-viewport.X, -viewport.Y);

            components = new ComponentCollection(this);
        }


        /// <summary>
        /// The number of seconds of mouse cursor rest time after which a MouseHover message is sent
        /// </summary>
        public float HoverRestTime { get; set; }

        public ToolTip ToolTip { get; private set; }

        public Rectangle ViewPort { get { return viewPort; } }

        /// <summary>
        /// Adds one or more components to the top of the component collection.
        /// Components are only added if they do not already exist in the collection.
        /// </summary>
        /// <param name="comps"></param>
        public void AddComponents(params Component[] comps)
        {
            foreach(var component in comps)
            {
                components.Add(component);
            }
        }

        /// <summary>
        /// Remove the specified component from the collection
        /// </summary>
        /// <param name="component"></param>
        public void RemoveComponent(Component component)
        {
            components.Remove(component);
        }

        /// <summary>
        /// Returns the topmost component at the given position (in console space)
        /// If there are no components at that position, then this method returns null
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Component GetTopComponentAt(Point pos)
        {
            Component over = null;

            IEnumerable<Component> sorted = components.GetSorted();

            foreach(var curr in sorted.Reverse())
            {
                if (curr.HitTest(pos))
                {
                    over = curr;
                    break;
                }
            }

            return over;
        }


        ComponentCollection components;
        Component lastMouseOver;
        float hoverTime;
        bool isHovering;
        MouseEventData lastMouseMoveData;
        Component currentFocus;
        bool ToolTipShowing = false;
        internal MemorySurface drawingSurface;
        Surface renderTo;
        Rectangle viewPort;
        Size coordDelta;

        internal void ComponentLayerChanged()
        {
            components.NeedsSorting = true;
        }

        internal void ComponentAdded(Component comp)
        {
            comp.view = this;
            comp.OnOpening();
        }

        internal void ComponentRemoved(Component comp)
        {
            comp.view = null;

            if (lastMouseOver == comp)
                lastMouseOver = null;

            if (currentFocus == comp)
                currentFocus = null;
        }

        internal void RequestTakeFocus(Component comp)
        {
            if (currentFocus != null)
            {
                currentFocus.HasFocus = false;
                currentFocus.OnFocusReleased();
            }

            comp.HasFocus = true;
            currentFocus = comp;
            comp.OnFocusTaken();
        }

        internal void RequestReleaseFocus(Component comp)
        {
            comp.OnFocusReleased();

            currentFocus = null;
            comp.HasFocus = false;
        }


        private void StartToolTip(Control control)
        {
            ToolTip.StartTooltip(control.ToolTipText, lastMouseMoveData.ConsoleLocation);
            ToolTipShowing = true;
        }

        private void EndToolTip()
        {
            ToolTip.EndTooltip();
            ToolTipShowing = false;
        }

        private MouseEventData TranslateToViewport(MouseEventData src)
        {
            return new MouseEventData(Point.Add(src.ConsoleLocation, coordDelta), src.PixelPosition, src.Button);
        }


        public void OnUpdate(float elapsed)
        {
            if (components.Count == 0)
                return;

            drawingSurface.Clear();

            var sorted = components.GetSorted();

            components.StartEnumeration();
            foreach (var comp in sorted)
            {

                comp.OnUpdate(elapsed);
                comp.OnRedraw();
                comp.OnRender(drawingSurface);
            }
            components.EndEnumeration();

            if (ToolTipShowing)
            {
                ToolTip.OnUpdate(elapsed);
                ToolTip.OnPaint();
                ToolTip.OnRender(drawingSurface);
            }

            hoverTime += elapsed;

            if (!isHovering && hoverTime >= HoverRestTime)
            {
                isHovering = true;

                if (lastMouseMoveData != null)
                {
                    var currComponent = GetTopComponentAt(lastMouseMoveData.ConsoleLocation);

                    if (currComponent != null)
                    {
                        currComponent.OnHoverBegin(new MouseMessageData(lastMouseMoveData,
                            currComponent.ConsoleToLocalSpace(lastMouseMoveData.ConsoleLocation)));

                        if (currComponent is Control)
                        {
                            Control control = currComponent as Control;

                            if (!string.IsNullOrEmpty(control.ToolTipText))
                                StartToolTip(control);
                        }
                    }
                }
            }

            Surface.Blit(drawingSurface, renderTo, viewPort.X, viewPort.Y);
        }

        public void OnMouseMove(MouseEventData mouse)
        {
            if (components.Count == 0)
                return;

            var mouseData = TranslateToViewport(mouse);

            lastMouseMoveData = mouseData;
            hoverTime = 0f;

            if (ToolTipShowing)
            {
                EndToolTip();
            }

            var currComponent = GetTopComponentAt(mouseData.ConsoleLocation);

            if (currComponent != null)
            {
                var childMouseInfo = new MouseMessageData(mouseData, 
                    currComponent.ConsoleToLocalSpace(mouseData.ConsoleLocation));

                if (currComponent != lastMouseOver)
                {
                    currComponent.OnMouseEnter(childMouseInfo);

                    if (lastMouseOver != null)
                    {
                        lastMouseOver.OnMouseLeave();
                    }

                    lastMouseOver = currComponent;
                }

                if (isHovering)
                {
                    isHovering = false;

                    currComponent.OnHoverEnd(childMouseInfo);
                }

                currComponent.OnMouseMove(childMouseInfo);
            }
            else if (lastMouseOver != null)
            {
                lastMouseOver.OnMouseLeave();

                lastMouseOver = null;
            }
        }

        public void OnMouseButtonUp(MouseEventData mouse)
        {
            if (components.Count == 0)
                return;

            var mouseData = TranslateToViewport(mouse);

            var currComponent = GetTopComponentAt(mouseData.ConsoleLocation);

            if(currComponent != null)
            {
                if (currComponent.HitTest(mouseData.ConsoleLocation))
                {
                    var childMouseInfo = new MouseMessageData(mouseData, 
                        currComponent.ConsoleToLocalSpace(mouseData.ConsoleLocation));
                    currComponent.OnMouseButtonUp(childMouseInfo);
                }
            }
        }

        public void OnMouseButtonDown(MouseEventData mouse)
        {
            if (components.Count == 0)
                return;

            var mouseData = TranslateToViewport(mouse);

            var currComponent = GetTopComponentAt(mouseData.ConsoleLocation);

            if (currComponent != null)
            {
                if (currComponent.HitTest(mouseData.ConsoleLocation))
                {
                    var childMouseInfo = new MouseMessageData(mouseData, 
                        currComponent.ConsoleToLocalSpace(mouseData.ConsoleLocation));
                    currComponent.OnMouseButtonDown(childMouseInfo);
                }
            }

            components.StartEnumeration();
            foreach (var comp in components)
            {
                if (comp != currComponent)
                {
                    comp.OnMouseClickOutside(mouse.Button);
                }
            }
            components.EndEnumeration();
        }
        
        public void OnKeyUp(KeyRawEventData kbData)
        {
            if (components.Count == 0)
                return;

            components.StartEnumeration();
            foreach (var comp in components)
            {
                if (comp.KeyboardMode == KeyboardInputMode.Always || comp == currentFocus)
                {
                    comp.OnKeyUp(kbData);
                }
            }
            components.EndEnumeration();
        }

        private Component GetNextFocus(Component after)
        {
            Component nextFocus = null;

            int start = components.IndexOf(after);
            int i = start + 1;
            if (i >= components.Count)
                i = 0;

            while (i != start)
            {
                if (components[i].CanHaveFocus)
                {
                    nextFocus = components[i];
                }

                i++;

                if (i >= components.Count)
                    i = 0;
            }

            return nextFocus;
        }

        public void OnKeyDown(KeyRawEventData kbData)
        {
            if (components.Count == 0)
                return;

            Component nextFocus = null;

            components.StartEnumeration();
            foreach(var comp in components)
            {
                if (comp == currentFocus)
                {
                    if (kbData.Key == KeyCode.Tab && comp.CaptureTabKey == false)
                    {
                        // find next to have focus
                        nextFocus = GetNextFocus(comp);

                        //int nextIndex = components.IndexOf(comp) + 1;
                        //if (nextIndex >= components.Count)
                        //    nextIndex = 0;

                        //nextFocus = components[nextIndex];
                    }
                    else
                    {
                        comp.OnKeyDown(kbData);
                    }
                }
                else if (comp.KeyboardMode == KeyboardInputMode.Always || comp == currentFocus)
                {
                    comp.OnKeyDown(kbData);
                }
            }
            components.EndEnumeration();

            if (kbData.Key == KeyCode.Tab && currentFocus == null)
            {
                nextFocus = GetNextFocus(components[0]);
            }

            if (nextFocus != null)
                nextFocus.TakeFocus();
            //else if (kbData.Key == KeyCode.Tab) // tab key pushed but no one had current focus, set set focus to first component
            //{
            //    components[0].TakeFocus();
            //}
        }

        public void OnKeyChar(KeyCharEventData kbData)
        {
            if (components.Count == 0)
                return;

            components.StartEnumeration();
            foreach (var comp in components)
            {
                if (comp.KeyboardMode == KeyboardInputMode.Always || comp == currentFocus)
                {
                    comp.OnKeyChar(kbData);
                }
            }
            components.EndEnumeration();
        }
    }
}
