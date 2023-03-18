using static Visuals.ConsoleVisuals;

using static System.Console;
using static System.Environment;
using static System.Threading.Thread;
using static System.ConsoleKey;

using static Computer_Science_Problem.Language.LanguageDictonary;

namespace Computer_Science_Problem;

/// <summary>The vocation of the Methods class is to be accessible from anywhere.It contains a random variable, utility and core methods.</summary>
public static class GeneralMethods
{  
    #region Processing methods
    /// <summary>This method is used to display the main menu.</summary>
    public static void MainMenu()
    {
        switch (ScrollingMenu(Dict[CurrentLanguage]["MainMenuTitle"], new string[] { 
            Dict[CurrentLanguage]["MainMenuButton1"], 
            Dict[CurrentLanguage]["MainMenuButton2"],
            Dict[CurrentLanguage]["MainMenuButton3"], }))
        {
            case 0:
                MainProgram.jump = MainProgram.Jump.Continue;
                break;
            case 1:
                MainProgram.jump = MainProgram.Jump.FutureLanguageFeature;
                break;
            case 2: case -1:
                MainProgram.jump = MainProgram.Jump.Exit;
                break;
        }
    }
    /// <summary>This method is used to display the Image source folder chooser.</summary>
    public static string ChooseFolder()
    {
        switch (ScrollingMenu(Dict[CurrentLanguage]["FolderTitle"], new string[]{
                Dict[CurrentLanguage]["FolderButton1"],
                Dict[CurrentLanguage]["FolderButton2"],
                Dict[CurrentLanguage]["FolderButton3"]}))
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
        switch (namePosition = ScrollingMenu(Dict[CurrentLanguage]["BrowseFilesTitle"], filesName))
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
    /// <summary> Not implemented yet. </summary>
    public static void FutureLanguageFeature()
    {
        switch(ScrollingMenu("This feature is not implemented yet !", new string[]{"Back"}))
        {
            default:
                MainProgram.jump = MainProgram.Jump.Main_Menu;
                break;
        }
    }
    /// <summary>This method is used to display the actions menu.</summary>
    public static void Actions()
    {
        switch (ScrollingMenu(Dict[CurrentLanguage]["ActionTitle"], new string[]{
                Dict[CurrentLanguage]["ActionButton1"],
                Dict[CurrentLanguage]["ActionButton2"],
                Dict[CurrentLanguage]["ActionButton3"]}))
        {
            case 0:
                MainProgram.jump = MainProgram.Jump.ApplyFilter;
                break;
            case 1:
                MainProgram.jump = MainProgram.Jump.ApplyManipulation;
                break;
            case 2: case -1:
                MainProgram.jump = MainProgram.Jump.Source_Folder;
                break;
        }
    }
    /// <summary>This method is used to display the filters menu.</summary>
    public static void ApplyFilter()
    {
        Image image = new (Image.imagePath);
        switch (ScrollingMenu(Dict[CurrentLanguage]["ApplyFilterTitle"], new string[]{
                Dict[CurrentLanguage]["ApplyFilterButton1"],
                Dict[CurrentLanguage]["ApplyFilterButton2"],
                Dict[CurrentLanguage]["ApplyFilterButton3"],
                Dict[CurrentLanguage]["ApplyFilterButton4"],
                Dict[CurrentLanguage]["ApplyFilterButton5"],
                Dict[CurrentLanguage]["ApplyFilterButton6"]}))
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
        switch (ScrollingMenu(Dict[CurrentLanguage]["ApplyManipulationTitle"], new string[]{
                Dict[CurrentLanguage]["ApplyManipulationButton1"],
                Dict[CurrentLanguage]["ApplyManipulationButton2"],
                Dict[CurrentLanguage]["ApplyManipulationButton3"],
                Dict[CurrentLanguage]["ApplyManipulationButton4"],
                Dict[CurrentLanguage]["ApplyManipulationButton5"]}))
        {
            case 0:
                int? angle = null;
                int occurrenceRotation = 0;
                do
                {   string answer = WritePrompt(Dict[CurrentLanguage]["RotatePrompt"]);
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
                    string answer = WritePrompt(Dict[CurrentLanguage]["ResizePrompt"]);
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