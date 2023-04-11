namespace Instances;

/// <summary> This class represents a pixel. </summary>
public class Pixel : IEquatable<Pixel>
{
    #region Fields
    /// <summary> This field represents the red value of the pixel. </summary>
    public byte Red;
    /// <summary> This field represents the green value of the pixel. </summary>
    public byte Green;
    /// <summary> This field represents the blue value of the pixel. </summary>
    public byte Blue;
    #endregion

    #region Constructors
    /// <summary> This natural constructor creates a pixel with the specified values. </summary>
    public Pixel(byte red, byte green, byte blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
    }
    /// <summary> This constructor creates a copy of the specified pixel. </summary>
    public Pixel(Pixel previous)
    {
        Red = previous.Red;
        Green = previous.Green;
        Blue = previous.Blue;
    }
    #endregion

    #region Methods
    /// <summary> This method returns the pixel in greyscale. </summary>
    /// <returns> A pixel in greyscale. </returns>
    public Pixel GreyAverage() => new ((byte)((Red + Green + Blue) / 3), (byte)((Red + Green + Blue) / 3), (byte)((Red + Green + Blue) / 3));
    /// <summary>This method returns a string representation of the pixel.</summary>
    /// <returns> A string representation of the pixel. </returns>
    public override string ToString() => $"({Red}, {Green}, {Blue})";
    /// <summary>This method defines the equality between two pixels.</summary>
    /// <param name="other">The pixel to compare.</param>
    /// <returns>True if the pixels are equal, false otherwise.</returns>
    public bool Equals(Pixel? other)
    {
        if (other is null)
            return false;
        else
            return this.Red == other.Red && this.Green == other.Green && this.Blue == other.Blue;
    }
     /// <summary>This method also defines the equality between two pixels.</summary>
    /// <param name="other">The object to compare.</param>
    /// <returns>True if the object is a pixel and is equal to the current pixel, false otherwise.</returns>
    public override bool Equals(object? other) => other is Pixel && this.Red == ((Pixel)other).Red && this.Green == ((Pixel)other).Green && this.Blue == ((Pixel)other).Blue;
    ///<summary>This method gets the hash code.</summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode() => base.GetHashCode();
    #endregion
}