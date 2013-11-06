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
    public class EntryValidationEventArgs : EventArgs
    {
        public EntryValidationEventArgs(string text)
        {
            this.Text = text;
            IsValid = true;
        }

        public string Text { get; set; }

        public bool IsValid { get; set; }
    }

    [Flags]
    public enum CharValidationFlags
    {
        Letters = 1,
        Numbers = 2,
        Symbols = 4,
        Space,
        All = Letters | Numbers | Symbols
    }



    public class TextEntry : Control
    {
        /// <summary>
        /// Triggered when the entered text has been committed, usually by pressing the ENTER key.
        /// </summary>
        public event EventHandler EntryChanged;

        public event EventHandler<EntryValidationEventArgs> ValidateCharacter;
        public event EventHandler<EntryValidationEventArgs> ValidateField;

        private TimerCollection timers;

        private bool cursorOn = true;
        private bool waitingToOverwrite;

        protected CharValidationFlags CharValidation { get; set; }

        const uint blinkDelay = 500;

        public TextEntry(Point position, TextEntryTemplate template)
            : base(position, template)
        {
            if (IsValidText(template.InitialText))
            {
                CurrentText = template.InitialText;
                ValidatedText = template.InitialText;
            }

            CharValidation = template.ValidChars;
            MaximumCharacters = template.NumberOfCharacters;

            ReplaceOnFirstKey = template.ReplaceOnFirstKey;
            CursorChar = template.CursorChar;

            KeyboardMode = KeyboardInputMode.Focus;

            timers = new TimerCollection();
            timers.AddTimer(0.5f, true,
                (t) =>
                {
                    cursorOn = !cursorOn;
                });
        }

        /// <summary>
        /// Gets the last valid, committed text that has been entered.  This may or may not be
        /// the same as what is being currently displayed by the entry
        /// </summary>
        public string ValidatedText { get; protected set; }


        /// <summary>
        /// Get or set the alignment of the label within the entry region
        /// </summary>
        public HorizontalAlignment HAlignment { get; set; }

        /// <summary>
        /// If true, simulates the "select-all and replace on first keypress" behaviour
        /// seen in other GUI systems.  Defaults to false.
        /// </summary>
        protected bool ReplaceOnFirstKey { get; set; }

        /// <summary>
        /// The current text state of the control and what is drawn.  This text has not yet been
        /// validated.
        /// </summary>
        protected string CurrentText { get; set; }

        /// <summary>
        /// Get the maximum number of characters that can be typed
        /// </summary>
        protected int MaximumCharacters { get; set; }

        /// <summary>
        /// Get the current position of the entry cursor, representing the position
        /// of the next typed character.
        /// </summary>
        protected int CursorPos { get; private set; }

        /// <summary>
        /// The character used to draw at the cursor position
        /// </summary>
        protected char CursorChar { get; set; }

        /// <summary>
        /// Return true if character is a valid entry.  An invalid character will be ignored
        /// by the entry and not added to the entry field.  Override to implement custom character
        /// validation.
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        protected virtual bool IsValidChar(char character)
        {
            if (ValidateCharacter != null)
            {
                EntryValidationEventArgs args = new EntryValidationEventArgs(character.ToString());

                ValidateCharacter(this, args);

                if (!args.IsValid)
                    return false;
            }

            if (CharValidation.HasFlag(CharValidationFlags.Letters))
            {
                if (char.IsLetter(character))
                    return true;
            }

            if (CharValidation.HasFlag(CharValidationFlags.Space))
            {
                if (character == ' ')
                    return true;
            }

            if (CharValidation.HasFlag(CharValidationFlags.Numbers))
            {
                if (char.IsNumber(character))
                    return true;
            }

            if (CharValidation.HasFlag(CharValidationFlags.Symbols))
            {
                if ("`~!@#$|^&*()_+-={};:'\",<.>/?".Contains(character))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if the provided entry is valid.  This is checked when the field is about
        /// to be committed; if invalid, the field will revert to the last valid field.  Override to implement
        /// custom field validation. This method will only be called by TryCommit if the current text
        /// is less than or equal the maximum number of characters.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        protected virtual bool IsValidText(string entry)
        {
            if (ValidateField != null)
            {
                EntryValidationEventArgs args = new EntryValidationEventArgs(entry);

                ValidateField(this, args);

                if (!args.IsValid)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Called when some text has been validated and committed to the entry.
        /// </summary>
        protected virtual void OnFieldChanged()
        {
            if (EntryChanged != null)
            {
                EntryChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Trys to commmit the current text, by calling ValidateField.  If successful,
        /// the CurrentText will be set to the current text, and OnFieldChanged will
        /// be called.
        /// </summary>
        /// <returns></returns>
        public virtual bool TryCommit()
        {
            if (this.CurrentText == this.ValidatedText)
                return false;

            if (CurrentText.Length <= MaximumCharacters)
            {
                if (IsValidText(CurrentText))
                {
                    ValidatedText = CurrentText;

                    OnFieldChanged();
                    return true;
                }
            }
            CurrentText = ValidatedText;
            return false;
        }

        protected internal override void OnUpdate(float elapsed)
        {
            base.OnUpdate(elapsed);
            timers.Update(elapsed);
        }

        protected override void DrawContent()
        {
            DrawingSurface.Clear();

            Pigment pigment;
            if (waitingToOverwrite)
            {
                pigment = Pigments.ViewSelected;
            }
            else
            {
                pigment = Pigments.ViewNormal;
            }

            DrawingSurface.PrintStringAligned(ViewRect.X, ViewRect.Y, CurrentText,
                ViewRect.Width, HAlignment,
                pigment.Foreground,pigment.Background);

            if (cursorOn && HasKeyboardFocus)
            {
                //Pigment curPigment = pigment.Invert();

                DrawingSurface.PrintChar(ViewRect.X + CursorPos, ViewRect.Y,
                    CursorChar,
                    pigment.Foreground, pigment.Background);

            }
        }

        protected internal override void OnKeyChar(KeyCharEventData keyInfo)
        {
            base.OnKeyChar(keyInfo);


            if (IsValidChar(keyInfo.Character))
            {
                if (waitingToOverwrite)
                {
                    CurrentText = keyInfo.Character.ToString();
                    CursorPos = 1;
                    waitingToOverwrite = false;
                }
                else if (CurrentText.Length < MaximumCharacters)
                {
                    CurrentText += keyInfo.Character;
                    CursorPos++;
                }
            }
        }

        protected internal override void OnKeyDown(KeyRawEventData keyInfo)
        {
            base.OnKeyDown(keyInfo);

            if (keyInfo.Key == KeyCode.Backspace &&
                CurrentText.Length > 0)
            {
                CurrentText = CurrentText.Substring(0, CurrentText.Length - 1);
                CursorPos--;
            }
            else if (keyInfo.Key == KeyCode.Enter)
            {
                ReleaseKeyboardFocus();     // This will lead to OnFocusReleased being called

            }
            else if (keyInfo.Key == KeyCode.Escape)
            {
                CurrentText = ValidatedText;
                ReleaseKeyboardFocus();
            }
        }

        protected internal override void OnFocusTaken()
        {
            base.OnFocusTaken();

            CurrentText = ValidatedText;

            if (ReplaceOnFirstKey)
            {
                waitingToOverwrite = true;
            }

            this.CursorPos = CurrentText.Length;
        }

        protected internal override void OnFocusReleased()
        {
            base.OnFocusReleased();
            
            TryCommit();

            waitingToOverwrite = false;
        }
    }
}
