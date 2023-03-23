using static Visuals.ConsoleVisuals;

using static System.Console;
using static System.ConsoleColor;

using static Computer_Science_Problem.Language.LanguageDictonary;

namespace Computer_Science_Problem;

/// <summary>The vocation of the Methods class is to be accessible from anywhere.It contains a random variable, utility and core methods.</summary>
public static class GeneralMethods
{  
    #region Processing methods
    /// <summary>This method is used to display the main menu.</summary>
    public static void MainMenu()
    {
        switch (ScrollingMenu(Dict[s_Lang]["title"]["main"], new string[] { 
            Dict[s_Lang]["options"]["main_play"], 
            Dict[s_Lang]["options"]["main_options"],
            Dict[s_Lang]["options"]["main_quit"], }))
        {
            case 0:
                MainProgram.jump = MainProgram.Jump.Continue;
                break;
            case 1:
                MainProgram.jump = MainProgram.Jump.Options;
                break;
            case 2: case -1:
                MainProgram.jump = MainProgram.Jump.Exit;
                break;
        }
    }
    /// <summary>This method is used to display the Image source folder chooser.</summary>
    public static string ChooseFolder()
    {
        switch (ScrollingMenu(Dict[s_Lang]["title"]["folder"], new string[]{
                Dict[s_Lang]["options"]["folder_default"],
                Dict[s_Lang]["options"]["folder_generated"],
                Dict[s_Lang]["generic"]["back"]}))
        {
            case 0:
                return "Images";
            case 1:
                return "Images/OUT";
            default:
                MainProgram.jump = MainProgram.Jump.Main_Menu;
                return "none";
        }
    }
    /// <summary> This method is used to display the file chooser. </summary>
    public static void ChooseFile(string path)
    {
        if (path is "none") 
            return;
        string[] files = Directory.GetFiles(path);
        string[] filesName = new string[files.Length];
        if (path is "Images")
            for (int i = 0; i < files.Length; i++)
                filesName[i] = files[i].Substring(7);
        else if (path is "Images/OUT")
            for (int i = 0; i < files.Length; i++)
                filesName[i] = files[i].Substring(11);
        int namePosition;
        switch (namePosition = ScrollingMenu(Dict[s_Lang]["title"]["file"], filesName))
        {
            case -1:
                MainProgram.jump = MainProgram.Jump.Source_Folder;
                break;
            default:
                Image.imagePath = files[namePosition];
                MainProgram.jump = MainProgram.Jump.Continue;
                break;
        }
    }
    /// <summary> This method is used to display the options menu. </summary>
    public static void Options()
    {
        switch(ScrollingMenu(Dict[s_Lang]["title"]["options"], new string[]{
            Dict[s_Lang]["options"]["options_color"], 
            Dict[s_Lang]["options"]["options_language"],
            Dict[s_Lang]["generic"]["reload"],
            Dict[s_Lang]["generic"]["back"]}))
        {
            case 0:
                MainProgram.jump = MainProgram.Jump.FontColor;
                break;
            case 1:
                MainProgram.jump = MainProgram.Jump.Language;
                break;
            case 2:
                WriteFullScreen(true);
                MainProgram.jump = MainProgram.Jump.Main_Menu;
                break;
            case 3: case -1:
                MainProgram.jump = MainProgram.Jump.Main_Menu;
                break;
        }
    }
    /// <summary> This method is used to display the language chooser. </summary>
    public static void ChangeLanguage()
    {
        switch(ScrollingMenu(Dict[s_Lang]["title"]["language"], new string[]{
            Dict[s_Lang]["options"]["language_fr"],
            Dict[s_Lang]["options"]["language_en"],
            Dict[s_Lang]["generic"]["back"]}))
        {
            case 0:
                s_Lang = "french";
                break;
            case 1:
                s_Lang = "english";
                break;
            default:
                MainProgram.jump = MainProgram.Jump.Options;
                return;
        }
        MainProgram.jump = MainProgram.Jump.Main_Menu;
    }
    /// <summary> This method is used to display the color chooser. </summary>
    public static void ChangeColor()
    {
        switch(ScrollingMenu(Dict[s_Lang]["title"]["color"], new string[]{
            Dict[s_Lang]["options"]["color_default"], 
            Dict[s_Lang]["options"]["color_red"],
            Dict[s_Lang]["options"]["color_magenta"],
            Dict[s_Lang]["options"]["color_yellow"],
            Dict[s_Lang]["options"]["color_green"],
            Dict[s_Lang]["options"]["color_blue"],
            Dict[s_Lang]["options"]["color_cyan"],
            Dict[s_Lang]["generic"]["back"],}))
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
                default:
                    MainProgram.jump = MainProgram.Jump.Options;
                    return;
            }
            MainProgram.jump = MainProgram.Jump.FontColor;
    }
    /// <summary>This method is used to display the actions menu.</summary>
    public static void Actions()
    {
        switch (ScrollingMenu(Dict[s_Lang]["title"]["actions"], new string[]{
                Dict[s_Lang]["options"]["actions_filter"],
                Dict[s_Lang]["options"]["actions_transformation"],
                Dict[s_Lang]["options"]["actions_custom"],
                Dict[s_Lang]["generic"]["back"]}))
        {
            case 0:
                MainProgram.jump = MainProgram.Jump.ApplyFilter;
                break;
            case 1:
                MainProgram.jump = MainProgram.Jump.ApplyManipulation;
                break;
            case 2:
                MainProgram.jump = MainProgram.Jump.ApplyCustomKernel;
                break;
            case 3: case -1:
                MainProgram.jump = MainProgram.Jump.Source_Folder;
                break;
        }
    }
    /// <summary>This method is used to display the filters menu.</summary>
    public static void ApplyFilter()
    {
        Image image = new (Image.imagePath);
        switch (ScrollingMenu(Dict[s_Lang]["title"]["language"], new string[]{
                Dict[s_Lang]["options"]["filter_grey"],
                Dict[s_Lang]["options"]["filter_bnw"],
                Dict[s_Lang]["options"]["filter_gauss"],
                Dict[s_Lang]["options"]["filter_sharp"],
                Dict[s_Lang]["options"]["filter_contrast"],
                Dict[s_Lang]["generic"]["back"]}))
        {
            case 0:
                image = image.TurnGrey();
                break;
            case 1:
                image = image.BlackAndWhite();
                break;
            case 2:
                image = image.ApplyKernelByName(Convolution.Kernel.GaussianBlur3x3);
                break;
            case 3:
                image = image.ApplyKernelByName(Convolution.Kernel.Sharpen);
                break;
            case 4:
                image = image.ApplyKernelByName(Convolution.Kernel.Contrast);
                break;
            case 5: case -1:
                MainProgram.jump = MainProgram.Jump.Actions;
                return;
        }
        image.Save();
    }
    /// <summary>This method is used to display the manipulations menu.</summary>
    public static void ApplyManipulation()
    {
        Image image = new Image(Image.imagePath);
        switch (ScrollingMenu(Dict[s_Lang]["title"]["manip"], new string[]{
                Dict[s_Lang]["options"]["manip_rotate"],
                Dict[s_Lang]["options"]["manip_resize"],
                Dict[s_Lang]["options"]["manip_detect"],
                Dict[s_Lang]["options"]["manip_push"],
                Dict[s_Lang]["generic"]["back"]}))
        {
            case 0:
                int? angle = null;
                int occurrenceRotation = 0;
                do
                {   string answer = WritePrompt(Dict[s_Lang]["prompt"]["rotate"]);
                    angle = int.TryParse(answer, out int result) ? result : null;
                    occurrenceRotation++;
                } while (angle is null || angle < 0 || angle > 360);
                image = image.Rotate((int)angle);
                break;
            case 1:
                float? scale = null;
                int occurrenceRescale = 0;
                do
                {
                    string answer = WritePrompt(Dict[s_Lang]["prompt"]["resize"]);
                    scale = float.TryParse(answer, out float result) ? result : null;
                    occurrenceRescale++;
                } while (scale is null || scale < 1);
                image = image.Resize((float)scale);
                break;
            case 2:
                image = image.ApplyKernelByName(Convolution.Kernel.EdgeDetection);
                break;
            case 3:
                image = image.ApplyKernelByName(Convolution.Kernel.EdgePushing);
                break;
            case 4: case -1:
                MainProgram.jump = MainProgram.Jump.Actions;
                return;
        }
        image.Save();
    }
    /// <summary>This method is used to display the specific kernels menu.</summary>
    public static void ApplyCustomKernel()
    {
        float[,]? kernel;
        while (true)
        {
            if(int.TryParse(WritePrompt(Dict[s_Lang]["prompt"]["custom"]), out int value) && value > 2)
            {
                kernel = new float[value, value];
                break;
            }
        }
        if (kernel.MatrixSelector() is null)
        {
            MainProgram.jump = MainProgram.Jump.Actions;
        }
        else
        {
            float[,] newKernel = (float[,])kernel;
            Image image = new Image(Image.imagePath);
            image = image.ApplyKernel(newKernel);
            image.Save();
            MainProgram.jump = MainProgram.Jump.Main_Menu;
        }
    }
    #endregion
 
    #region Extension to the Stream class
    /// <summary> This method reads a <see cref="byte"/> array from a <see cref="Stream"/>. </summary>
    /// <param name="stream"> The <see cref="Stream"/> to read from. </param>
    /// <param name="length"> The length of the array to read. </param>
    /// <returns> The <see cref="byte"/> array read. </returns>
    public static byte[] ReadBytes(this Stream stream, int length)
    {
        byte[] array = new byte[length];
        for (int i = 0; i < length; i++) 
            array[i] = (byte)stream.ReadByte();
        return array;
    }
    /// <summary> This method extracts a <see cref="byte"/> array from a <see cref="Stream"/>. </summary>
    /// <param name="array"> The <see cref="byte"/> array to extract from. </param>
    /// <param name="length"> The length of the array to extract. </param>
    /// <param name="offset"> The offset in the array. </param>
    /// <returns> The <see cref="byte"/> array extracted. </returns>
    public static byte[] ExtractBytes(this byte[] array, int length, int offset = 0)
    {
        byte[] bytes = new byte[length];
        for (int i = 0; i < length; i++) 
            bytes[i] = array[offset + i];
        return bytes;
    }
    /// <summary> This method inserts a <see cref="byte"/> array into another <see cref="byte"/> array. </summary>
    /// <param name="array"> The array to insert into. </param>
    /// <param name="data"> The array to insert. </param>
    /// <param name="offsetTo"> The offset in the first array. </param>
    /// <param name="offsetFrom"> The offset in the second array. </param>
    /// <param name="length"> The length of the array to insert. </param>
    public static void InsertBytes(this byte[] array, byte[] data, int offsetTo = 0, int offsetFrom = 0, int length = -1)
    {
        if (length < 0) 
            length = data.Length;
        for (int i = 0; i < length; i++) 
            array[offsetTo + i] = data[offsetFrom + i];
    }
    #endregion
}