using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace Computer_Science_Problem.Language;

/// <summary> This class represents a language dictionary. </summary>
public static class LanguageDictonary
{
    /// <summary> This field represents the dictionary. </summary>
    public static Dictionary<string, Dictionary<string,  Dictionary<string, string>>> Dict { get; set; } = new();
    /// <summary> This field represents the current language. </summary>
    public static string s_Lang = "english";

    /// <summary> This method initializes the dictionary. </summary>
    public static void IntializeDict()
    {
        string jsonString = File.ReadAllText("Language/dataLanguages.json");

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        Dictionary<string, Dictionary<string,  Dictionary<string, string>>>? nullHandler = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string,  Dictionary<string, string>>>>(jsonString, options);
        Dict = nullHandler ?? throw new NullReferenceException("The dictionary is null.");
    }

}
