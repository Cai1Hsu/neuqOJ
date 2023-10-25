namespace neuqOJ.Utils;

public static class Strings
{
    /// <summary>
    /// Add `"` for string
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string StringToString(string s) => $"\"{s.Trim('\"')}\"";
}