using UnityEngine;

public class PlayerPrefsSaver
{
    public string GetString(string key)
    {
        string result = null;
        if (PlayerPrefs.HasKey(key))
            result = PlayerPrefs.GetString(key);
        return result;
    }
    
    public void SaveString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        Save();
    }

    private void Save()
    {
        PlayerPrefs.Save();
    }
}