using System.Diagnostics;
using System.Text;

using static System.Console;

using Utilitary;

using static Visuals.ConsoleVisuals;
using static Language.LanguageDictonary;

namespace Computer_Science_Problem;

/// <summary> This class represents an image. </summary>
public class Image 
{
    #region Fields
    /// <summary> Represents the path of the first image. </summary>
    public static string? s_InitialImagePath;
    /// <summary> Represents the path of the final image. </summary>
    public static string? s_FinalImagePath;
    /// <summary> Represents the stopwatch used to measure the time of the execution of the filter. </summary>
    public static Stopwatch sw = new ();
    ///<summary> Creation of the array of bytes of the header. </summary>
    public byte[] Header;
    ///<summary> Creation of the array of bytes of the image. </summary>                            
    public byte[] Content;
    #endregion

    #region Constructors
    /// <summary> Creates an <see cref="Image"/> instance from a referenced file by <paramref name="filePath"/>. </summary>
    /// <param name="filePath"> Image path to open. </param>
    public Image(string? filePath)
    {
        if (filePath is null) 
            throw new ArgumentNullException(nameof(filePath));

        using (FileStream stream = File.OpenRead(filePath))
        {
            Header = new byte[54];
            for (int i = 0; i < 54; i++) 
                Header[i] = (byte)stream.ReadByte();

            Content = new byte[(int)(FileSize - StartOffset)];
            for (int i = 0; i < (int)(FileSize - StartOffset); i++) 
                Content[i] = (byte)stream.ReadByte();
        }

        if (Header is null) 
            throw new ArgumentNullException(nameof(Header));
        if (Content is null) 
            throw new ArgumentNullException(nameof(Content));
    }
    /// <summary> Creates an <see cref="Image"/> instance from an height <paramref name="height"/> and a width <paramref name="width"/>. <br/>The image is automatically filled in black (components set to 0).</summary>
    /// <param name="width">Image width.</param>
    /// <param name="height">Image height.</param>
    public Image(int width, int height)
    {
        if (width < 1 || height < 1) 
            throw new ArgumentOutOfRangeException("Width and height must be positive");
            
        Header = new byte[54];
        for (int i = 0; i < Encoding.ASCII.GetBytes("BM").Length; i++) 
            Header[i] = Encoding.ASCII.GetBytes("BM")[i];
        for (int i = 0; i < ConvertTo.LittleEndian((uint)Header.Length).Length; i++) 
            Header[10 + i] = ConvertTo.LittleEndian((uint)Header.Length)[i];
        for (int i = 0; i < ConvertTo.LittleEndian(40).Length; i++) 
            Header[14 + i] = ConvertTo.LittleEndian(40)[i];
        for (int i = 0; i < ConvertTo.LittleEndian(width).Length; i++) 
            Header[18 + i] = ConvertTo.LittleEndian(width)[i];
        for (int i = 0; i < ConvertTo.LittleEndian(height).Length; i++) 
            Header[22 + i] = ConvertTo.LittleEndian(height)[i];
        for (int i = 0; i < ConvertTo.LittleEndian((ushort)1).Length; i++) 
            Header[26 + i] = ConvertTo.LittleEndian((ushort)1)[i];
        for (int i = 0; i < ConvertTo.LittleEndian((ushort)24).Length; i++) 
            Header[28 + i] = ConvertTo.LittleEndian((ushort)24)[i];

        Content = new byte[height * Stride];
        for (int i = 0; i < ConvertTo.LittleEndian((uint)(Header.Length + Content.Length)).Length; i++) 
            Header[2 + i] = ConvertTo.LittleEndian((uint)(Header.Length + Content.Length))[i];
    }
    #endregion

    #region Properties
    /// <summary> Utils.LittleEndianToUInt is a method which turn the information of the file size contained in a bytes array into an unsigned integer </summary>
    public uint FileSize => ConvertTo.UInt(Header, 2);
    /// <summary> Utils.LittleEndianToUInt is a method which turn the information of the start offset contained in a bytes array into an unsigned integer </summary>
    public uint StartOffset => ConvertTo.UInt(Header, 10);
    /// <summary> Utils.LittleEndianToInt is a method which turn the information of the width contained in a bytes array into an integer </summary>
    public int Width => ConvertTo.Int(Header, 18);
    /// <summary> Utils.LittleEndianToInt is a method which turn the information of the height contained in a bytes array into an integer </summary>
    public int Height => ConvertTo.Int(Header, 22);
    /// <summary> Utils.LittleEndianToUShort is a method which turn the information of the color depth contained in a bytes array into an unsigned short </summary>
    public ushort ColorDepth => ConvertTo.UShort(Header, 28);
    /// <summary> Stride is the number of bytes per line of pixels. </summary>
    public int Stride => (Width * ColorDepth / 8 + 3) / 4 * 4; 
    #endregion 

