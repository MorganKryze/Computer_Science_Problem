using System.Text;
using Utilitary;

using static Visuals.ConsoleVisuals;

namespace Computer_Science_Problem;

/// <summary> This class represents an image. </summary>
public class Image : IEquatable<Image>
{
    #region Constants
    /// <summary> Represents the path of the image.</summary>
    public static string imagePath = "Images/";
    /// <summary> Represents the file type (bmp in this case). </summary>
    public const int type = 0;
    /// <summary> Represents the total file size (in bytes). </summary>
    public const int fileSize = 2;
    /// <summary> Position of the first pixel in the file (in bytes). </summary>
    public const int offsetFirstPixel = 10;
    /// <summary> Represents the size of the second part of the header </summary>    
    public const int infoHeaderSize = 14;
    /// <summary> Represents the image width (in pixels). </summary>
    public const int offsetWidth = 18;
    /// <summary> Represents the image height (in pixels). </summary>
    public const int offsetHeight = 22;
    /// <summary> Represents the number of color planes (always equals to 1 for a bmp). </summary>
    public const int offsetColorPlanes = 26;
    /// <summary> Represents the number of bits per pixel (often 24 because 3 bytes * 8 bits = 24). </summary>
    public const int offsetColorDepth = 28;
    #endregion

    #region Fields
    ///<summary> Creation of the array of bytes of the header.</summary>
    public byte[] Header;
    ///<summary> Creation of the array of bytes of the image.</summary>                            
    public byte[] Pixels;
    #endregion

    #region Properties
    /// <summary> Encoding.ASCII.GetString is a method which turn a bytes array into a string </summary>
    public string Type => Encoding.ASCII.GetString(Header.ExtractBytes(2, type));
    /// <summary> Utils.LittleEndianToUInt is a method which turn the information of the file size contained in a bytes array into an unsigned integer </summary>
    public uint FileSize => ConvertTo.UInt(Header, fileSize);
    /// <summary> Utils.LittleEndianToUInt is a method which turn the information of the start offset contained in a bytes array into an unsigned integer </summary>
    public uint StartOffset => ConvertTo.UInt(Header, offsetFirstPixel);
    /// <summary> Utils.LittleEndianToUInt is a method which turn the information of the info header size contained in a bytes array into an unsigned integer </summary>
    public uint InfoHeaderSize => ConvertTo.UInt(Header, infoHeaderSize);
    /// <summary> Utils.LittleEndianToInt is a method which turn the information of the width contained in a bytes array into an integer </summary>
    public int Width => ConvertTo.Int(Header, offsetWidth);
    /// <summary> Utils.LittleEndianToInt is a method which turn the information of the height contained in a bytes array into an integer </summary>
    public int Height => ConvertTo.Int(Header, offsetHeight);
    /// <summary> Utils.LittleEndianToUShort is a method which turn the information of the color planes contained in a bytes array into an unsigned short </summary>
    public ushort ColorPlanes => ConvertTo.UShort(Header, offsetColorPlanes);
    /// <summary> Utils.LittleEndianToUShort is a method which turn the information of the color depth contained in a bytes array into an unsigned short </summary>
    public ushort ColorDepth => ConvertTo.UShort(Header, offsetColorDepth);
    /// <summary> Stride is the number of bytes per line of pixels. </summary>
    public int Stride => (Width * ColorDepth / 8 + 3) / 4 * 4; 
    /// <summary> This method is used to get the <see cref="Pixel"/> at the specified <paramref name="x"/> and <paramref name="y"/> coordinates. </summary>
    /// <param name="x"> The x coordinate of the <see cref="Pixel"/>. </param>
    /// <param name="y"> The y coordinate of the <see cref="Pixel"/>. </param>
    /// <returns> The position at the specified <paramref name="x"/> and <paramref name="y"/> coordinates. </returns>
    private int _position(int x, int y) => x * 3 + (Height - y - 1) * Stride;
    #endregion

