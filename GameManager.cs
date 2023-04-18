using System.Security.AccessControl;
using System.Text.Json;
using System.Diagnostics;
using static System.ConsoleColor;
using static System.Console;

using Instances;
using Utilitary;
using static Instances.PictureBitMap;
using static Visuals.ConsoleVisuals;

namespace Computer_Science_Problem;

/// <summary> The Mainprogram class is the entry point of the program. from this point, every processing method is called. </summary>
public static class GameManager
{
    #region Main
    /// <summary> The entry point of the program. </summary>
    public static void Main(string[] args)
    {
        #region Processing
        WriteFullScreen(default);

        Main_Menu:

        MainMenu();
        switch (t_Jump)
        {
            case Jump.Continue:
                goto Actions;
            case Jump.Options:
                goto Options;
            case Jump.Exit:
                ProgramExit();
                return;
        }

        Actions:

        Actions();
        switch (t_Jump)
        {
            case Jump.Filter:
                goto Filter;
            case Jump.Manipulation:
                goto Manipulation;
            case Jump.CustomKernel:
                goto CustomKernel;
            case Jump.Steganography:
                goto Steganography;
            case Jump.Back:
                goto Main_Menu;
        }

        Filter:
        Transformation filter = ChooseFilter();
        if (t_Jump is Jump.Back) 
            goto Actions;
        ChooseFile(ChooseFolder());
        if (t_Jump is Jump.Back) 
            goto Filter;
        ApplyFilter(filter);
        WriteFullScreen(true);
        goto Main_Menu;

        Manipulation:
        Manipulation manipulation = ChooseManipulation();
        if (t_Jump is Jump.Back) 
            goto Actions;
        ChooseFile(ChooseFolder());
        if (t_Jump is Jump.Back) 
            goto Manipulation;
        ApplyManipulation(manipulation);
        WriteFullScreen(true);
        goto Main_Menu;

        CustomKernel:
        float[,]? kernel = DefineCustomKernel();
        if (kernel is null)
            goto Actions;
        ChooseFile(ChooseFolder());
        if (t_Jump is Jump.Back) 
            goto CustomKernel;
        ApplyCustomKernel(kernel);
        WriteFullScreen(true);
        goto Main_Menu;

        Steganography:
        Encrypting encryptingProcess = ChooseEncrypting();
        if (t_Jump is Jump.Back) 
            goto Actions;
        switch (encryptingProcess)
        {
            case Encrypting.Encrypt:
                goto Encrypt;
            case Encrypting.Decrypt:
                goto Decrypt;
        }
        
        Encrypt:
        
        WriteParagraph(new string[]{
            s_Dict[s_Lang]["title"]["stega_host1"],
            s_Dict[s_Lang]["title"]["stega_host2"]}, true);
        ReadKey(true);
        ClearContent();
        ChooseFile(ChooseFolder());
        if (t_Jump is Jump.Back) 
            goto Steganography;
        var hostImage = new PictureBitMap(s_SourceImagePath);
        
        WriteParagraph(new string[]{
            s_Dict[s_Lang]["title"]["stega_guest1"],
            s_Dict[s_Lang]["title"]["stega_guest2"]}, true);
        ReadKey(true);
        ClearContent();
        ChooseFile(ChooseFolder());
        if (t_Jump is Jump.Back) 
            goto Encrypt;
        var guestImage = new PictureBitMap(s_SourceImagePath);

        s_ProcessStopwatch.Start();
        Encrypt(hostImage, guestImage);
        WriteFullScreen(true);
        goto Main_Menu;

        Decrypt:
        ChooseFile("Images/OUT/steganography/encrypted/");
        if (t_Jump is Jump.Back) 
            goto Steganography;
        var encryptedImage = Decrypt(s_SourceImagePath);
        WriteFullScreen(true);
        goto Main_Menu;
        #endregion

        #region Options
        Options:

        Options();
        switch (t_Jump)
        {
            case Jump.Language:
                goto Language;
            case Jump.FontColor:
                goto FontColor;
            case Jump.Main_Menu:
                goto Main_Menu;
        }

        Language:

        ChangeLanguage();
        switch (t_Jump)
        {
            case Jump.Main_Menu:
                goto Options;
            case Jump.Back:
                goto Options;
        }

        FontColor:

        ChangeColor();
        switch (t_Jump)
        {
            case Jump.FontColor:
                goto FontColor;
            case Jump.Back:
                goto Options;
        }
        #endregion
    }
    #endregion

