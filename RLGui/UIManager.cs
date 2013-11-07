﻿//Copyright (c) 2013 Shane Baker
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
using System.Text;
using SharpRL;
using System.Drawing;
using RLGui.Controls;
using SharpRL.Toolkit;

namespace RLGui
{
    /// <summary>
    /// This object hooks into the GameConsole and manages sending input and system messages to its MainComponent
    /// </summary>
    public class UIManager
    {
        List<Component> components = new List<Component>();
        List<Component> sortedComps = new List<Component>();

        Component lastMouseOver;
        float hoverTime;
        bool isHovering;
        MouseEventData lastMouseMoveData;
        Component currentFocus;
        bool ToolTipShowing = false;
        


        /// <summary>
        /// Constructs a UIManager object given the GameConsole
        /// </summary>
        /// <param name="console"></param>
        public UIManager(GameConsole console, ToolTip toolTip = null)
        {
            console.KeyChar += new EventHandler<EventArgs<KeyCharEventData>>(console_KeyChar);
            console.KeyDown += new EventHandler<EventArgs<KeyRawEventData>>(console_KeyDown);
            console.KeyUp += new EventHandler<EventArgs<KeyRawEventData>>(console_KeyUp);
            console.MouseButtonDown += new EventHandler<EventArgs<MouseEventData>>(console_MouseButtonDown);
            console.MouseButtonUp += new EventHandler<EventArgs<MouseEventData>>(console_MouseButtonUp);
            console.MouseMove += new EventHandler<EventArgs<MouseEventData>>(console_MouseMove);
            console.Drawing += new EventHandler<EventArgs<float>>(console_Drawing);

            Console = console;
            HoverRestTime = 1f;

            if (toolTip == null)
                this.ToolTip = new DefaultToolTip();
            else
                this.ToolTip = toolTip;

            ToolTip.Manager = this;
        }

        /// <summary>
        /// The number of seconds of mouse cursor rest time after which a MouseHover message is sent
        /// </summary>
        public float HoverRestTime { get; set; }
        


        public ToolTip ToolTip { get; private set; }

        internal GameConsole Console { get; private set; }



        private void ComponentAdded(Component comp)
        {
            comp.manager = this;
            comp.OnOpening();
        }

        private bool isIterating = false;
        enum Command
        {
            Add,
            Remove
        }

        class CommandData
        {
            public Command command;
            public Component comp;

            public void DoCommand(UIManager manager)
            {
                switch (command)
                {
                    case Command.Add:
                        manager.components.Add(comp);
                        manager.sortedComps.Add(comp);
                        manager.ComponentAdded(comp);
                        break;

                    case Command.Remove:
                        manager.components.Remove(comp);
                        manager.sortedComps.Remove(comp);
                        comp.manager = null;

                        if (manager.lastMouseOver == comp)
                            manager.lastMouseOver = null;

                        if (manager.currentFocus == comp)
                            manager.currentFocus = null;
                        break;
                }
            }
        }

        Queue<CommandData> cmdQueue = new Queue<CommandData>();


        /// <summary>
        /// Adds one or more components to the top of the component collection.
        /// Components are only added if they do not already exist in the collection.
        /// </summary>
        /// <param name="comps"></param>
        public void AddComponents(params Component[] comps)
        {
            foreach(var component in comps)
            {
                if (!components.Contains(component))
                {
                    var cmd = new CommandData() { comp = component, command = Command.Add };

                    if (isIterating)
                    {
                        cmdQueue.Enqueue(cmd);
                    }
                    else
                    {
                        cmd.DoCommand(this);
                    }
                }
            }
        }

