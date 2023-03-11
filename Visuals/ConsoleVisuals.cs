using static System.Console;
using static System.Threading.Thread;
using static System.IO.File;
using static System.ConsoleColor;
using static System.ConsoleKey;

namespace Visuals;
/// <summary> The class ConsoleVisuals contains all the methods used to display the app on the console. </summary>
public static class ConsoleVisuals
{
    #region Constants
    /// <summary> The path to the title file. </summary>
    public const string titlePath = "Images/Title/title.txt";
    #endregion

    #region Print on screen
    /// <summary>This method is used to display a title.</summary>
    /// <param name= "text"> The content of the title.</param>
    /// <param name= "pathSpecialText"> A special text stored in a file.</param>
    /// <param name= "occurrence"> Whether the title has been displayed yet or not.</param>
    public static void Title(string text = "", string pathSpecialText = "", int occurrence = 0)
    {
        if (occurrence <  0)
            throw new ArgumentException("The occurrence must be positive or equal to 0.");
        Clear();
        if(pathSpecialText != "") PrintSpecialText(pathSpecialText);
        if(text != "")
        {
            if(occurrence != 0) CenteredWL(text);
            else
            {
                Write("{0," + ((WindowWidth / 2) - (text.Length / 2)) + "}", "");
                for(int i = 0; i < text.Length; i++)
                {
                    Write(text[i]);
                    Sleep(50);
                    if(KeyAvailable)
                    {
                        ConsoleKeyInfo keyPressed = ReadKey(true);
                        if(keyPressed.Key == Enter || keyPressed.Key == Escape)
                        {
                            Write(text.Substring(i + 1));
                            break;
                        }
                    }
                }
                Write("\n");
            }
            Write("\n");

        }

    }
    /// <summary>This method is used to display a special text, stored in a txt file. </summary>
    /// <param name="path">the relative path of the file.</param>
    public static void PrintSpecialText(string path)
    {
        string[] specialText = ReadAllLines(path);
        foreach(string line in specialText) WriteLine("{0," + ((WindowWidth / 2) + (line.Length / 2)) + "}", line);
        WriteLine("\n");
    }
    /// <summary>This method is used to display a centered text and come back to line.</summary>
    /// <param name="text"> The text to display. </param>
    /// <param name="fore"> The foreground color of the text. </param>
    /// <param name="back"> The background color of the text. </param>
    public static void CenteredWL(string text, ConsoleColor fore = White, ConsoleColor back = Black)
    {
        ConsoleConfiguration();
        Write("{0," + ((WindowWidth / 2) - (text.Length / 2)) + "}", "");
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
        Write("{0," + ((WindowWidth / 2) - (text.Length / 2)) + "}", "");
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
        for(int i = 0; i < message.Length; i++)
        {
            if(max.Length < message[i].Length) max = message[i];
        }
        int index = Array.IndexOf(message, max);
        CenteredWL(String.Format("{0," + message[index].Length + "}", ""), Black, backColor);
        for(int i = 0; i < message.Length; i++)
        {
            if(message[index].Length > message[i].Length)
            {
                for(int j = 0; message[index].Length != message[i].Length; j++)
                {
                    if(j % 2 == 0) message[i] += " ";
                    else message[i] = " " + message[i];
                }
            }
            CenteredWL(String.Format("{0," + message[index].Length + "}", message[i]), Black, backColor);
        }
        CenteredWL(String.Format("{0," + message[index].Length + "}", ""), Black, backColor);
        WriteLine();
    }
    /// <summary>This method is used to display a prompt and get a string in return.</summary>
    /// <param name="message"> The message to be displayed. </param>
    /// <param name="occurrence"> Whether the prompt has been displayed yet or not. </param>
    /// <param name="specialtext"> A special text stored in a file. </param>
    /// <returns> The string entered by the user. </returns>
    public static string Prompt(string message, int occurrence = 0, string specialtext = "")
    {
        string prompt = "";
        do
        {
            Title(message, specialtext, occurrence);
            Write("{0," + ((WindowWidth / 2) - (message.Length / 2)) + "}", "");
            Write("> ");
            ConsoleConfiguration(false);
            prompt = ReadLine() ?? "";
            ConsoleConfiguration();
            occurrence++;
        } while (prompt is "");
        return prompt;
    }
    #endregion

    #region General
    /// <summary> This method is used to set the console configuration. </summary>
    /// <param name="start"> Wether the config is used as the default config (true) or for the end of the program (false). </param>
    public static void ConsoleConfiguration(bool start = true)
    {
        if(start)
        {
            CursorVisible = false;
            BackgroundColor = Black;
            ForegroundColor = White;
        }
        else CursorVisible = true;
    }
    /// <summary>This method is used to display a scrolling menu.</summary>
    /// <param name="question"> The question to be displayed.</param>
    /// <param name="choices"> The choices to be displayed.</param>
    /// <param name="specialText"> A special text stored in a file.</param>
    /// <returns> The position of the choice selected.</returns>
    public static int ScrollingMenu(string question, string[] choices, string specialText = titlePath)
    {
        int position = 0;
        int recurrence = 0;
        int longestChoice = 0;
        for (int i = 0; i < choices.Length; i++) if (choices[i].Length > longestChoice) longestChoice = choices[i].Length;
        for (int i = 0; i < choices.Length; i++) choices[i] = choices[i].PadRight(longestChoice + 1);
        while (true)
        {
            Clear();
            Title(question, specialText, recurrence);
            string[] currentChoice = new string[choices.Length];
            for (int i = 0; i < choices.Length; i++)
            {
                if (i == position)
                {
                    currentChoice[i] = $" > {choices[i]}";
                    CenteredWL(currentChoice[i], Black, White);
                    ConsoleConfiguration();
                }
                else
                {
                    currentChoice[i] = $"   {choices[i]}";
                    CenteredWL(currentChoice[i]);
                }
            }
            switch (ReadKey().Key)
            {
                case UpArrow: case Z: if (position == 0) position = choices.Length - 1; else if (position > 0) position--; break;
                case DownArrow: case S: if (position == choices.Length - 1) position = 0; else if (position < choices.Length - 1) position++; break;
                case Enter: return position;
                case Escape: return -1;
            }
            recurrence++;
        }
    }
    /// <summary>This method is used to display a loading screen.</summary>
    /// <param name="text"> The text to display. </param>
    public static void LoadingScreen(string text)
    {
        Clear();
        int t_interval = (int)2000 / text.Length;
        char[] loadingBar = new char[text.Length];
        for(int j = 0; j < loadingBar.Length; j++) loadingBar[j] = '█';
        for(int i = 0; i <= text.Length; i++)
        {
            Title(text, titlePath, 1);
            if(KeyAvailable)
            {
                ConsoleKeyInfo keyPressed = ReadKey(true);
                if(keyPressed.Key == Enter || keyPressed.Key == Escape)
                {
                    i = text.Length;
                    break;
                }
            }
            WriteLine("\n");
            string bar = "";
            for(int l = 0; l < i; l++) bar += loadingBar[l];
            Write("{0," + ((WindowWidth / 2) - (text.Length / 2)) + "}", "");
            ForegroundColor = Green;
            Write(bar);
            bar = "";
            if(i != text.Length)
            {
                for(int l = i; l < text.Length; l++) bar += loadingBar[l];
                ForegroundColor = Red;
                Write(bar);
                Sleep(t_interval);
            }
            else Sleep(1000);
            ConsoleConfiguration();
            Clear();
        }
    }
    #endregion
}