    #region Fields
    /// <summary> The jump variable, used as a "thread" to get the information wether to continue the execution or to "jump" to a specific part of the code.</summary>
    public static Jump t_Jump = Jump.Main_Menu;
    /// <summary> Represents the path of the source image for a specific use. </summary>
    public static string s_SourceImagePath = "none";
    /// <summary> This field represents the current language. </summary>
    public static string s_Lang = "english";
    /// <summary> This field represents the dictionary from which the strings are extracted. </summary>
    public static Dictionary<string, Dictionary<string,  Dictionary<string, string>>> s_Dict { get; private set; } = InitializeDictionary();
    /// <summary> Represents the stopwatch used to measure the time of the execution of the action. </summary>
    public static Stopwatch s_ProcessStopwatch = new ();
    #endregion

    #region Jump enum
    /// <summary> Enumerates the different jumps. </summary>
    public enum Jump
    {
        /// <summary> Continue the execution. </summary>
        Continue,
        /// <summary> Go back to the previous menu. </summary>
        Back,
        /// <summary> Go to the main menu. </summary>
        Main_Menu,
        /// <summary> Go to the options menu. </summary>
        Options,
        /// <summary> Go to the language menu. </summary>
        Language,
        /// <summary> Go to the font color menu. </summary>
        FontColor,
        /// <summary> Go to the actions menu. </summary>
        Actions,
        /// <summary> Go to the filter menu. </summary>
        Filter,
        /// <summary> Go to the manipulation menu. </summary>
        Manipulation,
        /// <summary> Go to the custom kernel menu. </summary>
        CustomKernel,
        /// <summary> Go to the steganography menu. </summary>
        Steganography,
        /// <summary> Exit the program. </summary>
        Exit
    }
    #endregion
    
    #region Methods
    #region Processing methods
    /// <summary>This method is used to display the main menu.</summary>
    public static void MainMenu()
    {
        ClearContent();
        switch (ScrollingMenu(s_Dict[s_Lang]["title"]["main"], new string[] { 
            s_Dict[s_Lang]["options"]["main_play"], 
            s_Dict[s_Lang]["options"]["main_options"],
            s_Dict[s_Lang]["options"]["main_quit"], }))
        {
            case 0:
                t_Jump = Jump.Continue;
                break;
            case 1:
                t_Jump = Jump.Options;
                break;
            case 2: case -1:
                t_Jump = Jump.Exit;
                break;
        }
    }
    /// <summary>This method is used to display the actions menu.</summary>
    public static void Actions()
    {
        switch (ScrollingMenu(s_Dict[s_Lang]["title"]["actions"], new string[]{
                s_Dict[s_Lang]["options"]["actions_filter"],
                s_Dict[s_Lang]["options"]["actions_transformation"],
                s_Dict[s_Lang]["options"]["actions_custom"],
                s_Dict[s_Lang]["options"]["actions_steganography"],
                s_Dict[s_Lang]["generic"]["back"]}))
        {
            case 0:
                t_Jump = Jump.Filter;
                break;
            case 1:
                t_Jump = Jump.Manipulation;
                break;
            case 2:
                t_Jump = Jump.CustomKernel;
                break;
            case 3:
                t_Jump = Jump.Steganography;
                break;
            case 4: case -1:
                t_Jump = Jump.Back;
                break;
        }
    }
    #endregion

