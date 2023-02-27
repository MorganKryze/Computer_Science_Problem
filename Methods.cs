using System;

using static System.Console;
using static System.Environment;
using static System.Threading.Thread;
using static System.IO.File;
using static System.ConsoleColor;
using static System.ConsoleKey;

namespace Computer_Science_Problem
{
    /// <summary>The vocation of the Methods class is to be accessible from anywhere.It contains a random variable, utility and core methods.</summary>
    public static class Methods
    {
        #region Field
        /// <summary>The random variable, usable everywhere.</summary>
        public static Random rnd = new Random();
        public static string imagePath = "Images/lac.bmp";
        #endregion
        #region Core methods
        /// <summary>This method is used to display the main menu.</summary>
        public static void MainMenu()
        {
            switch(ScrollingMenu("Welcome User! Use the arrow keys to move and press [ENTER] to confirm.", new string[]{"Play    ","Options ","Quit    "}, "Title.txt"))
            {
                case 0 : 
                    MainProgram.jump = MainProgram.Jump.Continue; 
                    break;
                case 1 : 
                    switch(ScrollingMenu("Choisissez un fichier parmi les trois proposés.", new string[]{"coco   ","lac    ","lena   "}, "Title.txt"))
                    {
                        case 0 : 
                            imagePath = "Images/coco.bmp";
                            break;
                        case 1 : 
                            imagePath = "Images/lac.bmp";
                            break;
                        case 2 : 
                            imagePath = "Images/lena.bmp";
                            break;
                    }
                    MainProgram.jump = MainProgram.Jump.Main_Menu;
                    break;
                case 2 : case -1: 
                    MainProgram.jump = MainProgram.Jump.Exit;
                    break;
            }
        }
        public static void Choices()
        {
            Image image = new Image(imagePath);
            switch(ScrollingMenu("Choose one action to do on your image.", new string[]{
                "Greyscale       ",
                "Black and white ",
                "Rotate          ",
                "Resize          ",
                "Exit            "}, "Title.txt"))
            {
                case 0 : 
                    image = image.Greyscale();
                    break;
                case 1 : 
                    image = image.BlackAndWhite();
                    break;
                case 2 : 
                    int? angle = null;
                    do 
                    {
                        Title("Type an angle to rotate the picture.", "Title.txt");
                        Write("{0,"+((WindowWidth / 2) - ("Type an angle to rotate the picture.".Length / 2)) + "}","");
                        Write("> ");
                        ConsoleConfiguration(false);
                        angle = Convert.ToInt32(ReadLine());
                        ConsoleConfiguration();
                    }while(angle is null || angle < 0 || angle > 360);
                    if (angle is not null) image = image.Rotate((int)angle);
                    
                    break;
                case 3 : 
                    int? scale = null;
                    do 
                    {
                        Title("Type an angle to rotate the picture.", "Title.txt");
                        Write("{0,"+((WindowWidth / 2) - ("Type an angle to rotate the picture.".Length / 2)) + "}","");
                        Write("> ");
                        ConsoleConfiguration(false);
                        scale = Convert.ToInt32(ReadLine());
                        ConsoleConfiguration();
                    }while(scale is null || scale < 0 );
                    if (scale is not null) image = image.Scale((int)scale);
                    break;
                case 4 : case -1: 
                    MainProgram.jump = MainProgram.Jump.Exit;
                    break;
            }

            string fileName ="";
            do 
            {
                Title("Type the name of the file.", "Title.txt");
                Write("{0,"+((WindowWidth / 2) - ("Type the name of the file.".Length / 2)) + "}","");
                Write("> ");
                ConsoleConfiguration(false);
                fileName = ReadLine() ?? "";
                ConsoleConfiguration();
            }while(fileName == "");
            image.Save("Sorties/"+fileName+".bmp");
        }
        #endregion

