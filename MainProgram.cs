using static Visuals.ConsoleVisuals;
using static Computer_Science_Problem.GeneralMethods;

namespace Computer_Science_Problem;

class MainProgram
{
    #region Field
    /// <summary>The jump variable, used as a thread to gat the information wether to continue the execution or to "jump" to a specific part of the code.</summary>
    public static Jump jump = Jump.Continue;
    #endregion

    #region Enum
    public enum Jump
    {
        Continue,
        Main_Menu,
        FutureLanguageFeature,
        Source_Folder,
        Actions,
        ApplyManipulation,
        ApplyFilter,
        Exit
    }
    #endregion

    #region Main
    public static void Main(string[] args)
    {
        ConsoleConfiguration();
        
        Main_Menu:

        MainMenu();
        if (jump is not Jump.Continue) 
            goto Select;
        goto Source_Folder;

        FutureLanguageFeature:

        FutureLanguageFeature();
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

        Select:

        switch (jump)
        {
            case Jump.Main_Menu:
                jump = Jump.Continue;
                goto Main_Menu;
            case Jump.FutureLanguageFeature:
                jump = Jump.Continue;
                goto FutureLanguageFeature;
            case Jump.Source_Folder:
                jump = Jump.Continue;
                goto Source_Folder;
            case Jump.Actions:
                jump = Jump.Continue;
                goto Actions;
            case Jump.Exit:
                FinalExit();
                break;
            default:
                goto Main_Menu;
        }
    }
    #endregion
    
}

