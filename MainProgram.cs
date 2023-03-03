using System.Diagnostics;
using System;
using System.Collections.Generic;

using static System.Console;
using static Computer_Science_Problem.Methods;

namespace Computer_Science_Problem
{
    /// <summary>The main program class.</summary>
    class MainProgram
    {
        public static Jump jump = Jump.Continue;
        /// <summary>The Main fonction.</summary>
        public static void Main(string[] args)
        {
            ConsoleConfiguration();
            
            Main_Menu :

            MainMenu();
            if(jump is not Jump.Continue) goto Select;
            goto Actions;

            Source_Folder :

            string folder = ChooseFolder();
            if(jump is Jump.Main_Menu) goto Select;
            ChooseFile(folder);
            goto Select;

            Actions :

            Actions();
            if(jump is not Jump.Continue) goto Select;
            goto Main_Menu;

            Select :

            switch(jump)
            {
                case Jump.Continue: 
                    break;
                case Jump.Main_Menu: 
                    jump = Jump.Continue;
                    goto Main_Menu;
                case Jump.Source_Folder:
                    jump = Jump.Continue;
                    goto Source_Folder;
                case Jump.Actions:
                    jump = Jump.Continue;
                    goto Actions;
                case Jump.Exit: 
                    FinalExit(); 
                    break;
            }
            
        } 
        public enum Jump
        {
            Continue,
            Main_Menu,
            Source_Folder,
            Actions,
            Exit
        }
    }
}
