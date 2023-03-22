using static Computer_Science_Problem.GeneralMethods;
using static Visuals.ConsoleVisuals;

using static Computer_Science_Problem.Language.LanguageDictonary;

namespace Computer_Science_Problem;

/// <summary> The Mainprogram class is the entry point of the program. from this point, every processing method is called. </summary>
public class MainProgram
{
    #region Field
    /// <summary>The jump variable, used as a thread to gat the information wether to continue the execution or to "jump" to a specific part of the code.</summary>
    public static Jump jump = Jump.Continue;
    #endregion

    #region Jump enumaration
    /// <summary> Enumerates the different jumps. </summary>
    public enum Jump
    {
        /// <summary> Continue the execution. </summary>
        Continue,
        /// <summary> Go to the main menu. </summary>
        Main_Menu,
        /// <summary> Go to the options menu. </summary>
        Options,
        /// <summary> Go to the language menu. </summary>
        Language,
        /// <summary> Go to the font color menu. </summary>
        FontColor,
        /// <summary> Go to the source folder chooser. </summary>
        Source_Folder,
        /// <summary> Go to the actions menu. </summary>
        Actions,
        /// <summary> Go to the specific kernel application. </summary>
        ApplyCustomKernel,
        /// <summary> Go to the manipulation application. </summary>
        ApplyManipulation,
        /// <summary> Go to the filter application. </summary>
        ApplyFilter,
        /// <summary> Exit the program. </summary>
        Exit
    }
    #endregion

    #region Main
    /// <summary> The entry point of the program. </summary>
    public static void Main(string[] args)
    {
        #region Processing
        IntializeDict();
        WriteFullScreen(default);

        Main_Menu:

        MainMenu();
        if (jump is Jump.Continue) 
            goto Source_Folder;
        goto Select;

        Options:

        Options();
        goto Select;

        Language:

        ChangeLanguage();
        goto Select;

        FontColor:

        ChangeColor();
        goto Select;

        Source_Folder:

        string folder = ChooseFolder();
        if (jump is Jump.Main_Menu) 
            goto Select;
        ChooseFile(folder);
        if (jump is not Jump.Continue) 
            goto Select;

        Actions:

        Actions();
        if (jump is Jump.ApplyFilter) 
            ApplyFilter();
        else if (jump is Jump.ApplyManipulation)
            ApplyManipulation();
        else if (jump is Jump.ApplyCustomKernel)
            ApplyCustomKernel();
        #endregion

        #region Selection
        Select:

        switch (jump)
        {
            case Jump.Main_Menu:
                jump = Jump.Continue;
                goto Main_Menu;
            case Jump.Options:
                jump = Jump.Continue;
                goto Options;
            case Jump.Language:
                jump = Jump.Continue;
                goto Language;
            case Jump.FontColor:
                jump = Jump.Continue;
                goto FontColor;
            case Jump.Source_Folder:
                jump = Jump.Continue;
                goto Source_Folder;
            case Jump.Actions:
                jump = Jump.Continue;
                goto Actions;
            case Jump.Exit:
                ProgramExit();
                break;
            default:
                jump = Jump.Main_Menu;
                goto Main_Menu;
        }
        #endregion
    }
    #endregion
    
}