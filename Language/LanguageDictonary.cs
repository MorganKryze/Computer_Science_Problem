using System.Text.Json;

namespace Language;

/// <summary> This class represents a language dictionary. </summary>
public static class LanguageDictonary
{
    /// <summary> This field represents the dictionary. </summary>
    public static Dictionary<string, Dictionary<string,  Dictionary<string, string>>> s_Dict { get; set; } = new();
    /// <summary> This field represents the current language. </summary>
    public static string s_Lang = "english";

    /// <summary> This method initializes the dictionary. </summary>
    public static void InitializeDict()
    {
        string jsonString = File.ReadAllText("Language/dataLanguages.json");

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        Dictionary<string, Dictionary<string,  Dictionary<string, string>>>? nullHandler = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string,  Dictionary<string, string>>>>(jsonString, options);
        s_Dict = nullHandler ?? throw new NullReferenceException("The dictionary is null.");
    }

}
