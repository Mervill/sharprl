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

namespace RLGui
{
    /// <summary>
    /// Determines when a component will receive keyboard input messages.
    /// </summary>
    public enum KeyboardInputMode
    {
        /// <summary>
        /// The component will never receive keyboard input
        /// </summary>
        Never,

        /// <summary>
        /// The component will receive keyboard input only when it has keyboard focus
        /// </summary>
        Focus,

        /// <summary>
        /// The keyboard will always receive keyboard input
        /// </summary>
        Always
    }

    /// <summary>
    /// Base class for user interface components.  A component receives messages from the UIManager, and represents a visual
    /// object having a position and size.
    /// Classes deriving from Component can handle system and input messages by overriding the appropriate virtual handler
    /// methods.
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        /// This method will be used to transform mouse location to local space when sending mouse input messages
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public abstract Point ConsoleToLocalSpace(Point pos);

        /// <summary>
        /// Construct a component
        /// </summary>
        protected Component()
        {

            KeyboardMode = KeyboardInputMode.Always;
        }

        internal UIManager manager;

        /// <summary>
        /// Returns true if the mouse is currently over this widget
        /// </summary>
        public bool IsMouseOver { get; private set; }

        /// <summary>
        /// Returns true if the left mouse button has been pressed and is currently down while over this widget
        /// </summary>
        public bool IsBeingPushed { get; private set; }

        /// <summary>
        /// The current mouse position in local space.  If the mouse cursor is outside of this widget's
        /// region, then this will be 0,0
        /// </summary>
        public Point MousePosition { get; private set; }

        /// <summary>
        /// Returns true if the mouse is over this widget and is in the hover state
        /// </summary>
        public bool IsHovering { get; private set; }

        /// <summary>
        /// Override to provide customized hit testing.  UIManager uses this to determine if mouse messages should be passed to
        /// this component.  The default base method checks to see if the point is insides this component's rectangle
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public abstract bool HitTest(Point pos);

        /// <summary>
        /// The current keyboard input mode, which determines when this component receives keyboard messages.
        /// </summary>
        public KeyboardInputMode KeyboardMode { get; protected set; }

        private bool hasKeyboardFocus;
        /// <summary>
        /// True if this component has keyboard focus.  This only has meaning if the KeyboardMode is set to Focus.
        /// </summary>
        public bool HasKeyboardFocus
        {
            get { return hasKeyboardFocus; }
            internal set
            {
                if (KeyboardMode != KeyboardInputMode.Focus)
                    return;

                if (hasKeyboardFocus == value)
                    return;

                hasKeyboardFocus = value;

                if (hasKeyboardFocus)
                    OnFocusTaken();
                else
                    OnFocusReleased();
            }
        }

        /// <summary>
        /// Take the keyboard focus.  Other components will loose their focus if any has it.  This method is only
        /// applicable if the KeyboardMode is set to Focus
        /// </summary>
        protected void TakeKeyboardFocus()
        {
            if (hasKeyboardFocus || KeyboardMode != KeyboardInputMode.Focus)
                return;

            manager.RequestKBTake(this);
        }

        /// <summary>
        /// Release the keyboard focus.  This method is only applicable if this component currently has focus
        /// and the KeyboardMode is set to Focus
        /// </summary>
        protected void ReleaseKeyboardFocus()
        {
            if (!hasKeyboardFocus || KeyboardMode != KeyboardInputMode.Focus)
                return;

            manager.RequestKBRelease(this);
        }

        /// <summary>
        /// Called when this component receives a raw keyboard key press message.
        /// Override to provide custom message handling code after calling base method
        /// </summary>
        /// <param name="keyInfo"></param>
        protected internal virtual void OnKeyDown(KeyRawEventData keyInfo)
        {
            if (KeyDown != null)
                KeyDown(this, new EventArgs<KeyRawEventData>(keyInfo));
        }

        /// <summary>
        /// Fired when a key has been pressed
        /// </summary>
        public event EventHandler<EventArgs<KeyRawEventData>> KeyDown;