    #region Options
    /// <summary> This method is used to display the options menu. </summary>
    public static void Options()
    {
        switch(ScrollingMenu(s_Dict[s_Lang]["title"]["options"], new string[]{
            s_Dict[s_Lang]["options"]["options_color"], 
            s_Dict[s_Lang]["options"]["options_language"],
            s_Dict[s_Lang]["generic"]["reload"],
            s_Dict[s_Lang]["generic"]["back"]}))
        {
            case 0:
                t_Jump = Jump.FontColor;
                break;
            case 1:
                t_Jump = Jump.Language;
                break;
            case 2: case 3: case -1:
                t_Jump = Jump.Main_Menu;
                break;
        }
    }
    /// <summary> This method is used to display the language chooser. </summary>
    public static void ChangeLanguage()
    {
        switch(ScrollingMenu(s_Dict[s_Lang]["title"]["language"], new string[]{
            s_Dict[s_Lang]["options"]["language_fr"],
            s_Dict[s_Lang]["options"]["language_en"],
            s_Dict[s_Lang]["generic"]["back"]}))
        {
            case 0:
                s_Lang = "french";
                t_Jump = Jump.Main_Menu;
                break;
            case 1:
                s_Lang = "english";
                t_Jump = Jump.Main_Menu;
                break;
            case 2: case -1:
                t_Jump = Jump.Options;
                break;
        }
        
    }
    /// <summary> This method is used to display the color chooser. </summary>
    public static void ChangeColor()
    {
        switch(ScrollingMenu(s_Dict[s_Lang]["title"]["color"], new string[]{
            s_Dict[s_Lang]["options"]["color_default"], 
            s_Dict[s_Lang]["options"]["color_red"],
            s_Dict[s_Lang]["options"]["color_magenta"],
            s_Dict[s_Lang]["options"]["color_yellow"],
            s_Dict[s_Lang]["options"]["color_green"],
            s_Dict[s_Lang]["options"]["color_blue"],
            s_Dict[s_Lang]["options"]["color_cyan"]}))
            {
                case 0:
                    ChangeFont(White);
                    break;
                case 1:
                    ChangeFont(Red);
                    break;
                case 2:
                    ChangeFont(Magenta);
                    break;
                case 3:
                    ChangeFont(Yellow);
                    break;
                case 4:
                    ChangeFont(Green);
                    break;
                case 5:
                    ChangeFont(Blue);
                    break;
                case 6:
                    ChangeFont(Cyan);
                    break;
                case -1:
                    t_Jump = Jump.Back;
                    return;
            }
            t_Jump = Jump.FontColor;
    }
    #endregion
    
    #region Filter
    /// <summary> This method is used to display the filter menu. </summary>
    public static Transformation ChooseFilter()
    {
        switch (ScrollingMenu(s_Dict[s_Lang]["title"]["filter"], new string[]{
                s_Dict[s_Lang]["options"]["filter_grey"],
                s_Dict[s_Lang]["options"]["filter_bnw"],
                s_Dict[s_Lang]["options"]["filter_negative"],
                s_Dict[s_Lang]["options"]["filter_gauss"],
                s_Dict[s_Lang]["options"]["filter_sharp"],
                s_Dict[s_Lang]["options"]["filter_contrast"],
                s_Dict[s_Lang]["generic"]["back"]}))
        {
            case 0:
                return Transformation.Grey;
            case 1:
                return Transformation.BnW;
            case 2:
                return Transformation.Negative;
            case 3:
                return Transformation.GaussianBlur;
            case 4:
                return Transformation.Sharpen;
            case 5:
                return Transformation.Contrast;
            default:
                t_Jump = Jump.Back;
                return Transformation.Grey;
        }
    }
    /// <summary>This method is used to display the filters menu.</summary>
    private static void ApplyFilter(Transformation t)
    {
        PictureBitMap image = new (s_SourceImagePath);
        s_ProcessStopwatch.Start();
        switch (t)
        {
            case Transformation.Grey: case Transformation.BnW: case Transformation.Negative:
                image = image.AlterColors(t);
                break;
            case Transformation.GaussianBlur: case Transformation.Sharpen: case Transformation.Contrast:
                image = image.ApplyKernelByName(t);
                break;
        }
        image.Save();
    }
    #endregion