        /// <summary>
        /// Remove the specified component from the collection
        /// </summary>
        /// <param name="component"></param>
        public void RemoveComponent(Component component)
        {
            if (components.Contains(component))
            {
                var cmd = new CommandData() { command = Command.Remove, comp = component };

                if (isIterating)
                {
                    cmdQueue.Enqueue(cmd);
                }
                else
                {
                    cmd.DoCommand(this);
                }
                
            }
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
            for (int i = components.Count - 1; i >= 0; i--)
            {
                var curr = components[i];

                if (curr.HitTest(pos))
                {
                    over = curr;
                    break;
                }
            }

            return over;
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
        
        void console_Drawing(object sender, EventArgs<float> e)
        {
            Console.Root.Clear();

            sortedComps.Sort((c1, c2) => { return c1.Depth.CompareTo(c2.Depth); });

            foreach (var currComponent in sortedComps)
            {
                currComponent.OnUpdate(e.Value);
                currComponent.OnPaint();
                currComponent.OnRender(Console.Root);



            }

            if (ToolTipShowing)
            {
                ToolTip.OnUpdate(e.Value);
                ToolTip.OnPaint();
                ToolTip.OnRender(Console.Root);
            }

            hoverTime += e.Value;

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

                            if(!string.IsNullOrEmpty(control.ToolTipText))
                                StartToolTip(control);
                        }
                    }
                }
            }
        }

        void console_MouseMove(object sender, EventArgs<MouseEventData> e)
        {
            lastMouseMoveData = e.Value;
            hoverTime = 0f;

            if (ToolTipShowing)
            {
                EndToolTip();
            }

            var currComponent = GetTopComponentAt(e.Value.ConsoleLocation);

            if (currComponent != null)
            {
                var childMouseInfo = new MouseMessageData(e.Value, currComponent.ConsoleToLocalSpace(e.Value.ConsoleLocation));

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
            else if(lastMouseOver != null)
            {
                lastMouseOver.OnMouseLeave();

                lastMouseOver = null;
            }

        }

        void console_MouseButtonUp(object sender, EventArgs<MouseEventData> e)
        {
            var currComponent = GetTopComponentAt(e.Value.ConsoleLocation);

            if(currComponent != null)
            {
                if (currComponent.HitTest(e.Value.ConsoleLocation))
                {
                    var childMouseInfo = new MouseMessageData(e.Value, currComponent.ConsoleToLocalSpace(e.Value.ConsoleLocation));
                    currComponent.OnMouseButtonUp(childMouseInfo);
                }
            }
        }

        void console_MouseButtonDown(object sender, EventArgs<MouseEventData> e)
        {
            var currComponent = GetTopComponentAt(e.Value.ConsoleLocation);

            if (currComponent != null)
            {
                if (currComponent.HitTest(e.Value.ConsoleLocation))
                {
                    var childMouseInfo = new MouseMessageData(e.Value, currComponent.ConsoleToLocalSpace(e.Value.ConsoleLocation));
                    currComponent.OnMouseButtonDown(childMouseInfo);
                }
            }
        }
        
        void console_KeyUp(object sender, EventArgs<KeyRawEventData> e)
        {
            isIterating = true;
            for (int i = components.Count - 1; i >= 0; i--)
            {
                Component comp = components[i];
                if (comp.KeyboardMode == KeyboardInputMode.Always || comp == currentFocus)
                {
                    comp.OnKeyUp(e.Value);
                }
            }
            isIterating = false;
            while (cmdQueue.Count != 0)
                cmdQueue.Dequeue().DoCommand(this);
        }

        void console_KeyDown(object sender, EventArgs<KeyRawEventData> e)
        {
            isIterating = true;
            for (int i = components.Count - 1; i >= 0; i--)
            {
                Component comp = components[i];

                if (comp == currentFocus)
                {
                    if (e.Value.Key == KeyCode.Tab && comp.CaptureTabKey == false)
                    {
                        // next to have focus
                        int nextIndex = components.IndexOf(comp) + 1;
                        if (nextIndex >= components.Count)
                            nextIndex = 0;

                        Component next = components[nextIndex];
                        next.TakeFocus(); 
                    }
                    else
                    {
                        comp.OnKeyDown(e.Value);
                    }
                }
                else if (comp.KeyboardMode == KeyboardInputMode.Always || comp == currentFocus)
                {
                    comp.OnKeyDown(e.Value);
                }
            }
            isIterating = false;
            while (cmdQueue.Count != 0)
                cmdQueue.Dequeue().DoCommand(this);
        }

        void console_KeyChar(object sender, EventArgs<KeyCharEventData> e)
        {
            isIterating = true;
            for (int i = components.Count - 1; i >= 0; i--)
            {
                Component comp = components[i];
                if (comp.KeyboardMode == KeyboardInputMode.Always || comp == currentFocus)
                {
                    comp.OnKeyChar(e.Value);
                }
            }
            isIterating = false;
            while (cmdQueue.Count != 0)
                cmdQueue.Dequeue().DoCommand(this);
        }
    }
}