    #region Transformations
    /// <summary> Transform this <see cref="Image"/> to shades of grey (this method generates a copy). </summary>
    /// <returns> A greyscale copy of this <see cref="Image"/>. </returns>
    public Image TurnGrey()
    {
        Image newImage = Copy();

        for (int x = 0; x < Width; x++) 
            for (int y = 0; y < Height; y++) 
                newImage[x, y] = this[x, y].TurnGrey();
        return newImage;
    }
    /// <summary> Transform this <see cref="Image"/> to black and white (this method generate a copy). </summary>
    /// <returns> A black and white copy of this <see cref="Image"/>.</returns>
    public Image BlackAndWhite()
    {
        Image newImage = Copy();

        for (int x = 0; x < Width; x++) 
            for (int y = 0; y < Height; y++) 
                newImage[x, y] = this[x, y].TurnGrey().Red > 127 ? new Pixel(255, 255, 255) : new Pixel(0, 0, 0);
        return newImage;
    }
    /// <summary> Rotates the <see cref="Image"/> instance at an <paramref name="angle"/>.</summary>
    /// <param name="angle">Angle of the rotation (in degrees)</param>
    /// <returns>A copy of this <see cref="Image"/> rotated by <paramref name="angle"/> degrees.</returns>
    public Image Rotate(int angle)
    {
        Image previousImage = Copy();

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
        Image previousImage = Copy();
            
        int newWidth = (int)(Width * scale);
        int newHeight = (int)(Height * scale);

        Image newImage = new (newWidth, newHeight);

        for (int x = 0; x < newWidth; x++)
            for (int y = 0; y < newHeight; y++)
                newImage[x, y] = new (previousImage[(int)(x / scale), (int)(y / scale)]);
        return newImage;
    }
    #endregion

    #region Methods
    /// <summary> This method is used to get a copy of this <see cref="Image"/>. </summary>
    /// <returns> A copy of this <see cref="Image"/>. </returns>
    public Image Copy() 
    {
        Image newImage = new (Width, Height);
        newImage.Header = Header;
        newImage.Content = Content;
        return newImage;
    }
    /// <summary> This method is used to get the <see cref="Pixel"/> at the specified <paramref name="x"/> and <paramref name="y"/> coordinates. </summary>
    /// <param name="x"> The x coordinate of the <see cref="Pixel"/>. </param>
    /// <param name="y"> The y coordinate of the <see cref="Pixel"/>. </param>
    /// <returns> The position at the specified <paramref name="x"/> and <paramref name="y"/> coordinates. </returns>
    private int pos(int x, int y) => x * 3 + (Height - y - 1) * Stride;
    /// <summary> This method is used to get the <see cref="Pixel"/> at the specified <paramref name="x"/> and <paramref name="y"/> coordinates. </summary>
    public Pixel this[int x, int y]
    {
        get
        {
            int position = pos(x, y);
            return new (Content[position + 2], Content[position + 1], Content[position + 0]);
        }
        set
        {
            int position = pos(x, y);
            Content[position + 2] = value.Red;
            Content[position + 1] = value.Green;
            Content[position + 0] = value.Blue;
        }
    }
    /// <summary> This method saves the <see cref="Image"/>. </summary>
    public void Save()
    {
        sw.Stop();
        s_FinalImagePath = "Images/OUT/" + WritePrompt(s_Dict[s_Lang]["prompt"]["save"]) + ".bmp";
        sw.Start();
        using (FileStream stream = File.OpenWrite(s_FinalImagePath))
        {
            stream.Write(Header, 0, Header.Length);
            stream.Write(Content, 0, Content.Length);
        }
        sw.Stop();
        WriteParagraph(new string[] {
            s_Dict[s_Lang]["title"]["save1"]  + s_FinalImagePath + " ", 
            s_Dict[s_Lang]["title"]["save2"] + sw.ElapsedMilliseconds + " ms. "}, true);
        sw.Reset();
        DisplayImage();
    }
    /// <summary> This method is used to print the <see cref="Image"/>. </summary>
    public static void DisplayImage()
    {
        switch(ScrollingMenu(s_Dict[s_Lang]["title"]["display_action"] , new string[]{
            s_Dict[s_Lang]["generic"]["no"], 
            s_Dict[s_Lang]["generic"]["yes"]}, false, CursorTop + 2))
        {
            case 1:
                break;
            default:
                if (s_FinalImagePath is not null)
                {
                    s_InitialImagePath = null;
                    s_FinalImagePath = null;
                }
                return;
        }
        if (s_FinalImagePath is not null)
            Display(s_FinalImagePath);
        if (s_InitialImagePath is not null)
            Display(s_InitialImagePath);
        else
            throw new NullReferenceException("The image path is null.");
            
        WriteParagraph(new string[] {
            s_Dict[s_Lang]["title"]["display_waiting1"], 
            s_Dict[s_Lang]["title"]["display_waiting2"]  }, true);
        ReadKey(true);
        if (s_FinalImagePath is not null)
        {
            s_InitialImagePath = null;
            s_FinalImagePath = null;
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
    #endregion
}