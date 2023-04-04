using static System.ConsoleColor;

using Instances;
using Utilitary;
using static Instances.PictureBitMap;
using static Visuals.ConsoleVisuals;
using static Language.LanguageDictonary;

namespace Computer_Science_Problem;

/// <summary>The vocation of the Methods class is to be accessible from anywhere.It contains a random variable, utility and core methods.</summary>
public static class GeneralMethods
{  
    #region Processing methods
    /// <summary>This method is used to display the main menu.</summary>
    public static void MainMenu()
    {
        switch (ScrollingMenu(s_Dict[s_Lang]["title"]["main"], new string[] { 
            s_Dict[s_Lang]["options"]["main_play"], 
            s_Dict[s_Lang]["options"]["main_options"],
            s_Dict[s_Lang]["options"]["main_quit"], }))
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
        switch (ScrollingMenu(s_Dict[s_Lang]["title"]["folder"], new string[]{
                s_Dict[s_Lang]["options"]["folder_default"],
                s_Dict[s_Lang]["options"]["folder_generated"],
                s_Dict[s_Lang]["generic"]["back"]}))
        {
            case 0:
                return "Images/Default";
            case 1:
                return "Images/OUT/bmp";
            default:
                MainProgram.jump = MainProgram.Jump.Main_Menu;
                return "none";
        }
    }
    /// <summary> This method is used to display the file chooser. </summary>
    /// <param name="path"> The path of the folder to display. </param>
    public static void ChooseFile(string path)
    {
        if (path is "none") 
            return;
        string[] files = Directory.GetFiles(path);
        string[] filesName = new string[files.Length];
        if (path is "Images/Default")
            for (int i = 0; i < files.Length; i++)
                filesName[i] = files[i].Substring(15, files[i].Length - 19);
        else if (path is "Images/OUT/bmp")
            for (int i = 0; i < files.Length; i++)
                filesName[i] = files[i].Substring(15, files[i].Length - 19);
        int namePosition;
        switch (namePosition = ScrollingMenu(s_Dict[s_Lang]["title"]["file"], filesName))
        {
            case -1:
                MainProgram.jump = MainProgram.Jump.Source_Folder;
                break;
            default:
                PictureBitMap.s_WorkingImagePath = files[namePosition];
                PictureBitMap.DisplayImage();
                MainProgram.jump = MainProgram.Jump.Continue;
                break;
        }
    }
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
        switch(ScrollingMenu(s_Dict[s_Lang]["title"]["language"], new string[]{
            s_Dict[s_Lang]["options"]["language_fr"],
            s_Dict[s_Lang]["options"]["language_en"],
            s_Dict[s_Lang]["generic"]["back"]}))
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
        switch(ScrollingMenu(s_Dict[s_Lang]["title"]["color"], new string[]{
            s_Dict[s_Lang]["options"]["color_default"], 
            s_Dict[s_Lang]["options"]["color_red"],
            s_Dict[s_Lang]["options"]["color_magenta"],
            s_Dict[s_Lang]["options"]["color_yellow"],
            s_Dict[s_Lang]["options"]["color_green"],
            s_Dict[s_Lang]["options"]["color_blue"],
            s_Dict[s_Lang]["options"]["color_cyan"],
            s_Dict[s_Lang]["generic"]["back"],}))
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
        switch (ScrollingMenu(s_Dict[s_Lang]["title"]["actions"], new string[]{
                s_Dict[s_Lang]["options"]["actions_filter"],
                s_Dict[s_Lang]["options"]["actions_transformation"],
                s_Dict[s_Lang]["options"]["actions_custom"],
                s_Dict[s_Lang]["generic"]["back"]}))
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
        PictureBitMap image = new (PictureBitMap.s_WorkingImagePath);
        switch (ScrollingMenu(s_Dict[s_Lang]["title"]["language"], new string[]{
                s_Dict[s_Lang]["options"]["filter_grey"],
                s_Dict[s_Lang]["options"]["filter_bnw"],
                s_Dict[s_Lang]["options"]["filter_negative"],
                s_Dict[s_Lang]["options"]["filter_gauss"],
                s_Dict[s_Lang]["options"]["filter_sharp"],
                s_Dict[s_Lang]["options"]["filter_contrast"],
                s_Dict[s_Lang]["generic"]["back"]}))
        {
            case 0:
                PictureBitMap.sw.Start();
                image = image.AlterColors(Transformations.Grey);
                break;
            case 1:
                PictureBitMap.sw.Start();
                image = image.AlterColors(Transformations.BnW);
                break;
            case 2:
                PictureBitMap.sw.Start();
                image = image.AlterColors(Transformations.Negative);
                break;
            case 3:
                PictureBitMap.sw.Start();
                image = image.ApplyKernelByName(Convolution.Kernel.GaussianBlur);
                break;
            case 4:
                PictureBitMap.sw.Start();
                image = image.ApplyKernelByName(Convolution.Kernel.Sharpen);
                break;
            case 5:
                PictureBitMap.sw.Start();
                image = image.ApplyKernelByName(Convolution.Kernel.Contrast);
                break;
            case 6: case -1:
                MainProgram.jump = MainProgram.Jump.Actions;
                return;
        }
        image.Save();
    }
    /// <summary>This method is used to display the manipulations menu.</summary>
    public static void ApplyManipulation()
    {
        PictureBitMap image = new PictureBitMap(PictureBitMap.s_WorkingImagePath);
        switch (ScrollingMenu(s_Dict[s_Lang]["title"]["manip"], new string[]{
                s_Dict[s_Lang]["options"]["manip_rotate"],
                s_Dict[s_Lang]["options"]["manip_resize"],
                s_Dict[s_Lang]["options"]["manip_detect"],
                s_Dict[s_Lang]["options"]["manip_push"],
                s_Dict[s_Lang]["generic"]["back"]}))
        {
            case 0:
                int? angle = null;
                int occurrenceRotation = 0;
                do
                {   string answer = WritePrompt(s_Dict[s_Lang]["prompt"]["rotate"]);
                    angle = int.TryParse(answer, out int result) ? result : null;
                    occurrenceRotation++;
                } while (angle is null || angle < 0 || angle > 360);
                PictureBitMap.sw.Start();
                image = image.Rotation((int)angle);
                break;
            case 1:
                float? scale = null;
                int occurrenceResize = 0;
                do
                {
                    string answer = WritePrompt(s_Dict[s_Lang]["prompt"]["resize"]);
                    scale = float.TryParse(answer, out float result) ? result : null;
                    occurrenceResize++;
                } while (scale is null || scale < 1);
                PictureBitMap.sw.Start();
                image = image.Resize((float)scale);
                break;
            case 2:
                PictureBitMap.sw.Start();
                image = image.ApplyKernelByName(Convolution.Kernel.EdgeDetection);
                break;
            case 3:
                PictureBitMap.sw.Start();
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
            if(int.TryParse(WritePrompt(s_Dict[s_Lang]["prompt"]["custom"]), out int value) && value > 2)
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
            PictureBitMap image = new PictureBitMap(PictureBitMap.s_WorkingImagePath);
            PictureBitMap.sw.Start();
            image = image.ApplyKernel(newKernel);
            image.Save();
            MainProgram.jump = MainProgram.Jump.Main_Menu;
        }
    }
    #endregion
}