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
    public static string? s_SourceImagePath;
    /// <summary> Represents the path of the final image. </summary>
    public string? s_ResultImagePath;
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
        PictureBitMap newImage = Dupplicate();
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
        PictureBitMap previousImage = Dupplicate();

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
        if(scale == 1)
            return Dupplicate();
        if (scale < 0) 
            throw new ArgumentException("Scale factor must be positive.");
        if (scale > 20) 
            throw new ArgumentException("Scale factor is too high.");

        PictureBitMap previousImage = Dupplicate();
            
        int newWidth = (int)(GetLength(0) * scale);
        int newHeight = (int)(GetLength(1) * scale);
        if (newWidth < 0 || newHeight < 0) 
            throw new ArgumentException("Scale factor is too low.");

        PictureBitMap newImage = new (newWidth, newHeight);

        for (int x = 0; x < newWidth; x++)
            for (int y = 0; y < newHeight; y++)
                newImage[x, y] = new (previousImage[(int)(x / scale), (int)(y / scale)]);
        return newImage;
    }
    #endregion

    #region Steganography
    /// <summary> Hides a <paramref name="guestPicture"/> in the <paramref name="hostPicture"/>. </summary>
    /// <param name="hostPicture"> The picture in which the <paramref name="guestPicture"/> will be hidden. </param>
    /// <param name="guestPicture"> The picture to hide in the <paramref name="hostPicture"/>. </param>
    /// <returns> The <paramref name="hostPicture"/> with the <paramref name="guestPicture"/> hidden in it. </returns>
    public static PictureBitMap PictureEncoding(PictureBitMap hostPicture, PictureBitMap guestPicture)
    {
        byte codeByte(byte host, byte guest)
        {
            return (byte)((host & 0b11110000) | ((guest >> 4) & 0b00001111));
            //string hostString = Convert.ToString(host, 2).PadLeft(8, '0').Substring(0, 4);
            //string guestString = Convert.ToString(guest, 2).PadLeft(8, '0').Substring(0, 4);
            
            //return Convert.ToByte(hostString + guestString, 2);
        }

        PictureBitMap composedPicture = hostPicture.Dupplicate();
        if (hostPicture.GetLength(0) == guestPicture.GetLength(0) && hostPicture.GetLength(1) == guestPicture.GetLength(1))
        {
            for (int x = 0; x < hostPicture.GetLength(0); x++)
            {
                for (int y = 0; y < hostPicture.GetLength(1); y++)
                {
                    Pixel host = hostPicture[x, y];
                    Pixel guest = guestPicture[x, y];
                    composedPicture[x, y] = new Pixel(
                        codeByte(host.Red, guest.Red),
                        codeByte(host.Green, guest.Green),
                        codeByte(host.Blue, guest.Blue));
                }
            }
        }
        composedPicture.Save();

        return composedPicture;
    }
    /// <summary> Reveals a <see cref="PictureBitMap"/>. </summary>
    /// <returns> The <see cref="PictureBitMap"/> hidden in the <see cref="PictureBitMap"/> instance. </returns>
    public PictureBitMap PictureDecoding()
    {
        byte decodeByte(byte composed)
        {   
            //string composedString = Convert.ToString(composed, 2).PadLeft(8, '0').Substring(0, 8);
            
            //return Convert.ToByte(composedString.Substring(4, 4).PadRight(8, '0'), 2);
            return (byte)((composed << 4) & 0b11110000);
        }   

        PictureBitMap revealedPicture = new (this.GetLength(0), this.GetLength(1));
        for (int x = 0; x < this.GetLength(0); x++)
        {
            for (int y = 0; y < this.GetLength(1); y++)
            {
                Pixel host = this[x, y];
                revealedPicture[x, y] = new Pixel(
                    decodeByte(host.Red),
                    decodeByte(host.Green),
                    decodeByte(host.Blue));
            }
        }
        revealedPicture.Save();

        return revealedPicture;
    }
    #endregion
    
    #region Utility methods
    /// <summary> This method is used to get a copy of this <see cref="PictureBitMap"/>. </summary>
    /// <returns> A copy of this <see cref="PictureBitMap"/>. </returns>
    public PictureBitMap Dupplicate() 
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
    public void Save(string savePath = "Images/OUT/bmp/")
    {
        sw.Stop();
        s_ResultImagePath = savePath + WritePrompt(s_Dict[s_Lang]["prompt"]["save"]) + ".bmp";
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
    public void DisplayImage()
    {
        switch(ScrollingMenu(s_Dict[s_Lang]["title"]["display_action"] , new string[]{
            s_Dict[s_Lang]["generic"]["no"], 
            s_Dict[s_Lang]["generic"]["yes"]}, false, CursorTop + 2))
        {
            case 0:
                break;
            case 1:
                if (s_SourceImagePath is not null)
                    Display(s_SourceImagePath);
                if (s_ResultImagePath is not null)
                    Display(s_ResultImagePath);
                if(s_SourceImagePath is null && s_ResultImagePath is null)
                    throw new NullReferenceException("The image path is null.");

                WriteParagraph(new string[] {
                    s_Dict[s_Lang]["title"]["display_waiting1"], 
                    s_Dict[s_Lang]["title"]["display_waiting2"]  }, true);
                ReadKey(true);
                break;
            default:
                break;
        }
        if (s_ResultImagePath is not null)
        {
            s_SourceImagePath = null;
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
        if (path is not null)
            throw new NotImplementedException();


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