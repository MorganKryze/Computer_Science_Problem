using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Computer_Science_Problem
{
    public class Pixel : IEquatable<Pixel>
    {
        #region Fields
        public byte Red;
        public byte Green;
        public byte Blue;
        #endregion

        #region Constructors
        public Pixel(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }
        public Pixel() : this(0, 0, 0) { }
        public Pixel(byte val) : this(val, val, val) { }
        public Pixel(Pixel original)
        {
            Red = original.Red;
            Green = original.Green;
            Blue = original.Blue;
        }
        #endregion

        #region Methods
        /// <summary> This method copies the current pixel. </summary>
        /// <returns> A copy of the current pixel. </returns>
        public Pixel Copy()
        {
            return new Pixel(this);
        }
        /// <summary> This method returns the pixel in greyscale. </summary>
        /// <returns> A pixel in greyscale. </returns>
        public Pixel Greyscale() => new Pixel((byte)((Red + Green + Blue) / 3)); // m�thode en 1 ligne
        /// <summary>This method returns a string representation of the pixel.</summary>
        /// <returns> A string representation of the pixel. </returns>
        public override string ToString() => $"({Red}, {Green}, {Blue})";
        #endregion

        #region Operators
        /// <summary>This method defines the equality between two pixels.</summary>
        /// <param name="a">The first pixel.</param>
        /// <param name="b">The second pixel.</param>
        public static bool operator ==(Pixel a, Pixel b) => a.Red == b.Red && a.Green == b.Green && a.Blue == b.Blue;
        /// <summary>This method defines the inequality between two pixels.</summary>
        /// <param name="a">The first pixel.</param>
        /// <param name="b">The second pixel.</param>
        public static bool operator !=(Pixel a, Pixel b) => !(a == b);
        /// <summary>This method also defines the equality between two pixels.</summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>True if the object is a pixel and is equal to the current pixel, false otherwise.</returns>
        public override bool Equals(object? other) => other is Pixel && this == (Pixel)other;
        ///<summary>This method gets the hash code.</summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() => base.GetHashCode();

        public bool Equals(Pixel? other)
        {
            return Equals(other);
        }
        #endregion
    }
}