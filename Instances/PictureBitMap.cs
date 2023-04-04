using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using static System.Console;
using System.IO;

using Utilitary;
using static Visuals.ConsoleVisuals;
using static Language.LanguageDictonary;

namespace Instances;

/// <summary> This class represents an image. </summary>
public class PictureBitMap 
{
    #region Picture content
    ///<summary> Information about the image. </summary>
    private byte[] info;
    ///<summary> The data of the pixels from the current image. </summary>                    
    private byte[] data;
    #endregion

    #region Fields
    /// <summary> Represents the path of the first image. </summary>
    public static string? s_WorkingImagePath;
    /// <summary> Represents the path of the final image. </summary>
    public static string? s_ResultImagePath;
    /// <summary> Represents the stopwatch used to measure the time of the execution of the filter. </summary>
    public static Stopwatch sw = new ();
    #endregion

    #region Indexers
    /// <summary> Gets the size of the image. </summary>
    private uint numberOfBytes => ConvertTo.UInt(info, 2);
    /// <summary> Gets the beginning of the image cont. </summary>
    private uint firstByte => ConvertTo.UInt(info, 10);
    /// <summary> Gets the color depth of the image. </summary>
    private ushort colorDepth => ConvertTo.UShort(info, 28);
    /// <summary> Stride is the number of bytes per line of pixels which must be a 4-factor. </summary>
    private int stride => (GetLength(0) * colorDepth / 8 + 3) / 4 * 4; 
    #endregion

    #region Dimensions
    /// <summary> Gets the dimension of the image. </summary>
    /// <param name="dimension"> The dimension to get. </param>
    /// <returns> The dimension of the image. </returns>
    public int GetLength(int dimension) => dimension switch
    {
        // Width
        0 => ConvertTo.Int(info, 18),
        // Height
        1 => ConvertTo.Int(info, 22),
        // default
        _ => throw new ArgumentOutOfRangeException(nameof(dimension), dimension, "Dimension must be 0 or 1")
    };
    #endregion 

    #region Constructors
    /// <summary> Creates an <see cref="PictureBitMap"/> instance from a referenced file by <paramref name="filePath"/>. </summary>
    /// <param name="filePath"> Image path to open. </param>
    public PictureBitMap(string? filePath)
    {
        if (filePath is null) 
            throw new ArgumentNullException(nameof(filePath));

        using (FileStream stream = File.OpenRead(filePath))
        {
            info = new byte[54];
            for (int i = 0; i < 54; i++) 
                info[i] = (byte)stream.ReadByte();

            data = new byte[(int)(numberOfBytes - firstByte)];
            for (int i = 0; i < (int)(numberOfBytes - firstByte); i++) 
                data[i] = (byte)stream.ReadByte();
        }

        if (info is null) 
            throw new ArgumentNullException(nameof(info));
        if (data is null) 
            throw new ArgumentNullException(nameof(data));
    }
    /// <summary> Creates an <see cref="PictureBitMap"/> instance from an height <paramref name="height"/> and a width <paramref name="width"/>. <br/>The image is automatically filled in black (components set to 0).</summary>
    /// <param name="width">Image width.</param>
    /// <param name="height">Image height.</param>
    public PictureBitMap(int width, int height)
    {
        if (width < 1 || height < 1) 
            throw new ArgumentOutOfRangeException("Width and height must be positive");
            
        info = new byte[54];
        for (int i = 0; i < Encoding.ASCII.GetBytes("BM").Length; i++) 
            info[i] = Encoding.ASCII.GetBytes("BM")[i];
        for (int i = 0; i < ConvertTo.LittleEndian((uint)info.Length).Length; i++) 
            info[10 + i] = ConvertTo.LittleEndian((uint)info.Length)[i];
        for (int i = 0; i < ConvertTo.LittleEndian(40).Length; i++) 
            info[14 + i] = ConvertTo.LittleEndian(40)[i];
        for (int i = 0; i < ConvertTo.LittleEndian(width).Length; i++) 
            info[18 + i] = ConvertTo.LittleEndian(width)[i];
        for (int i = 0; i < ConvertTo.LittleEndian(height).Length; i++) 
            info[22 + i] = ConvertTo.LittleEndian(height)[i];
        for (int i = 0; i < ConvertTo.LittleEndian((ushort)1).Length; i++) 
            info[26 + i] = ConvertTo.LittleEndian((ushort)1)[i];
        for (int i = 0; i < ConvertTo.LittleEndian((ushort)24).Length; i++) 
            info[28 + i] = ConvertTo.LittleEndian((ushort)24)[i];

        data = new byte[height * stride];
        for (int i = 0; i < ConvertTo.LittleEndian((uint)(info.Length + data.Length)).Length; i++) 
            info[2 + i] = ConvertTo.LittleEndian((uint)(info.Length + data.Length))[i];
    }
    #endregion