        /// <summary>
        /// Called when this component receives a raw keyboard key released message
        /// Override to provide custom message handling code after calling base method
        /// </summary>
        /// <param name="keyInfo"></param>
        protected internal virtual void OnKeyUp(KeyRawEventData keyInfo)
        {
            if (KeyUp != null)
                KeyUp(this, new EventArgs<KeyRawEventData>(keyInfo));
        }

        /// <summary>
        /// Fired when a key has been released
        /// </summary>
        public event EventHandler<EventArgs<KeyRawEventData>> KeyUp;



        /// <summary>
        /// Called when this component receives a translated keypress message
        /// Override to provide custom message handling code after calling base method
        /// </summary>
        /// <param name="keyInfo"></param>
        protected internal virtual void OnKeyChar(KeyCharEventData keyInfo)
        {
            if (KeyChar != null)
                KeyChar(this, new EventArgs<KeyCharEventData>(keyInfo));
        }

        /// <summary>
        /// Fired when a key or series of keys has been pressed that have a character representation
        /// </summary>
        public event EventHandler<EventArgs<KeyCharEventData>> KeyChar;


        /// <summary>
        /// Called when the mouse pointer moves while over this component.
        /// Override to provide custom message handling code after calling base method
        /// </summary>
        /// <param name="mouseInfo"></param>
        protected internal virtual void OnMouseMove(MouseMessageData mouseInfo)
        {
            MousePosition = mouseInfo.LocalPos;

            if (MouseMove != null)
                MouseMove(this, new EventArgs<MouseMessageData>(mouseInfo));
        }

        /// <summary>
        /// Fired when the mouse has moved while over this component
        /// </summary>
        public event EventHandler<EventArgs<MouseMessageData>> MouseMove;



        /// <summary>
        /// Called when a mouse button has been pressed and the mouse pointer is over this component.
        /// Override to provide custom message handling code after calling base method
        /// </summary>
        /// <param name="mouseInfo"></param>
        protected internal virtual void OnMouseButtonDown(MouseMessageData mouseInfo)
        {
            if (mouseInfo.Button == MouseButton.Left)
            {
                IsBeingPushed = true;
            }

            if (MouseButtonDown != null)
                MouseButtonDown(this, new EventArgs<MouseMessageData>(mouseInfo));

            if (mouseInfo.Button == MouseButton.Left && KeyboardMode == KeyboardInputMode.Focus)
            {
                TakeKeyboardFocus();
            }
        }

        /// <summary>
        /// Fired when a mouse button has been pushed while over this component
        /// </summary>
        public event EventHandler<EventArgs<MouseMessageData>> MouseButtonDown;


        /// <summary>
        /// Called when a mouse button has been released and the mouse pointer is over this component.
        /// Override to provide custom message handling code after calling base method
        /// </summary>
        /// <param name="mouseInfo"></param>
        protected internal virtual void OnMouseButtonUp(MouseMessageData mouseInfo)
        {
            if (mouseInfo.Button == MouseButton.Left)
            {
                if (IsBeingPushed)
                {
                    IsBeingPushed = false;

                    OnClicked(mouseInfo);
                }
            }

            if (MouseButtonUp != null)
                MouseButtonUp(this, new EventArgs<MouseMessageData>(mouseInfo));
        }


        /// <summary>
        /// Fired when a mouse button has been released while over this component
        /// </summary>
        public event EventHandler<EventArgs<MouseMessageData>> MouseButtonUp;


        /// <summary>
        /// Called when the mouse pointer is at rest for a short time and enters a hover state.
        /// Override to provide custom message handling code after calling base method
        /// </summary>
        /// <param name="mouseInfo"></param>
        protected internal virtual void OnHoverBegin(MouseMessageData mouseInfo)
        {
            IsHovering = true;

            if (HoverBegin != null)
                HoverBegin(this, new EventArgs<MouseMessageData>(mouseInfo));
        }

