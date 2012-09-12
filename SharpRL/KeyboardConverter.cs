//Copyright (c) 2012 Shane Baker
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
using OpenTK.Input;

namespace SharpRL
{
    static class KeyboardConverter
    {
        public static KeyEventArgs Convert(KeyboardKeyEventArgs sysArgs)
        {
            KeyEventArgs args = new KeyEventArgs();

            var ks = Keyboard.GetState();


            args.Alt = ks[Key.AltLeft] || ks[Key.AltRight];
            args.LeftCtrl = ks[Key.ControlLeft];
            args.RightCtrl = ks[Key.ControlRight];
            args.Shift = ks[Key.ShiftLeft] || ks[Key.ShiftRight];
            args.Key = keymap[sysArgs.Key];

            return args;
        }

        static Dictionary<Key, KeyCode> keymap = new Dictionary<Key, KeyCode>();

        static KeyboardConverter()
        {
            // function keys
            for (int i = 0; i < 12; i++)
            {
                keymap.Add((Key)(Key.F1 + i), (KeyCode)(KeyCode.F1 + i));
            }

            // digits
            for (int i = 0; i <= 9; i++)
            {
                keymap.Add((Key)(Key.Number0 + i), (KeyCode)(KeyCode.Number0 + i));
            }

            // letters
            for (int i = 0; i < 26; i++)
            {
                keymap.Add((Key)(Key.A + i), (KeyCode)(KeyCode.A + i));
            }

            keymap.Add(Key.AltLeft, KeyCode.Alt);
            keymap.Add(Key.AltRight, KeyCode.Alt);
            keymap.Add(Key.Back, KeyCode.Backspace);
            keymap.Add(Key.ControlLeft, KeyCode.Control);
            keymap.Add(Key.ControlRight, KeyCode.Control);
            keymap.Add(Key.ShiftLeft, KeyCode.Shift);
            keymap.Add(Key.ShiftRight, KeyCode.Shift);
            keymap.Add(Key.Delete, KeyCode.Delete);

            keymap.Add(Key.End, KeyCode.End);
            keymap.Add(Key.Enter, KeyCode.Enter);
            keymap.Add(Key.Escape, KeyCode.Escape);
            keymap.Add(Key.Home, KeyCode.Home);
            keymap.Add(Key.Insert, KeyCode.Insert);
            keymap.Add(Key.LWin, KeyCode.Lwin);
            keymap.Add(Key.NumLock, KeyCode.Numlock);
            keymap.Add(Key.PageDown, KeyCode.PageDown);
            keymap.Add(Key.PageUp, KeyCode.PageUp);
            keymap.Add(Key.Pause, KeyCode.Pause);
            keymap.Add(Key.PrintScreen, KeyCode.Printscreen);
            keymap.Add(Key.RWin, KeyCode.Rwin);
            keymap.Add(Key.ScrollLock, KeyCode.Scrolllock);
            keymap.Add(Key.Space, KeyCode.Space);
            keymap.Add(Key.Tab, KeyCode.Tab);

            keymap.Add(Key.Semicolon, KeyCode.Semicolon);
            keymap.Add(Key.Slash, KeyCode.Slash);
            keymap.Add(Key.Tilde, KeyCode.Tilde);
            keymap.Add(Key.BracketLeft, KeyCode.LeftBrace);
            keymap.Add(Key.BackSlash, KeyCode.Backslash);
            keymap.Add(Key.BracketRight, KeyCode.RightBrace);
            keymap.Add(Key.Quote, KeyCode.Quote);
            keymap.Add(Key.Plus, KeyCode.Add);
            keymap.Add(Key.Comma, KeyCode.Comma);
            keymap.Add(Key.Minus, KeyCode.Subtract);
            keymap.Add(Key.Period, KeyCode.Period);

            // Keypad
            for (int i = 0; i <= 9; i++)
            {
                keymap.Add((Key)(Key.Keypad0 + i), (KeyCode)(KeyCode.Keypad0 + i));
            }
            keymap.Add(Key.KeypadDecimal, KeyCode.KeypadDecimal);
            keymap.Add(Key.KeypadAdd, KeyCode.KeypadAdd);
            keymap.Add(Key.KeypadSubtract, KeyCode.KeypadSubtract);
            keymap.Add(Key.KeypadDivide, KeyCode.KeypadDivide);
            keymap.Add(Key.KeypadMultiply, KeyCode.KeypadMultiply);

            keymap.Add(Key.Left, KeyCode.Left);
            keymap.Add(Key.Right, KeyCode.Right);
            keymap.Add(Key.Down, KeyCode.Down);
            keymap.Add(Key.Up, KeyCode.Up);

        }
    }
}
