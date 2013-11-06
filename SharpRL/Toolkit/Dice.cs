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
using System.Linq;
using System.Text;

namespace SharpRL.Toolkit
{
    /// <summary>
    /// An immutable type representing a set of rpg dice along with a modifier.
    /// </summary>
    [Serializable]
    public struct Dice
    {
        readonly int ndice;
        readonly int nsides;
        readonly int mod;

        #region Constructors
        /// <summary>
        /// Construct a Dice instanced given the number of dice and number of sides for each die.
        /// </summary>
        /// <example>
        /// To create a Dice object representing two six-sided dice:
        /// <code>
        /// Dice d = new Dice(2,6);
        /// </code>
        /// </example>
        /// <param name="numberOfDice"></param>
        /// <param name="numberOfSides"></param>
        public Dice(int numberOfDice, int numberOfSides)
        {
            ndice = numberOfDice;
            nsides = numberOfSides;
            mod = 0;
        }

        /// <summary>
        /// Construct a Dice instance given the number of dice, number sides per die, and a
        /// positive or negative modifier.
        /// </summary>
        /// <example>To construct a Dice object representing one six-sided die that gets a +2 modifier
        /// (which would return a random range of 3 thru 8):
        /// <code>Dice d = new Dice(1,6,2);</code></example>
        /// <param name="numberOfDice"></param>
        /// <param name="numberOfSides"></param>
        /// <param name="Modifier"></param>
        public Dice(int numberOfDice, int numberOfSides, int Modifier)
        {
            ndice = numberOfDice;
            nsides = numberOfSides;
            mod = Modifier;
        }

        /// <summary>
        /// Coonstructs a Dice object from a string that follows common RPG conventions 
        /// when specifying dice.
        /// </summary>
        /// <example>To create a dice object representing two six sided dice with a -2 modifier:
        /// <code>Dice d = new Dice("2d6-2");</code></example>
        /// <param name="formatString">
        /// In the form of {n}["d"|"D"][s]{"+"|"-"m}, where n=number of dice (optional), s = number of sides,
        /// and m=modifier (optional)
        /// </param>
        public Dice(string formatString)
        {
            string ndiceString = "";
            string nsidesString = "";
            string modString = "";

            ndice = 0;
            nsides = 0;
            mod = 0;

            string[] tokens = formatString.Split(new char[] { 'd', 'D' });
            ndiceString = tokens[0];

            if (tokens.Length > 1)
            {
                tokens = tokens[1].Split(new char[] { '+', '-' });

                nsidesString = tokens[0];

                if (tokens.Length > 1)
                {
                    modString = tokens[1];
                }
            }

            int ns;
            if (int.TryParse(nsidesString, out ns))
            {
                ndice = 1;
                nsides = ns;
            }

            int nd;
            if (int.TryParse(ndiceString, out nd))
            {
                ndice = nd;
            }

            int m;
            if (int.TryParse(modString, out m))
            {
                mod = m;
                if (formatString.Contains("-"))
                    mod = -mod;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The number of dice this Dice object represents
        /// </summary>
        public int NumberOfDice
        {
            get { return ndice; }
        }

        /// <summary>
        /// The number of sides per each die.
        /// </summary>
        public int NumberOfSides
        {
            get { return nsides; }
        }

        /// <summary>
        /// The linear modifier of the random range obtained with this Dice object.
        /// </summary>
        public int Modifier
        {
            get { return mod; }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Check whether or not this object is equal to the other
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (this.GetType() != obj.GetType())
                return false;

            return Equals((Dice)obj);
        }

        /// <summary>
        /// Check whether or not this object is equal to the other
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public bool Equals(Dice d)
        {
            return (this.ndice == d.ndice && this.nsides == d.nsides);
        }

        /// <summary>
        /// Returns a hash code for this object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hash = 7;
            hash = hash * 13 + nsides.GetHashCode();
            hash = hash * 13 + ndice.GetHashCode();
            hash = hash * 13 + mod.GetHashCode();

            return hash;
        }

        /// <summary>
        /// Returns a friendly string representation of this object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            System.Text.StringBuilder ret = new System.Text.StringBuilder(
                string.Format("{0}d{1}", ndice, nsides));

            if (mod != 0)
            {
                if (mod > 0)
                {
                    ret.AppendFormat("+{0}", mod);
                }
                else
                {
                    ret.AppendFormat("-{0}", mod);
                }
            }

            return ret.ToString();

        }
        /// <summary>
        /// Check whether or not this object is equal to the other
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Dice left, Dice right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }

            return (left.Equals(right));
        }

        /// <summary>
        /// Check whether this object is not-equal to the other
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Dice left, Dice right)
        {
            return !(left == right);
        }
        #endregion

        /// <summary>
        /// A singe six-sided die
        /// </summary>
        public static Dice _1d6 = new Dice(1, 6);

        /// <summary>
        /// Two six sided dice added together
        /// </summary>
        public static Dice _2d6 = new Dice(2, 6);

        /// <summary>
        /// Three six-sided dice added together
        /// </summary>
        public static Dice _3d6 = new Dice(3, 6);

    }
}
