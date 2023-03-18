using static System.Console;
using static System.Threading.Thread;
using static System.IO.File;
using static System.ConsoleColor;
using static System.ConsoleKey;

using static Computer_Science_Problem.Language.LanguageDictonary;

namespace Visuals;

/// <summary> The <see cref="ConsoleVisuals"/> classe contains all the visual elements for a console app. </summary>
public static class ConsoleVisuals
{
    #region Attributes
    private const string titlePath = "Images/Title/title.txt";
    private static  string[] titleContent = ReadAllLines(titlePath);
    private static int initialWindowWidth = WindowWidth;
    private static string initialLanguage = CurrentLanguage;
    private static (ConsoleColor,ConsoleColor) initialColorpanel = (ForegroundColor, BackgroundColor);
    private static (ConsoleColor,ConsoleColor) colorPanel = (White, Black);
    #endregion

    #region Properties
    private static (string, string, string) defaultHeader => (Dict[CurrentLanguage]["DefaultHeader1"], Dict[CurrentLanguage]["DefaultHeader2"], Dict[CurrentLanguage]["DefaultHeader3"]);
    private static (string, string, string) defaultFooter => (Dict[CurrentLanguage]["DefaultFooter1"], Dict[CurrentLanguage]["DefaultFooter2"], Dict[CurrentLanguage]["DefaultFooter3"]);
    private static int TitleHeight => titleContent.Length;
    private static int HeaderHeigth => TitleHeight ;
    private static int FooterHeigth => WindowHeight - 2;
    private static int ContentHeigth => HeaderHeigth + 1;
    #endregion

    #region Enums
    /// <summary> The <see cref="Placement"/> enum defines the placement of a string in the console. </summary>
    public enum Placement
    {
        /// <summary> The string is placed at the left of the console. </summary>
        Left,
        /// <summary> The string is placed at the center of the console. </summary>
        Center,
        /// <summary> The string is placed at the right of the console. </summary>
        Right
    }
    #endregion

    #region Utility ethods

    private static void SetColor(bool negative = false)
    {
        ForegroundColor = negative ? colorPanel.Item2 : colorPanel.Item1;
        BackgroundColor = negative ? colorPanel.Item1 : colorPanel.Item2;
    }
    
    private static void WritePositionnedString(string str, Placement position = Placement.Center, bool negative = false, int line = -1, bool chariot = false)
	{
        SetColor(negative);
		if (line < 0) 
            line = Console.CursorTop;
		if (str.Length < Console.WindowWidth) 
            switch (position)
		    {
		    	case (Placement.Left): 
                    SetCursorPosition(0, line); 
                    break;
		    	case (Placement.Center): 
                    SetCursorPosition((WindowWidth - str.Length) / 2, line); 
                    break;
		    	case (Placement.Right): 
                    SetCursorPosition(WindowWidth - str.Length, line); 
                    break;
		    }
		else SetCursorPosition(0, line);
		if (chariot) WriteLine(str);
        else Write(str);
        SetColor(default);
	}

    private static void ClearLine(int line)
	{
		SetColor(default);
		WritePositionnedString("".PadRight(Console.WindowWidth), Placement.Left, default, line);
	}

    private static void ClearPanel()
    {
        for (int i = ContentHeigth; i < FooterHeigth; i++)
            ClearLine(i);
    }

    private static void ClearAll()
    {
        colorPanel = initialColorpanel;
        for (int i = 0; i < WindowHeight; i++)
            ContinuousPrint("".PadRight(WindowWidth), i, default, 100, 10);
        Clear();
        colorPanel = (White, Black);
    }
    
    private static void ContinuousPrint(string text, int line, bool negative = false, int stringTime = 2000, int endStringTime = 1000)
    {
        int t_interval = (int)(stringTime / text.Length);
        for (int i = 0; i <= text.Length; i++)
        {
            string continuous = "";
            for(int j = 0; j < i; j++) 
                continuous += text[j];
            continuous = continuous.PadRight(text.Length);
            WritePositionnedString(continuous.BuildString(WindowWidth, Placement.Center), default, negative, line);

            if(i != text.Length)
                Sleep(t_interval);

            if(KeyAvailable)
            {
                ConsoleKeyInfo keyPressed = ReadKey(true);
                if(keyPressed.Key == Enter || keyPressed.Key == Escape)
                {
                    i = text.Length;
                    break;
                }
            }
        }
        WritePositionnedString(text.BuildString(WindowWidth, Placement.Center), default, negative, line);
        Sleep(endStringTime);
    }
    