    #region Manipulation
    /// <summary> The different manipulations .</summary>
    public enum Manipulation
    {
        ///<summary>Rotate the image.</summary>
        Rotate,
        ///<summary>Resize the image.</summary>
        Resize,
        ///<summary>Detect the edges of the image.</summary>
        Detect,
        ///<summary>Push the image.</summary>
        Push
    }
    /// <summary>This method is used to display the manipulations menu.</summary>
    public static Manipulation ChooseManipulation()
    {
        switch (ScrollingMenu(s_Dict[s_Lang]["title"]["manip"], new string[]{
                s_Dict[s_Lang]["options"]["manip_rotate"],
                s_Dict[s_Lang]["options"]["manip_resize"],
                s_Dict[s_Lang]["options"]["manip_detect"],
                s_Dict[s_Lang]["options"]["manip_push"],
                s_Dict[s_Lang]["generic"]["back"]}))
        {
            case 0:
                return Manipulation.Rotate;
            case 1:
                return Manipulation.Resize;
            case 2:
                return Manipulation.Detect;
            case 3:
                return Manipulation.Push;
            default:
                t_Jump = Jump.Actions;
                return Manipulation.Rotate;
        }
    }
    /// <summary>This method is used to display the manipulations menu.</summary>
    public static void ApplyManipulation(Manipulation m)
    {
        PictureBitMap image = new PictureBitMap(s_SourceImagePath);
        switch (m)
        {
            case Manipulation.Rotate:
                int? angle = null;
                int occurrenceRotation = 0;
                do
                {   string answer = WritePrompt(s_Dict[s_Lang]["prompt"]["rotate"]);
                    angle = int.TryParse(answer, out int result) ? result : null;
                    occurrenceRotation++;
                } while (angle is null || angle < 0 || angle > 360);
                s_ProcessStopwatch.Start();
                image = image.Rotation((int)angle);
                break;
            case Manipulation.Resize:
                float? scale = null;
                int occurrenceResize = 0;
                do
                {
                    string answer = WritePrompt(s_Dict[s_Lang]["prompt"]["resize"]);
                    scale = float.TryParse(answer, out float result) ? result : null;
                    occurrenceResize++;
                } while (scale is null || scale < 1);
                s_ProcessStopwatch.Start();
                image = image.Resize((float)scale);
                break;
            case Manipulation.Detect:
                s_ProcessStopwatch.Start();
                image = image.ApplyKernelByName(Transformation.EdgeDetection);
                break;
            case Manipulation.Push:
                s_ProcessStopwatch.Start();
                image = image.ApplyKernelByName(Transformation.EdgePushing);
                break;
        }
        image.Save();
    }
    #endregion
    
    #region Custom Kernel
    /// <summary>This method is used to display the specific kernels menu.</summary>
    public static float[,]? DefineCustomKernel()
    {
        float[,] kernel;
        while (true)
        {
            if(int.TryParse(WritePrompt(s_Dict[s_Lang]["prompt"]["custom"]), out int value) && value > 2)
            {
                kernel = new float[value, value];
                break;
            }
        }
        return kernel.MatrixSelector();
    }
    /// <summary>This method is used to display the specific kernels menu.</summary>
    public static void ApplyCustomKernel(float[,] kernel)
    {
        float[,] newKernel = (float[,])kernel;
        PictureBitMap image = new PictureBitMap(s_SourceImagePath);
        s_ProcessStopwatch.Start();
        image = image.ApplyKernel(newKernel);
        image.Save();
        t_Jump = Jump.Main_Menu;
        
    }
    #endregion