        #region Utility Methods
        /// <summary> This method is used to set the console configuration. </summary>
        /// <param name="start"> Wether the config is used as the default config (true) or for the end of the program (false). </param>
        public static void ConsoleConfiguration(bool start = true)
        {
            if (start)
            {
                CursorVisible = false;
                BackgroundColor = Black;
                ForegroundColor = White;
            }
            else CursorVisible = true;
        }
        /// <summary>This method is used to display a title.</summary>
        /// <param name= "text"> The content of the title.</param>
        /// <param name= "pathSpecialText"> A special text stored in a file.</param>
        /// <param name= "recurrence"> Whether the title has been displayed yet or not.</param>
        public static void Title (string text = "",string pathSpecialText = "", int recurrence = 0)
        {
            Clear();
            if (pathSpecialText != "")PrintSpecialText(pathSpecialText);
            if(text != "")
            {
                if (recurrence != 0)CenteredWL(text);
                else
                {
                    Write("{0,"+((WindowWidth / 2) - (text.Length / 2)) + "}","");
                    for(int i = 0; i < text.Length; i++)
                    {
                        Write(text[i]);
                        Sleep(50);
                        if(KeyAvailable)
                        {
                            ConsoleKeyInfo keyPressed = ReadKey(true);
                            if(keyPressed.Key == Enter||keyPressed.Key == Escape)
                            {
                                Write(text.Substring(i+1));
                                break;
                            }
                        }
                    }
                    Write("\n");
                }
                Write("\n");
                
            }
            
        }
        /// <summary>This method is used to display a scrolling menu.</summary>
        /// <param name="question"> The question to be displayed.</param>
        /// <param name="choices"> The choices to be displayed.</param>
        /// <param name="specialText"> A special text stored in a file.</param>
        /// <returns> The position of the choice selected.</returns>
        public static int ScrollingMenu(string question, string[] choices, string specialText = "Title.txt")
        {
            int position = 0;
            int recurrence = 0;
            while(true)
            {
                Clear();
                Title(question,specialText,recurrence);
                string[]currentChoice = new string[choices.Length];
                for (int i = 0; i < choices.Length; i++)
                {
                    if (i == position)
                    {
                        currentChoice[i] = $" > {choices[i]}";
                        CenteredWL(currentChoice[i], Black, Green);
                        ConsoleConfiguration();
                    }
                    else 
                    {
                        currentChoice [i]= $"   {choices[i]}";
                        CenteredWL(currentChoice[i]);
                    }
                }
                switch(ReadKey().Key)
                {
                    case UpArrow : case Z : if(position == 0)position = choices.Length-1; else if(position > 0) position--;break;
                    case DownArrow : case S : if(position == choices.Length-1)position = 0; else if(position < choices.Length-1)position++;break;
                    case Enter : return position;
                    case Escape : return -1;
                }
                recurrence++;
            }
        }
        /// <summary>This method is used to display a special text, stored in a txt file. </summary>
        /// <param name="path">the relative path of the file.</param>
        public static void PrintSpecialText(string path)
        {
            string[] specialText = ReadAllLines(path);
            foreach(string line in specialText)WriteLine("{0," + ((WindowWidth / 2) + (line.Length / 2)) + "}", line);
            WriteLine("\n");
        }
        /// <summary>This method is used to display a centered text and come back to line.</summary>
        /// <param name="text"> The text to display. </param>
        /// <param name="fore"> The foreground color of the text. </param>
        /// <param name="back"> The background color of the text. </param>
        public static void CenteredWL(string text, ConsoleColor fore = White, ConsoleColor back = Black)
        {
            ConsoleConfiguration();
            Write("{0,"+((WindowWidth / 2) - (text.Length / 2)) + "}","");
            ForegroundColor = fore;
            BackgroundColor = back;
            WriteLine(text);
            ConsoleConfiguration();
        }
        /// <summary>This method is used to display a centered text.</summary>
        /// <param name="text"> The text to display. </param>
        /// <param name="fore"> The foreground color of the text. </param>
        /// <param name="back"> The background color of the text. </param>
        public static void CenteredW(string text, ConsoleColor fore = White, ConsoleColor back = Black)
        {
            ConsoleConfiguration();
            Write("{0,"+((WindowWidth / 2) - (text.Length / 2)) + "}","");
            ForegroundColor = fore;
            BackgroundColor = back;
            Write(text);
            ConsoleConfiguration();
        }   

        /// <summary>This method is used to print a message telling whether the player has won or not.</summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="backColor">The background color of the message.</param>
        public static void BoardMessage(string[] message, ConsoleColor backColor = Green)
        {
            Clear();
            WriteLine();
            string max = message[0];
            for (int i = 0; i < message.Length; i++)
            {
                if (max.Length < message[i].Length) max = message[i];
            }
            int index = Array.IndexOf(message, max);
            CenteredWL(String.Format("{0,"+message[index].Length+"}", ""), Black, backColor);
            for (int i = 0; i < message.Length; i++)
            {
                if (message[index].Length > message[i].Length)
                {
                    for (int j = 0; message[index].Length != message[i].Length; j++ )
                    {
                        if(j % 2 == 0)message[i] +=" ";
                        else message[i] = " " + message[i];
                    }
                }
                CenteredWL(String.Format("{0,"+message[index].Length+"}", message[i]), Black, backColor);
            }
            CenteredWL(String.Format("{0,"+message[index].Length+"}", ""), Black, backColor);
            WriteLine();
        }
        /// <summary>This method is used to display a loading screen.</summary>
        /// <param name="text"> The text to display. </param>
        public static void LoadingScreen(string text)
        {
            Clear();
            int t_interval = (int) 2000/text.Length;
            char[] loadingBar = new char[text.Length];
            for (int j = 0; j < loadingBar.Length; j++) loadingBar[j] = '█';
            for (int i = 0; i <= text.Length; i++)
            {
                Title(text, "Title.txt", 1);
                if(KeyAvailable)
                {
                    ConsoleKeyInfo keyPressed = ReadKey(true);
                    if(keyPressed.Key == Enter||keyPressed.Key == Escape)
                    {
                        i = text.Length;
                        break;
                    }
                }
                WriteLine("\n");
                string bar = "";
                for (int l = 0; l < i; l++) bar += loadingBar[l];
                Write("{0,"+((WindowWidth / 2) - (text.Length / 2)) + "}","");
                ForegroundColor = Green;
                Write(bar);
                bar = ""; 
                if (i != text.Length)
                {
                    for (int l = i; l < text.Length; l++)bar += loadingBar[l];
                    ForegroundColor = Red;
                    Write(bar);
                    Sleep(t_interval);
                }
                else Sleep(1000);
                ConsoleConfiguration();
                Clear();
            }
        }
        /// <summary>This method is used to pause the program.</summary>
        public static void Pause()
        {
            CenteredWL("Press [ENTER] to continue...");
            while(ReadKey(true).Key != Enter) Sleep(5);
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
}