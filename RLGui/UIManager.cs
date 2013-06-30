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
using System.Text;
using SharpRL;
using System.Drawing;

namespace RLGui
{
    /// <summary>
    /// This object hooks into the GameConsole and manages sending input and system messages to its MainComponent
    /// </summary>
    public class UIManager
    {
        List<Component> components = new List<Component>();

        internal GameConsole Console { get; private set; }

        /// <summary>
        /// The number of milliseconds of mouse cursor rest time after which a MouseHover message will be sent
        /// </summary>
        public static float HoverTimeMilli = 500f;

        /// <summary>
        /// Constructs a UIManager object given the GameConsole
        /// </summary>
        /// <param name="console"></param>
        public UIManager(GameConsole console)
        {
            console.KeyChar += new EventHandler<EventArgs<KeyCharEventData>>(console_KeyChar);
            console.KeyDown += new EventHandler<EventArgs<KeyRawEventData>>(console_KeyDown);
            console.KeyUp += new EventHandler<EventArgs<KeyRawEventData>>(console_KeyUp);
            console.MouseButtonDown += new EventHandler<EventArgs<MouseEventData>>(console_MouseButtonDown);
            console.MouseButtonUp += new EventHandler<EventArgs<MouseEventData>>(console_MouseButtonUp);
            console.MouseMove += new EventHandler<EventArgs<MouseEventData>>(console_MouseMove);
            console.Drawing += new EventHandler<EventArgs<float>>(console_Drawing);

            Console = console;
        }

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
                    components.Add(component);
                    component.manager = this;
                }
            }
        }

        /// <summary>
        /// Adds the specified components above the given component
        /// </summary>
        /// <param name="insertAbove"></param>
        /// <param name="comps"></param>
        public void InsertComponentsAbove(Component insertAbove, params Component[] comps)
        {
            if (!components.Contains(insertAbove))
                throw new ArgumentException("The component insertAbove does not exist in the collection");

            int index = components.IndexOf(insertAbove) + 1;

            foreach (var component in comps)
            {
                if (!components.Contains(component))
                {
                    components.Insert(index, component);
                    index++;
                    component.manager = this;
                }
            }
        }

        /// <summary>
        /// Adds the specified components below the given component
        /// </summary>
        /// <param name="insertBelow"></param>
        /// <param name="comps"></param>
        public void InsertComponentsBelow(Component insertBelow, params Component[] comps)
        {
            if (!components.Contains(insertBelow))
                throw new ArgumentException("The component insertBelow does not exist in the collection");

            int index = components.IndexOf(insertBelow);

            foreach (var component in comps)
            {
                if (!components.Contains(component))
                {
                    components.Insert(index, component);
                    index++;
                    component.manager = this;
                }
            }
        }

        /// <summary>
        /// Moves the specified component above the other one
        /// </summary>
        /// <param name="componentToMove"></param>
        /// <param name="aboveComponent"></param>
        public void SendComponentAbove(Component componentToMove, Component aboveComponent)
        {
            if (!components.Contains(componentToMove))
                throw new ArgumentException("The componentToMove does not exist in the collection");
            if (!components.Contains(aboveComponent))
                throw new ArgumentException("The aboveComponent does not exist in the collection");

            components.Remove(componentToMove);
            InsertComponentsAbove(aboveComponent, componentToMove);
        }

        /// <summary>
        /// Moves the specified component below the other one
        /// </summary>
        /// <param name="componentToMove"></param>
        /// <param name="belowComponent"></param>
        public void SendComponentBelow(Component componentToMove, Component belowComponent)
        {
            if (!components.Contains(componentToMove))
                throw new ArgumentException("The componentToMove does not exist in the collection");
            if (!components.Contains(belowComponent))
                throw new ArgumentException("The belowComponent does not exist in the collection");

            components.Remove(componentToMove);
            InsertComponentsBelow(belowComponent, componentToMove);
        }

        /// <summary>
        /// Moves the specified component to the top
        /// </summary>
        /// <param name="componentToMove"></param>
        public void SendComponentToTop(Component componentToMove)
        {
            if (!components.Contains(componentToMove))
                throw new ArgumentException("The componentToMove does not exist in the collection");

            components.Remove(componentToMove);
            AddComponents(componentToMove);

        }

        /// <summary>
        /// Moves the specified component to the bottom
        /// </summary>
        /// <param name="componentToMove"></param>
        public void SendComponentToBottom(Component componentToMove)
        {
            if (!components.Contains(componentToMove))
                throw new ArgumentException("The componentToMove does not exist in the collection");

            components.Remove(componentToMove);
            components.Insert(0, componentToMove);
        }

        /// <summary>
        /// Remove the specified component from the collection
        /// </summary>
        /// <param name="component"></param>
        public void RemoveComponent(Component component)
        {
            if (components.Remove(component))
            {
                component.manager = null;
            }
        }


        void console_Drawing(object sender, EventArgs<float> e)
        {
            Console.Root.Clear();

            foreach (var currComponent in components)
            {
                currComponent.OnUpdate(e.Value);
                currComponent.OnPaint();
                currComponent.OnRender(Console.Root);
                
                
                hoverTime += e.Value;

                if (!isHovering && hoverTime >= HoverTimeMilli)
                {
                    isHovering = true;
                    if (lastMouseMoveData != null && currComponent.HitTest(lastMouseMoveData.ConsoleLocation))
                    {
                        currComponent.OnHoverBegin(new MouseMessageData(lastMouseMoveData, 
                            currComponent.ConsoleToLocalSpace(lastMouseMoveData.ConsoleLocation)));
                    }
                }
            }
        }

        Component lastMouseOver;
        float hoverTime;
        bool isHovering;
        MouseEventData lastMouseMoveData;


        Component GetTopComponentAt(Point pos)
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

        void console_MouseMove(object sender, EventArgs<MouseEventData> e)
        {
            lastMouseMoveData = e.Value;

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


        Component currentKBFocus;

        internal void RequestKBTake(Component comp)
        {
            if (currentKBFocus != null)
                currentKBFocus.HasKeyboardFocus = false;

            comp.HasKeyboardFocus = true;
        }

        internal void RequestKBRelease(Component comp)
        {
            currentKBFocus = null;
            comp.HasKeyboardFocus = false;
        }


        void console_KeyUp(object sender, EventArgs<KeyRawEventData> e)
        {
            foreach (var comp in components)
            {
                if (comp.KeyboardMode == KeyboardInputMode.Always || comp == currentKBFocus)
                {
                    comp.OnKeyUp(e.Value);
                }
            }
        }

        void console_KeyDown(object sender, EventArgs<KeyRawEventData> e)
        {
            foreach (var comp in components)
            {
                if (comp.KeyboardMode == KeyboardInputMode.Always || comp == currentKBFocus)
                {
                    comp.OnKeyDown(e.Value);
                }
            }
        }

        void console_KeyChar(object sender, EventArgs<KeyCharEventData> e)
        {
            foreach (var comp in components)
            {
                if (comp.KeyboardMode == KeyboardInputMode.Always || comp == currentKBFocus)
                {
                    comp.OnKeyChar(e.Value);
                }
            }
        }
    }
}