    #region Steganography
    /// <summary> The different encryption methods .</summary>
    public enum Encrypting
    {
        ///<summary>Encrypt the image.</summary>
        Encrypt,
        ///<summary>Decrypt the image.</summary>
        Decrypt
    }
    /// <summary>This method is used to display the steganography menu.</summary>
    public static Encrypting ChooseEncrypting()
    {
        switch (ScrollingMenu(s_Dict[s_Lang]["title"]["steganography"], new string[]{
                s_Dict[s_Lang]["options"]["steganography_encrypt"],
                s_Dict[s_Lang]["options"]["steganography_decrypt"],
                s_Dict[s_Lang]["generic"]["back"]}))
        {
            case 0:
                return Encrypting.Encrypt;
            case 1:
                return Encrypting.Decrypt;
            default: 
                t_Jump = Jump.Back;
                return default;
        }
    }
    #endregion
    
    #region Utilitary methods
    /// <summary> This method initializes the dictionary. </summary>
    public static Dictionary<string, Dictionary<string,  Dictionary<string, string>>> InitializeDictionary()
    {
        string jsonString = File.ReadAllText("Language/dataLanguages.json");

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        Dictionary<string, Dictionary<string,  Dictionary<string, string>>>? nullHandler = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string,  Dictionary<string, string>>>>(jsonString, options);
        return nullHandler ?? throw new NullReferenceException("The dictionary is null.");
    }
    /// <summary> This method checks if a file name is already taken in a directory. </summary>
    public static bool IsFileNameTaken(string fileName, string directoryPath)
    {
        string[] files = Directory.GetFiles(Path.GetDirectoryName(directoryPath) ?? throw new InvalidOperationException());
        return files.Any(file => Path.GetFileName(file) == fileName);
    }
    #endregion

    #region File selector
    /// <summary>This method is used to display the Image source folder chooser.</summary>
    public static string ChooseFolder()
    {
        switch (ScrollingMenu(s_Dict[s_Lang]["title"]["folder"], new string[]{
                s_Dict[s_Lang]["options"]["folder_default"],
                s_Dict[s_Lang]["options"]["folder_generated"],
                s_Dict[s_Lang]["generic"]["back"]}))
        {
            case 0:
                t_Jump = Jump.Continue;
                return "Images/Default";
            case 1:
                t_Jump = Jump.Continue;
                return "Images/OUT/bmp";
            default:
                t_Jump = Jump.Back;
                return "none";
        }
    }
    /// <summary> This method is used to display the file chooser. </summary>
    /// <param name="directoryPath"> The path of the folder to display. </param>
    /// <param name="exclude"> The extension to exclude. </param>
    public static void ChooseFile(string directoryPath, string exclude = ".json")
    {
        if (directoryPath is "none") 
            return;

        string[] files = Directory.GetFiles(directoryPath);
        files = files.Where(file => Path.GetExtension(file) != exclude).ToArray();
        string[] filesName = new string[files.Length];
        filesName = files.Select(file => Path.GetFileNameWithoutExtension(file)).ToArray();

        // if (directoryPath is "Images/Default")
        //     for (int i = 0; i < files.Length; i++)
        //         filesName[i] = files[i].Substring(15, files[i].Length - 19);
        // else if (directoryPath is "Images/OUT/bmp")
        //     for (int i = 0; i < files.Length; i++)
        //         filesName[i] = files[i].Substring(15, files[i].Length - 19);
                
        int namePosition;
        switch (namePosition = ScrollingMenu(s_Dict[s_Lang]["title"]["file"], 
            filesName))
        {
            case -1:
                t_Jump = Jump.Back;
                break;
            default:
                s_SourceImagePath = files[namePosition];
                new PictureBitMap(files[namePosition]).DisplayImage();
                t_Jump = Jump.Actions;
                break;
        }
    }
    #endregion
    #endregion
}