    private static void ReloadScreen()
    {
        if (WindowWidth != initialWindowWidth || CurrentLanguage != initialLanguage)
        {
            WriteFullScreen(true);
            initialWindowWidth = WindowWidth;
            initialLanguage = CurrentLanguage;
        }
        else
            ClearPanel();
    }
    #endregion

    #region Processing methods
    /// <summary> This method prints the title of the console app. </summary>
    public static void PrintTitle()
    {
        Clear();
        SetCursorPosition(0, 0);
        foreach (string line in titleContent)
        {
            WritePositionnedString(line.BuildString(WindowWidth, Placement.Center));
            WriteLine("");
        } 
    }
    /// <summary> This method prints a banner in the console. </summary>
    /// <param name="banner"> The banner to print. </param>
    /// <param name="header"> If true, the banner is printed at the top of the console. If false, the banner is printed at the bottom of the console. </param>
    /// <param name="straight"> If true, the title is not continuously printed. </param>
    public static void WriteBanner((string, string, string)? banner = null, bool header = true, bool straight = false)
	{
        (string, string, string) newBanner = banner ??= header ? defaultHeader : defaultFooter;

		SetColor(true);
		string strBanner = newBanner.Item2.BuildString(Console.WindowWidth, Placement.Center, true);
		strBanner = strBanner.Substring(0, strBanner.Length - newBanner.Item3.Length) + newBanner.Item3;
		strBanner = newBanner.Item1 + strBanner.Substring(newBanner.Item1.Length);
        if (straight) 
            WritePositionnedString(strBanner, default, true, header ? HeaderHeigth : FooterHeigth);
        else
		    ContinuousPrint(strBanner, header ? HeaderHeigth : FooterHeigth, true);
		SetColor();
	}
    /// <summary> This method prints a full screen in the console. </summary>
    /// <param name="straight"> If true, the title is not continuously printed. </param>
    /// <param name="header"> The header of the screen. </param>
    /// <param name="footer"> The footer of the screen. </param>
    public static void WriteFullScreen(bool straight = false, (string, string, string)? header = null, (string, string, string)? footer = null)
    {
        header ??= defaultHeader;
        footer ??= defaultFooter;
        CursorVisible = false;
        PrintTitle();
        WriteBanner(header, true, straight);
        WriteBanner(footer, false, straight);
        ClearPanel();
        if (!straight) 
            LoadingScreen(Dict[CurrentLanguage]["FirstLoadingTitle"]);
    }
    /// <summary> This method prints a message in the console and gets a string written by the user. </summary>
    /// <param name="message"> The message to print. </param>
    /// <returns> The string written by the user. </returns>
    public static string WritePrompt(string message)
    {
        ReloadScreen();
        ContinuousPrint(message.BuildString(message.Length, Placement.Center), ContentHeigth + 1, default, 1500, 50);
        string prompt = "";
        do
        {
            ClearLine(ContentHeigth + 2);
            Write("{0," + ((WindowWidth / 2) - (message.Length / 2) + 2) + "}", "> ");
            CursorVisible = true;
            prompt = ReadLine() ?? "";
            CursorVisible = false;
        } while (prompt is "");
        return prompt;
    }
    /// <summary> This method prints a paragraph in the console. </summary>
    /// <param name="lines"> The lines of the paragraph. </param>
    /// <param name="negative"> If true, the paragraph is printed in the negative colors. </param>
    /// <param name="truncate"> If true, the paragraph is truncated if it is too long. </param>
    public static void WriteParagraph(IEnumerable<string> lines, bool negative = false, bool truncate = true)
	{
        ReloadScreen();
		int maxLength = lines.Count() > 0 ? lines.Max(s => s.Length) : 0;
		if (truncate) maxLength = Math.Min(maxLength, Console.WindowWidth);
        
		SetColor();
		int i = TitleHeight + 2;
		foreach (string line in lines)
		{
			WritePositionnedString(line.BuildString(maxLength, Placement.Center), Placement.Center, negative, i++);
			if (truncate && i >= Console.WindowHeight - 1) 
                break;
		}
		Console.ReadKey(true);
        SetColor();
	}
    /// <summary> This method prints a menu in the console and gets the choice of the user. </summary>
    /// <param name="question"> The question to print. </param>
    /// <param name="choices"> The choices of the menu. </param>
    /// <returns> The choice of the user. </returns>
    public static int ScrollingMenu(string question, string[] choices)
    {
        ReloadScreen();
        int startDisplayposition = ContentHeigth + 1;
        int currentPosition = 0;
        int maxLength = choices.Count() > 0 ? choices.Max(s => s.Length) : 0;

        for (int i = 0; i < choices.Length; i++) 
            choices[i] = choices[i].PadRight(maxLength + 1);
        ContinuousPrint(question, startDisplayposition, default, 1500, 50);
        while (true)
        {
            string[] currentChoice = new string[choices.Length];
            for (int i = 0; i < choices.Length; i++)
            {
                if (i == currentPosition)
                {
                    currentChoice[i] = $" > {choices[i]}";
                    WritePositionnedString(currentChoice[i], Placement.Center, true, startDisplayposition + 2 + i);
                    continue;
                }
                currentChoice[i] = $"   {choices[i]}";
                WritePositionnedString(currentChoice[i], Placement.Center, false, startDisplayposition + 2 + i);
            }
            switch (ReadKey(true).Key)
            {
                case UpArrow: case Z: 
                    if (currentPosition == 0) 
                        currentPosition = choices.Length - 1; 
                    else if (currentPosition > 0)
                        currentPosition--; 
                        break;
                case DownArrow: case S: 
                    if (currentPosition == choices.Length - 1) 
                        currentPosition = 0;  
                    else if (currentPosition < choices.Length - 1) 
                        currentPosition++; 
                        break;
                case Enter: 
                    return currentPosition;
                case Escape: 
                    return -1;
            }
        }
    }
    /// <summary> This method prints a loading screen in the console. </summary>
    /// <param name="text"> The text to print. </param>
    public static void LoadingScreen(string text)
    {
        ReloadScreen();
        WritePositionnedString(text.BuildString(WindowWidth, Placement.Center), default, default, ContentHeigth + 1, true);
        string loadingBar = "";
            for(int j = 0; j < text.Length; j++) 
                loadingBar += '█';
        ContinuousPrint(loadingBar, ContentHeigth + 3);
    }
    /// <summary> This method exits the program. </summary>
    public static void ProgramExit()
    {
        LoadingScreen(Dict[CurrentLanguage]["LastLoadingTitle"]);
        ClearAll();
        CursorVisible = true;
        Environment.Exit(0);
    }
    #endregion

    #region Extensions  
    /// <summary> This method builds a string with a specific size and a specific placement. </summary>
    /// <param name="str"> The string to build. </param>
    /// <param name="size"> The size of the string. </param>
    /// <param name="position"> The placement of the string. </param>
    /// <param name="truncate"> If true, the string is truncated if it is too long. </param>
    /// <returns> The built string. </returns>
    public static string BuildString(this string str, int size, Placement position = Placement.Center, bool truncate = true)
	{
		int padding = size - str.Length;
        if (truncate && padding < 0) 
            switch (position)
		    {
		    	case (Placement.Left): 
                    return str.Substring(0, size);
		    	case (Placement.Center): 
                    return str.Substring((- padding) / 2, size);
		    	case (Placement.Right): 
                    return str.Substring(- padding, size);
		    }
        else 
		    switch (position)
		    {
		    	case (Placement.Left):
		    		return str.PadRight(size);
		    	case (Placement.Center):
		    		return str.PadLeft(padding / 2 + padding % 2 + str.Length).PadRight(padding + str.Length);
		    	case (Placement.Right):
		    		return str.PadLeft(size);
		    }
		return str;
	}
    #endregion
}