    #region Alter colors
    /// <summary> Represents the possible transformations of the image. </summary>
    public enum Transformations
    {
        /// <summary> Transforms the image to shades of grey. </summary>
        Grey,
        /// <summary> Transforms the image to black and white. </summary>
        BnW,
        /// <summary> Transforms the image to negative. </summary>
        Negative
    }
    /// <summary> Applies a transformation to the image. </summary>
    /// <param name="alter"> The transformation to apply. </param>
    /// <returns> The transformed image. </returns>
    public PictureBitMap AlterColors(Transformations alter)
    {
        PictureBitMap newImage = Copy();
        for (int x = 0; x < this.GetLength(0); x++) 
            for (int y = 0; y < this.GetLength(1); y++) 
                switch (alter)
                {
                    case Transformations.Grey:
                        newImage[x, y] = this[x, y].GreyAverage();
                        break;
                    case Transformations.BnW:
                        newImage[x, y] = this[x, y].GreyAverage().Red > 127 ? new Pixel(255, 255, 255) : new Pixel(0, 0, 0);
                        break;
                    case Transformations.Negative:
                        newImage[x, y] = new Pixel((byte)(255 - this[x, y].Red), (byte)(255 - this[x, y].Green), (byte)(255 - this[x, y].Blue));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(alter), alter, null);
                } 
        return newImage;
    }
    #endregion

    #region Manipulations
    /// <summary> Rotates the <see cref="PictureBitMap"/> instance at an <paramref name="degreesAngle"/>.</summary>
    /// <param name="degreesAngle">Angle of the rotation (in degrees)</param>
    /// <returns>A copy of this <see cref="PictureBitMap"/> rotated by <paramref name="degreesAngle"/> degrees.</returns>
    public PictureBitMap Rotation(int degreesAngle)
    {
        PictureBitMap previousImage = Copy();

        double radiansAngle = degreesAngle * (double)Math.PI / 180;
        double cos = (double)Math.Cos(radiansAngle);
        double sin = (double)Math.Sin(radiansAngle);

        int newWidth = (int)(GetLength(0) * Math.Abs(cos) + GetLength(1) * Math.Abs(sin));
        int newHeight = (int)(GetLength(0) * Math.Abs(sin) + GetLength(1) * Math.Abs(cos));

        PictureBitMap newImage = new (newWidth, newHeight);

        for (int x = 0; x < newWidth; x++)
            for (int y = 0; y < newHeight; y++)
            {
                double newX = (x - newWidth / 2) * cos - (y - newHeight / 2) * sin + GetLength(0) / 2;
                double newY = (x - newWidth / 2) * sin + (y - newHeight / 2) * cos + GetLength(1) / 2;

                if (newX >= 0 && newX < GetLength(0) && newY >= 0 && newY < GetLength(1)) 
                    newImage[x, y] = this[(int)newX, (int)newY];
            }
        return newImage;
    }
    /// <summary> This method scales the <see cref="PictureBitMap"/> instance by a <paramref name="scale"/> factor. </summary>
    /// <param name="scale"> Scale factor.</param>
    /// <returns> A copy of this <see cref="PictureBitMap"/> scaled by <paramref name="scale"/> factor.</returns>
    public PictureBitMap Resize(float scale)
    {
        PictureBitMap previousImage = Copy();
            
        int newWidth = (int)(GetLength(0) * scale);
        int newHeight = (int)(GetLength(1) * scale);

        PictureBitMap newImage = new (newWidth, newHeight);

        for (int x = 0; x < newWidth; x++)
            for (int y = 0; y < newHeight; y++)
                newImage[x, y] = new (previousImage[(int)(x / scale), (int)(y / scale)]);
        return newImage;
    }
    #endregion

    #region Steganography
    /// <summary> This method hides an <paramref name="imageToHide"/> inside this <see cref="PictureBitMap"/> instance. </summary>
    /// <param name="imageToHide"> Image to hide inside this <see cref="PictureBitMap"/> instance.</param>
    ///<returns> A copy of this <see cref="PictureBitMap"/> with the <paramref name="imageToHide"/> hidden inside.</returns>
    public PictureBitMap HideImageInside(PictureBitMap imageToHide)
    {
        PictureBitMap result = Copy();
        if (this.GetLength(0) < imageToHide.GetLength(0) || this.GetLength(1) < imageToHide.GetLength(1))
        {
            float scaleX = imageToHide.GetLength(0) / this.GetLength(0);
            float scaleY = imageToHide.GetLength(1) / this.GetLength(1);
            float scale = Math.Max(scaleX, scaleY);
            result = result.Resize(scale);
        }
        for(int x = 0; x < result.GetLength(0); x++)
        {
            for (int y = 0; y < result.GetLength(1); y++)
            {
                Pixel toHide = new (0, 0, 0);
                if(x < imageToHide.GetLength(0) && y < imageToHide.GetLength(1))
                {
                    toHide = imageToHide[x, y];
                }
                Pixel pixel = result[x, y];
                byte r = (byte)((pixel.Red & 0b11110000) + ((toHide.Red >> 4) & 0b00001111));
                byte g = (byte)((pixel.Green & 0b11110000) + ((toHide.Green >> 4) & 0b00001111));
                byte b = (byte)((pixel.Blue & 0b11110000) + ((toHide.Blue >> 4) & 0b00001111));
                result[x, y] = new Pixel(r, g, b);
            }
        }
        return result;
    }
    /// <summary> This method is used to get the hidden image inside this <see cref="PictureBitMap"/>. </summary>
    /// <returns> The hidden image inside this <see cref="PictureBitMap"/>. </returns>
    public PictureBitMap GetHiddenImage()
    {
        PictureBitMap result = this.Copy();
        for (int x = 0; x < GetLength(0); x++)
        {
            for (int y = 0; y < GetLength(1); y++)
            {
                Pixel pixel = this[x, y];
                result[x, y] = new Pixel((byte)(pixel.Red << 4), (byte)(pixel.Green << 4), (byte)(pixel.Blue << 4));
            }
        }
        return result;
    }
    #endregion
    
    #region Utility methods
    /// <summary> This method is used to get a copy of this <see cref="PictureBitMap"/>. </summary>
    /// <returns> A copy of this <see cref="PictureBitMap"/>. </returns>
    public PictureBitMap Copy() 
    {
        PictureBitMap newImage = new (GetLength(0), GetLength(1));
        newImage.info = info;
        newImage.data = data;
        return newImage;
    }
    /// <summary> This method is used to get the <see cref="Pixel"/> at the specified <paramref name="x"/> and <paramref name="y"/> coordinates. </summary>
    /// <param name="x"> The x coordinate of the <see cref="Pixel"/>. </param>
    /// <param name="y"> The y coordinate of the <see cref="Pixel"/>. </param>
    /// <returns> The position at the specified <paramref name="x"/> and <paramref name="y"/> coordinates. </returns>
    private int pos(int x, int y) => x * 3 + (GetLength(1) - y - 1) * stride;
    /// <summary> This method is used to get the <see cref="Pixel"/> at the specified <paramref name="x"/> and <paramref name="y"/> coordinates. </summary>
    public Pixel this[int x, int y]
    {
        get
        {
            int position = pos(x, y);
            return new (data[position + 2], data[position + 1], data[position + 0]);
        }
        set
        {
            int position = pos(x, y);
            data[position + 2] = value.Red;
            data[position + 1] = value.Green;
            data[position + 0] = value.Blue;
        }
    }
    /// <summary> This method saves the <see cref="PictureBitMap"/>. </summary>
    public void Save()
    {
        sw.Stop();
        s_ResultImagePath = "Images/OUT/bmp/" + WritePrompt(s_Dict[s_Lang]["prompt"]["save"]) + ".bmp";
        sw.Start();
        using (FileStream stream = File.OpenWrite(s_ResultImagePath))
        {
            stream.Write(info, 0, info.Length);
            stream.Write(data, 0, data.Length);
        }
        sw.Stop();
        WriteParagraph(new string[] {
            s_Dict[s_Lang]["title"]["save1"]  + s_ResultImagePath + " ", 
            s_Dict[s_Lang]["title"]["save2"] + sw.ElapsedMilliseconds + " ms. "}, true);
        sw.Reset();
        DisplayImage();
    }
    /// <summary> This method is used to print the <see cref="PictureBitMap"/>. </summary>
    public static void DisplayImage()
    {
        switch(ScrollingMenu(s_Dict[s_Lang]["title"]["display_action"] , new string[]{
            s_Dict[s_Lang]["generic"]["no"], 
            s_Dict[s_Lang]["generic"]["yes"]}, false, CursorTop + 2))
        {
            case 1:
                break;
            default:
                if (s_ResultImagePath is not null)
                {
                    s_WorkingImagePath = null;
                    s_ResultImagePath = null;
                }
                return;
        }
        if (s_WorkingImagePath is not null)
            Display(s_WorkingImagePath);
        if (s_ResultImagePath is not null)
            Display(s_ResultImagePath);
        if(s_WorkingImagePath is null && s_ResultImagePath is null)
            throw new NullReferenceException("The image path is null.");
            
        WriteParagraph(new string[] {
            s_Dict[s_Lang]["title"]["display_waiting1"], 
            s_Dict[s_Lang]["title"]["display_waiting2"]  }, true);
        ReadKey(true);
        if (s_ResultImagePath is not null)
        {
            s_WorkingImagePath = null;
            s_ResultImagePath = null;
        }

        void Display(string path)
        {

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    Process.Start(path);
                    break;
                case PlatformID.Unix:
                    if (isVsCodeRunning())
                        Process.Start("/Applications/Visual Studio Code.app/Contents/Resources/app/bin/code", path);
                    else
                        Process.Start("open", path);
                    break;
                default:
                    throw new PlatformNotSupportedException();
            }
        }
        bool isVsCodeRunning()
        {
            bool running = false;
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "/bin/bash",
                Arguments = "-c \"pgrep 'Code Helper'\"",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            process.StartInfo = startInfo;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(output))
            {
                running = true;
            }

            return running;
        }
    }
    /// <summary> This method is used to convert the <see cref="PictureBitMap"/> to a JPEG. </summary>
    
    /// <summary> This method is used to compress the <see cref="PictureBitMap"/>. </summary>
    public static void WorkingCompression(string path)
    {
        //throw new NotImplementedException();

        using (var image = Image.Load("Images/Default/lena.bmp"))
        {
            using (var outputStream = new MemoryStream())
            {
                image.Save(outputStream, new JpegEncoder());
                File.WriteAllBytes("Images/OUT/jpg/lena2.jpg", outputStream.ToArray());
            }
        }
        string imagePath = "Images/OUT/jpg/lena2.jpg"; //peut être ne pas laisser le même path pour les deux ^^'
        string compressedImagePath = "Images/OUT/jpg/lena2.jpg";
        int quality = 50;

        using (Stream inputStream = File.OpenRead(imagePath))
        using (Stream outputStream = File.OpenWrite(compressedImagePath))
        {
            using (var image = Image.Load(inputStream))
            {
                image.Mutate(x => x.Resize(image.Width, image.Height));
                var encoder = new JpegEncoder{Quality = quality};
                image.Save(outputStream, encoder);
            }
        }
        
    }

    #endregion
}