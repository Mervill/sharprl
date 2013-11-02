using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpRL
{
    /// <summary>
    /// Defines the characters used to draw a frame border using Surface.DrawFrame()
    /// </summary>
    public class FrameDefinition
    {
        /// <summary>
        /// Construct a default FrameDefinition object
        /// </summary>
        public FrameDefinition()
        {
            HorizontalLine = (char)SpecialChar.HorizontalLine;
            VerticalLine = (char)SpecialChar.VerticalLine;

            CornerLowerLeft = (char)SpecialChar.SouthWestLine;
            CornerLowerRight = (char)SpecialChar.SouthEastLine;
            CornerUpperLeft = (char)SpecialChar.NorthWestLine;
            CornerUpperRight = (char)SpecialChar.NorthEastLine;
        }

        /// <summary>
        /// The characters used to draw the left and right side of a frame
        /// </summary>
        public char HorizontalLine { get; set; }

        /// <summary>
        /// The characters used to draw the top and bottom of a frame
        /// </summary>
        public char VerticalLine { get; set; }

        /// <summary>
        /// The character used to draw the upper left corner of a frame
        /// </summary>
        public char CornerUpperLeft { get; set; }

        /// <summary>
        /// The character used to draw the upper right corner of a frame
        /// </summary>
        public char CornerUpperRight { get; set; }

        /// <summary>
        /// The character used to draw the lower left corner of a frame
        /// </summary>
        public char CornerLowerLeft { get; set; }

        /// <summary>
        /// The character used to draw the lower right corner of a frame
        /// </summary>
        public char CornerLowerRight { get; set; }

    }
}