    #region Constructors
    /// <summary> Creates an <see cref="Image"/> instance from a referenced file by <paramref name="filename"/>. </summary>
    /// <param name="filename"> Image path to open. </param>
    public Image(string filename)
    {
        using (FileStream stream = File.OpenRead(filename))
        {
            Header = stream.ReadBytes(54);

            if (Type is not "BM") 
                throw new FormatException("Invalid file type. We can only load BMP files.");
            if (ColorDepth is not 24) 
                throw new FormatException("We cannot load images with a depth of 24 bits.");

            Pixels = stream.ReadBytes((int)(FileSize - StartOffset));
        }

        if (Header is null) 
            throw new ArgumentNullException(nameof(Header));
        if (Pixels is null) 
            throw new ArgumentNullException(nameof(Pixels));
    }
    /// <summary> Creates an <see cref="Image"/> instance from an height <paramref name="height"/> and a width <paramref name="width"/>. <br/>The image is automatically filled in black (components set to 0).</summary>
    /// <param name="width">Image width.</param>
    /// <param name="height">Image height.</param>
    public Image(int width, int height)
    {
        if (width < 1 || height < 1) 
            throw new ArgumentOutOfRangeException("Width and height must be positive");
            
        Header = new byte[54];
        Header.InsertBytes(Encoding.ASCII.GetBytes("BM"), type);
        Header.InsertBytes(ConvertTo.LittleEndian((uint)Header.Length), offsetFirstPixel);
        Header.InsertBytes(ConvertTo.LittleEndian(0x28), infoHeaderSize);
        Header.InsertBytes(ConvertTo.LittleEndian(width), offsetWidth);
        Header.InsertBytes(ConvertTo.LittleEndian(height), offsetHeight);
        Header.InsertBytes(ConvertTo.LittleEndian((ushort)1), offsetColorPlanes);
        Header.InsertBytes(ConvertTo.LittleEndian((ushort)24), offsetColorDepth);

        Pixels = new byte[height * Stride];

        Header.InsertBytes(ConvertTo.LittleEndian((uint)(Header.Length + Pixels.Length)), fileSize);
    }
    /// <summary> Creates a copy of an <see cref="Image"/> instance . </summary>
    /// <param name="original"><see cref="Image"/>to copy.</param>
    public Image(Image original)
    {
        if (original is null) throw new ArgumentNullException("Original");

        Header = new byte[original.Header.Length];
        Array.Copy(original.Header, Header, Header.Length);

        Pixels = new byte[original.Pixels.Length];
        Array.Copy(original.Pixels, Pixels, Pixels.Length);

    }
    #endregion

    #region Transformations
    /// <summary>Transform this <see cref="Image"/> to shades of grey (this method generates a copy).</summary>
    /// <returns> A greyscale copy of this <see cref="Image"/>.</returns>
    public Image TurnGrey()
    {
        Image result = this.Copy();

        for (int x = 0; x < Width; x++) 
            for (int y = 0; y < Height; y++) 
                result[x, y] = this[x, y].TurnGrey();
        return result;
    }
    /// <summary> Transform this <see cref="Image"/> to black and white (this method generate a copy). </summary>
    /// <returns> A black and white copy of this <see cref="Image"/>.</returns>
    public Image BlackAndWhite()
    {
        Image previousImage = this.Copy();

        for (int x = 0; x < Width; x++) 
            for (int y = 0; y < Height; y++) 
                previousImage[x, y] = this[x, y].TurnGrey().Red > 127 ? new Pixel(255, 255, 255) : new Pixel(0, 0, 0);
        return previousImage;
    }
    /// <summary> Rotates the <see cref="Image"/> instance at an <paramref name="angle"/>.</summary>
    /// <param name="angle">Angle of the rotation (in degrees)</param>
    /// <returns>A copy of this <see cref="Image"/> rotated by <paramref name="angle"/> degrees.</returns>
    public Image Rotate(int angle)
    {
        Image previousImage = this.Copy();

        double radians = angle * (double)Math.PI / 180;
        double cos = (double)Math.Cos(radians);
        double sin = (double)Math.Sin(radians);

        int newWidth = (int)(Width * Math.Abs(cos) + Height * Math.Abs(sin));
        int newHeight = (int)(Width * Math.Abs(sin) + Height * Math.Abs(cos));

        Image newImage = new (newWidth, newHeight);

        for (int x = 0; x < newWidth; x++)
        {
            for (int y = 0; y < newHeight; y++)
            {
                double newX = (x - newWidth / 2) * cos - (y - newHeight / 2) * sin + Width / 2;
                double newY = (x - newWidth / 2) * sin + (y - newHeight / 2) * cos + Height / 2;

                if (newX >= 0 && newX < Width && newY >= 0 && newY < Height) 
                    newImage[x, y] = this[(int)newX, (int)newY];
            }
        }
        return newImage;
    }
    /// <summary> This method scales the <see cref="Image"/> instance by a <paramref name="scale"/> factor. </summary>
    /// <param name="scale"> Scale factor.</param>
    /// <returns> A copy of this <see cref="Image"/> scaled by <paramref name="scale"/> factor.</returns>
    public Image Resize(float scale)
    {
        Image previousImage = this.Copy();

        if (scale == 1) 
            return previousImage;
            
        int newWidth = (int)(Width * scale);
        int newHeight = (int)(Height * scale);

        Image newImage = new (newWidth, newHeight);

        for (int x = 0; x < newWidth; x++)
            for (int y = 0; y < newHeight; y++)
                newImage[x, y] = new (previousImage[(int)(x / scale), (int)(y / scale)]);
        return newImage;
    }
    #endregion

