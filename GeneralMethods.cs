using System;

using static Visuals.ConsoleVisuals;

using static System.Console;
using static System.Environment;
using static System.Threading.Thread;
using static System.ConsoleKey;

namespace Computer_Science_Problem;

/// <summary>The vocation of the Methods class is to be accessible from anywhere.It contains a random variable, utility and core methods.</summary>
public static class GeneralMethods
{
    #region Field
    /// <summary>The random variable, usable everywhere.</summary>
    public static Random rnd = new Random();
    #endregion
    #region Core methods
    /// <summary>This method is used to display the main menu.</summary>
    public static void MainMenu()
    {
        switch (ScrollingMenu("Welcome User! Use the arrow keys to move and press [ENTER] to confirm.", new string[] { 
            "Play", 
            "Options", 
            "Quit" }, titlePath))
        {
            case 0:
                MainProgram.jump = MainProgram.Jump.Continue;
                break;
            case 1:
                MainProgram.jump = MainProgram.Jump.Source_Folder;
                break;
            case 2: case -1:
                MainProgram.jump = MainProgram.Jump.Exit;
                break;
        }
    }
    public static string ChooseFolder()
    {
        switch (ScrollingMenu("You may choose a source file.", new string[]{
                "Default pictures ",
                "Created pictures ",
                "Back             "}, titlePath))
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
    public static void ChooseFile(string path)
    {
        if (path is "none") return;
        string[] files = Directory.GetFiles(path);
        string[] filesName = new string[files.Length];
        if (path is "Images")
        {
            for (int i = 0; i < files.Length; i++)
            {
                filesName[i] = files[i].Substring(6);
            }
        }
        else if (path is "Images/OUT")
        {
            for (int i = 0; i < files.Length; i++)
            {
                filesName[i] = files[i].Substring(11);
            }
        }
        int namePosition;
        switch (namePosition = ScrollingMenu("Choose a file:", filesName, titlePath))
        {
            case -1:
                MainProgram.jump = MainProgram.Jump.Source_Folder;
                break;
            default:
                Image.imagePath = files[namePosition];
                MainProgram.jump = MainProgram.Jump.Main_Menu;
                break;
        }
    }
    public static void Actions()
    {
        Image image = new Image(Image.imagePath);
        switch (ScrollingMenu("Choose one action to do on your image.", new string[]{
                "Greyscale       ",
                "Black and white ",
                "Rotate          ",
                "Resize          ",
                "Back            "}, titlePath))
        {
            case 0:
                image = image.Greyscale();
                break;
            case 1:
                image = image.BlackAndWhite();
                break;
            case 2:
                int? angle = null;
                do
                {
                    Title("Type an angle to rotate the picture.", titlePath);
                    Write("{0," + ((WindowWidth / 2) - ("Type an angle to rotate the picture.".Length / 2)) + "}", "");
                    Write("> ");
                    ConsoleConfiguration(false);
                    angle = Convert.ToInt32(ReadLine());
                    ConsoleConfiguration();
                } while (angle is null || angle < 0 || angle > 360);
                if (angle is not null) image = image.Rotate((int)angle);

                break;
            case 3:
                int? scale = null;
                do
                {
                    Title("Type an angle to rotate the picture.", titlePath);
                    Write("{0," + ((WindowWidth / 2) - ("Type an angle to rotate the picture.".Length / 2)) + "}", "");
                    Write("> ");
                    ConsoleConfiguration(false);
                    scale = Convert.ToInt32(ReadLine());
                    ConsoleConfiguration();
                } while (scale is null || scale < 0);
                if (scale is not null) image = image.Scale((int)scale);
                break;
            case 4:
            case -1:
                MainProgram.jump = MainProgram.Jump.Main_Menu;
                return;
        }
        string fileName = "";
        do
        {
            Title("Type the name of the file.", titlePath);
            Write("{0," + ((WindowWidth / 2) - ("Type the name of the file.".Length / 2)) + "}", "");
            Write("> ");
            ConsoleConfiguration(false);
            fileName = ReadLine() ?? "";
            ConsoleConfiguration();
        } while (fileName == "");
        image.Save("Images/OUT/" + fileName + ".bmp");
    }
    #endregion

    #region Utility Methods
    /// <summary>This method is used to pause the program.</summary>
    public static void Pause()
    {
        CenteredWL("Press [ENTER] to continue...");
        while (ReadKey(true).Key != Enter) Sleep(5);
    }
    /// <summary>This method is used to exit the game.</summary>
    public static void FinalExit()
    {
        LoadingScreen("[  Shutting down ...  ]");
        ConsoleConfiguration(false);
        Clear();
        Exit(0);
    }
    #endregion

}
