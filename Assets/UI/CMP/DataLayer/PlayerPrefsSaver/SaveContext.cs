public static class SaveContext
{
    private static PlayerPrefsSaver saver = new PlayerPrefsSaver();

    public static string GetString(string key)
    {
        return saver.GetString(key);
    }
    
    public static void SaveString(string key, string value)
    {
        saver.SaveString(key, value);
    }
}