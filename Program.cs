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
        public static void Main()
        {
            
            #region Config
            ConsoleConfiguration();
            #endregion

            Main_Menu :

            #region Lobby
            MainMenu();
            if(jump is not Jump.Continue) goto Select;
            #endregion

            Choices :

            #region Choices
            Choices();
            if(jump is not Jump.Continue) goto Select;
            #endregion
            
            goto Main_Menu;

            Select :

            switch(jump)
            {
                case Jump.Continue: 
                    break;
                case Jump.Main_Menu: 
                    jump = Jump.Continue;
                    goto Main_Menu;
                case Jump.Choices:
                    jump = Jump.Continue;
                    goto Choices;
                case Jump.Exit: 
                    FinalExit(); 
                    break;
            }
        } 
        public enum Jump
        {
            Continue,
            Main_Menu,
            Choices,
            Exit
        }
    }
}