    #region Operators
    /// <summary> The equality operator. </summary>
    /// <param name="a"> The first <see cref="Image"/> to compare. </param>
    /// <param name="b"> The second <see cref="Image"/> to compare. </param>
    /// <returns> True if the two <see cref="Image"/>s are equal, false otherwise. </returns>
    public static bool operator ==(Image a, Image b)
    {
        if (ReferenceEquals(a, b))
            return true;

        if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            return false;

        if (a.Width != b.Width || a.Height != b.Height)
            return false;

        for (int x = 0; x < a.Width; x++)
            for (int y = 0; y < a.Height; y++)
                if (a[x, y] != b[x, y])
                    return false;
        return true;
    }
    /// <summary> The inequality operator. </summary>
    /// <param name="a"> The first <see cref="Image"/> to compare. </param>
    /// <param name="b"> The second <see cref="Image"/> to compare. </param>
    /// <returns> True if the two <see cref="Image"/>s are not equal, false otherwise. </returns>
    public static bool operator !=(Image a, Image b) => !(a == b);
    /// <summary> This method is used to check if 2 <see cref="Image"/> are equal. </summary>
    /// <seealso cref="operator ==(Image, Image)"/>
    /// <returns>True if the 2 <see cref="Image"/> are equal, false otherwise.</returns>
    public override bool Equals(object? other) => other is Image && this == (Image)other;
    /// <summary> This method is used to check if 2 <see cref="Image"/> are equal. </summary>
    /// <seealso cref="operator ==(Image, Image)"/>
    /// <returns>True if the 2 <see cref="Image"/> are equal, false otherwise.</returns>
    public bool Equals(Image? other) => other is not null && this == other;
    /// <summary> This method is used to get the hash code of this <see cref="Image"/>. </summary>
    public override int GetHashCode() => base.GetHashCode();
    #endregion

    #region Methods
    /// <summary> This method is used to get a copy of this <see cref="Image"/>. </summary>
    /// <returns> A copy of this <see cref="Image"/>. </returns>
    public Image Copy() => new (this);
    /// <summary> This method is used to get the <see cref="Pixel"/> at the specified <paramref name="x"/> and <paramref name="y"/> coordinates. </summary>
    public Pixel this[int x, int y]
    {
        get
        {
            int position = _position(x, y);
            return new (Pixels[position + 2], Pixels[position + 1], Pixels[position + 0]);
        }
        set
        {
            int position = _position(x, y);
            Pixels[position + 2] = value.Red;
            Pixels[position + 1] = value.Green;
            Pixels[position + 0] = value.Blue;
        }
    }
    /// <summary> This method saves the <see cref="Image"/>. </summary>
    public void Save()
    {
        using (FileStream stream = File.OpenWrite("Images/OUT/" + WritePrompt("Type the name of the file.") + ".bmp"))
        {
            stream.Write(Header, 0, Header.Length);
            stream.Write(Pixels, 0, Pixels.Length);
        }
    }
    
    #endregion
}