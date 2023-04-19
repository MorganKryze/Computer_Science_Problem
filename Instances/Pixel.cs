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
    /// <summary> This constructor creates a pixel with the specified <see cref ="int"/> values. </summary>
    public Pixel(int red, int green, int blue) : this((byte)red, (byte)green, (byte)blue) { }
    /// <summary> This constructor creates a pixel with the specified <see cref ="double"/> values. </summary>
    public Pixel(double red, double green, double blue) : this((byte)red, (byte)green, (byte)blue) { }
    
    /// <summary> This constructor creates a copy of the specified pixel. </summary>
    public Pixel(Pixel other)
    {
        Red = other.Red;
        Green = other.Green;
        Blue = other.Blue;
    }
    #endregion

    #region Methods
    /// <summary> This method returns the pixel in greyscale. </summary>
    /// <returns> A pixel in greyscale. </returns>
    public Pixel GreyAverage() => new ((byte)((Red + Green + Blue) / 3), (byte)((Red + Green + Blue) / 3), (byte)((Red + Green + Blue) / 3));
    /// <summary>
	/// Create a new RGB pixel from a hue, saturation and value (HSV standart)
	/// </summary>
	/// <param name="hue">Hue</param>
	/// <returns>A new RGB pixel from HSV</returns>
	
    
    public static Pixel Hue(int hue)
	{
		hue %= 360;
		double x = 1 * (1 - Math.Abs((hue / 60.0) % 2 - 1));
		double r_, g_, b_;
        
		     if(hue < 60) (r_, g_, b_) = (1, x, 0);
		else if(hue < 120) (r_, g_, b_) = (x, 1, 0);
		else if(hue < 180) (r_, g_, b_) = (0, 1, x);
		else if(hue < 240) (r_, g_, b_) = (0, x, 1);
		else if(hue < 300) (r_, g_, b_) = (x, 0, 1);
		else if(hue < 360) (r_, g_, b_) = (1, 0, x);
		else (r_, g_, b_) = (0, 0, 0);

		return new Pixel((byte)(b_ * 255), (byte)(g_ * 255), (byte)(r_ * 255));
	}
    
    #endregion

    #region Utility
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