        /// <summary>
        /// Fired when the mouse has entered the hover state while over this component
        /// </summary>
        public event EventHandler<EventArgs<MouseMessageData>> HoverBegin;


        /// <summary>
        /// Called when the mouse pointer moves after being in a hover state.
        /// Override to provide custom message handling code after calling base method
        /// </summary>
        /// <param name="mouseInfo"></param>
        protected internal virtual void OnHoverEnd(MouseMessageData mouseInfo)
        {
            IsHovering = false;

            if (HoverEnd != null)
                HoverEnd(this, new EventArgs<MouseMessageData>(mouseInfo));
        }

        /// <summary>
        /// Fired when the mouse has left the hover state while over this component
        /// </summary>
        public event EventHandler<EventArgs<MouseMessageData>> HoverEnd;




        /// <summary>
        /// Called when the mouse pointer enters this component's region.
        /// Override to provide custom message handling code after calling base method
        /// </summary>
        /// <param name="mouseInfo"></param>
        protected internal virtual void OnMouseEnter(MouseMessageData mouseInfo)
        {
            IsMouseOver = true;

            if (MouseEnter != null)
                MouseEnter(this, new EventArgs<MouseMessageData>(mouseInfo));
        }


        /// <summary>
        /// Fired when the mouse pointer has entered this component's region
        /// </summary>
        public event EventHandler<EventArgs<MouseMessageData>> MouseEnter;



        /// <summary>
        /// Called when the mouse pointer leaves this component's region.
        /// Override to provide custom message handling code after calling base method
        /// </summary>
        protected internal virtual void OnMouseLeave()
        {
            IsMouseOver = false;
            MousePosition = Point.Empty;

            if (MouseLeave != null)
                MouseLeave(this, null);
        }

        /// <summary>
        /// Fired when the mouse pointer has left this component's region
        /// </summary>
        public event EventHandler MouseLeave;




        /// <summary>
        /// Called when the keyboard focus has been taken by this component.
        /// Override to provide custom message handling code after calling base method
        /// </summary>
        protected internal virtual void OnFocusTaken()
        {
            if (FocusTaken != null)
                FocusTaken(this, null);
        }

        /// <summary>
        /// Fired when the keyboard focus has been taken by this component
        /// </summary>
        public event EventHandler FocusTaken;




        /// <summary>
        /// Called when the keyboard focus has been released by this component.
        /// Override to provide custom message handling code after calling base method
        /// </summary>
        protected internal virtual void OnFocusReleased()
        {
            if (FocusReleased != null)
                FocusReleased(this, null);
        }

        /// <summary>
        /// Fired when the keyboard focus has been released by this component
        /// </summary>
        public event EventHandler FocusReleased;




        /// <summary>
        /// Called when this component receives an Update system message.
        /// Override to provide custom message handling code after calling base method
        /// </summary>
        /// <param name="elapsed"></param>
        protected internal virtual void OnUpdate(float elapsed)
        {
            if (Update != null)
                Update(this, new EventArgs<float>(elapsed));
        }

        /// <summary>
        /// Called when this component is doing an update
        /// </summary>
        public event EventHandler<EventArgs<float>> Update;



        /// <summary>
        /// Called when this widget is ready for drawing.
        /// Override to provide custom drawing code after calling base method
        /// </summary>
        protected internal abstract void OnPaint();

        /// <summary>
        /// Called when this widget receives a Clicked message.
        /// Override to provide custom message handling code after calling base method
        /// </summary>
        /// <param name="mouseInfo"></param>
        protected virtual void OnClicked(MouseMessageData mouseInfo)
        {
            if (Clicked != null)
                Clicked(this, new EventArgs<MouseMessageData>(mouseInfo));
        }

        /// <summary>
        /// Fired when the left mouse button is pushed and released while over this component
        /// </summary>
        public event EventHandler<EventArgs<MouseMessageData>> Clicked;

        /// <summary>
        /// Called when this component should put its contents onto to the console
        /// </summary>
        protected internal abstract void OnRender(Surface renderTo);

    }





}
