namespace Computer_Science_Problem;

/// <summary> This class represents a pixel. </summary>
public class Pixel : IEquatable<Pixel>
{
    #region Fields
    /// <summary> This field represents the red value of the pixel. </summary>
    public byte R;
    /// <summary> This field represents the green value of the pixel. </summary>
    public byte G;
    /// <summary> This field represents the blue value of the pixel. </summary>
    public byte B;
    #endregion

    #region Constructors
    /// <summary> This natural constructor creates a pixel with the specified values. </summary>
    public Pixel(byte red, byte green, byte blue)
    {
        R = red;
        G = green;
        B = blue;
    }
    /// <summary> This constructor creates a black pixel. </summary>
    public Pixel() : this(0, 0, 0) { }
    /// <summary> This constructor creates a copy of the specified pixel. </summary>
    public Pixel(Pixel original)
    {
        R = original.R;
        G = original.G;
        B = original.B;
    }
    #endregion

    #region Methods
    /// <summary> This method copies the current pixel. </summary>
    /// <returns> A copy of the current pixel. </returns>
    public Pixel Copy() => new Pixel(this);
    /// <summary> This method returns the pixel in greyscale. </summary>
    /// <returns> A pixel in greyscale. </returns>
    public Pixel Greyscale()
    {
        byte scale = (byte)((R + G + B) / 3);
        return new Pixel(scale, scale, scale);
    }
    /// <summary>This method returns a string representation of the pixel.</summary>
    /// <returns> A string representation of the pixel. </returns>
    public override string ToString() => $"({R}, {G}, {B})";
    #endregion

    #region Operators
    /// <summary>This method defines the equality between two pixels.</summary>
    /// <param name="a">The first pixel.</param>
    /// <param name="b">The second pixel.</param>
    public static bool operator ==(Pixel a, Pixel b) => a.R == b.R && a.G == b.G && a.B == b.B;
    /// <summary>This method defines the inequality between two pixels.</summary>
    /// <param name="a">The first pixel.</param>
    /// <param name="b">The second pixel.</param>
    public static bool operator !=(Pixel a, Pixel b) => !(a == b);
    /// <summary>This method also defines the equality between two pixels.</summary>
    /// <param name="other">The object to compare.</param>
    /// <returns>True if the object is a pixel and is equal to the current pixel, false otherwise.</returns>
    public override bool Equals(object? other) => other is Pixel && this == (Pixel)other;
    /// <summary>This method also defines the equality between two pixels.</summary>
    /// <param name="other">The pixel to compare.</param>
    /// <returns>True if the pixel is equal to the current pixel, false otherwise.</returns>
    public bool Equals(Pixel? other) => other is not null && this == other;
    ///<summary>This method gets the hash code.</summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode() => base.GetHashCode();
    #endregion
}
