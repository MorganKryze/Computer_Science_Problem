﻿using System.Diagnostics;
using System.Text;
using System.Text.Json;
using SixLabors.ImageSharp.Formats.Jpeg;
using static System.Console;

using Utilitary;
using static Visuals.ConsoleVisuals;
using static Computer_Science_Problem.GameManager;

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
    private string? imagePath;
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
        imagePath = filePath;
    }
    /// <summary> Creates an <see cref="PictureBitMap"/> instance from an height <paramref name="height"/> and a width <paramref name="width"/>. <br/>The image is automatically filled in black (components set to 0).</summary>
    /// <param name="width">Image width.</param>
    /// <param name="height">Image height.</param>
    public PictureBitMap(int width, int height)
    {
        if (width < 0 || height < 0) 
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
    public enum Transformation
    {
        /// <summary> Transforms the image to shades of grey. </summary>
        Grey,
        /// <summary> Transforms the image to black and white. </summary>
        BnW,
        /// <summary> Transforms the image to negative. </summary>
        Negative,
        /// <summary> Detects the edges. </summary>
        EdgeDetection,
        /// <summary> Pushes the edges. </summary>
        EdgePushing,
        /// <summary> Sharpen the image. </summary>
        Sharpen,
        /// <summary> Better emboss the image. </summary>
        GaussianBlur, 
        /// <summary> Contrasts the image. </summary>
        Contrast,
    }
    /// <summary> Applies a transformation to the image. </summary>
    /// <param name="alter"> The transformation to apply. </param>
    /// <returns> The transformed image. </returns>
    public PictureBitMap AlterColors(Transformation alter)
    {
        PictureBitMap newImage = Dupplicate();
        for (int x = 0; x < this.GetLength(0); x++) 
            for (int y = 0; y < this.GetLength(1); y++) 
                switch (alter)
                {
                    case Transformation.Grey:
                        newImage[x, y] = this[x, y].GreyAverage();
                        break;
                    case Transformation.BnW:
                        newImage[x, y] = this[x, y].GreyAverage().Red > 127 ? new Pixel(255, 255, 255) : new Pixel(0, 0, 0);
                        break;
                    case Transformation.Negative:
                        newImage[x, y] = new Pixel((byte)(255 - this[x, y].Red), (byte)(255 - this[x, y].Green), (byte)(255 - this[x, y].Blue));
                        break;
                    default:
                        throw new NotImplementedException("This transformation is not implemented for this transformation.");
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
            {
                int newX = (int)(x / scale);
                int newY = (int)(y / scale);
                newImage[x, y] = new (previousImage[newX, newY ]);
            }
        return newImage;
    }
    #endregion

    #region Steganography
    /// <summary> Hides a <paramref name="guest"/> in the <paramref name="source"/>. </summary>
    /// <param name="source"> The picture in which the <paramref name="guest"/> will be hidden. </param>
    /// <param name="guest"> The picture to hide in the <paramref name="source"/>. </param>
    /// <returns> The <paramref name="source"/> with the <paramref name="guest"/> hidden in it. </returns>
    public static PictureBitMap Encrypt(PictureBitMap source, PictureBitMap guest)
    {
        PictureBitMap host = source.Dupplicate();
        host.imagePath = source.imagePath;
        #region Verify dimensions
        if (host.GetLength(0) < guest.GetLength(0) || host.GetLength(1) < guest.GetLength(1))
        {
            float newScale = Math.Min((float)host.GetLength(0) / guest.GetLength(0), (float)host.GetLength(1) / guest.GetLength(1));
            guest = guest.Resize(newScale);
        }
        #endregion

        Dictionary<string, int> guestPictureSize = new Dictionary<string, int> { 
        { "height", guest.GetLength(0) }, 
        { "width", guest.GetLength(1) } };
        
        
        
        for (int x = 0; x < guest.GetLength(0); x++)
        {
            for (int y = 0; y < guest.GetLength(1); y++)
            {
                Pixel hostPix = host[x, y];
                Pixel guestPix = guest[x, y];
                host[x, y] = new Pixel(
                    codeByte(hostPix.Red, guestPix.Red),
                    codeByte(hostPix.Green, guestPix.Green),
                    codeByte(hostPix.Blue, guestPix.Blue));
            }
        }
        host.Save("Images/OUT/steganography/encrypted/");
        if (host.imagePath is null)
            throw new NullReferenceException("host.imagePath is null.");
        string jsonPath = $"{host.imagePath.Substring(0, host.imagePath.Length - 3)}json";
        File.WriteAllText(jsonPath, JsonSerializer.Serialize(guestPictureSize));
        return host;

        byte codeByte(byte host, byte guest) => (byte)((host & 0b11110000) | ((guest >> 4) & 0b00001111));
    }
    /// <summary> Reveals a <see cref="PictureBitMap"/>. </summary>
    /// <returns> The <see cref="PictureBitMap"/> hidden in the <see cref="PictureBitMap"/> instance. </returns>
    public static PictureBitMap Decrypt(string path)
    {
        PictureBitMap composedPicture = new PictureBitMap(path);
        string json = path.Substring(0, path.Length - 3) + "json";
        Dictionary<string, int> guestPictureSize = JsonSerializer.Deserialize<Dictionary<string, int>>(File.ReadAllText(json)) ?? throw new NullReferenceException("guestPictureSize.json not found.");

        PictureBitMap revealedPicture = new (guestPictureSize["height"], guestPictureSize["width"]);
        for (int x = 0; x < revealedPicture.GetLength(0); x++)
        {
            for (int y = 0; y < revealedPicture.GetLength(1); y++)
            {
                Pixel host = composedPicture[x, y];
                revealedPicture[x, y] = new Pixel(
                    decodeByte(host.Red),
                    decodeByte(host.Green),
                    decodeByte(host.Blue));
            }
        }
        revealedPicture.Save("Images/OUT/steganography/decrypted/dec_");
        return revealedPicture;

        byte decodeByte(byte composed) => (byte)((composed << 4) & 0b11110000);
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
        s_ProcessStopwatch.Stop();
        string imageName = "";
        do
        {
            if (imageName != "")
                WriteParagraph(new string[] { s_Dict[s_Lang]["error"]["taken_name"] }, true);
            imageName = WritePrompt(s_Dict[s_Lang]["prompt"]["save"]);
        }while(IsFileNameTaken(imageName, savePath));
        imagePath = savePath + imageName + ".bmp";

        s_ProcessStopwatch.Start();
        using (FileStream stream = File.OpenWrite(imagePath))
        {
            stream.Write(info, 0, info.Length);
            stream.Write(data, 0, data.Length);
        }
        s_ProcessStopwatch.Stop();
        WriteParagraph(new string[] {
            s_Dict[s_Lang]["title"]["save1"]  + imagePath + " ", 
            s_Dict[s_Lang]["title"]["save2"] + s_ProcessStopwatch.ElapsedMilliseconds + " ms. "}, true);
        s_ProcessStopwatch.Reset();
        ReadKey(true);
        ClearContent();
        DisplayImage();
    }
    /// <summary> This method is used to print the <see cref="PictureBitMap"/>. </summary>
    public void DisplayImage()
    {
        switch (ScrollingMenu(s_Dict[s_Lang]["title"]["display_action"] , new string[]{
            s_Dict[s_Lang]["generic"]["no"], 
            s_Dict[s_Lang]["generic"]["yes"]}))
        {
            case 0:
                break;
            case 1:
                if (imagePath is not null)
                    Display(imagePath);
                else if (s_SourceImagePath is not "none")
                    Display(s_SourceImagePath);
                if(s_SourceImagePath is null && imagePath is null)
                    throw new NullReferenceException("The image path is null.");

                WriteParagraph(new string[] {
                    s_Dict[s_Lang]["title"]["display_waiting1"], 
                    s_Dict[s_Lang]["title"]["display_waiting2"]  }, true);
                ReadKey(true);
                ClearContent();
                break;
            default:
                break;
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