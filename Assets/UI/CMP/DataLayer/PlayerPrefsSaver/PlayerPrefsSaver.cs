using System.Text.Json;
using UnityEngine;

public class PlayerPrefsSaver
{
    public LocalState GetLocalState(string key)
    {
        LocalState result = new LocalState();
        if (PlayerPrefs.HasKey(key))
        {
            var json = PlayerPrefs.GetString(key);
            var localState = JsonSerializer.Deserialize<LocalState>(json);
            result = localState;
        }
        return result;
    }
    
    public string GetString(string key)
    {
        string result = null;
        if (PlayerPrefs.HasKey(key))
            result = PlayerPrefs.GetString(key);
        return result;
    }

    public void SaveInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        Save();
    }
    
    public void SaveString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        Save();
    }
    
    public int GetInt(string key)
    {
        if(PlayerPrefs.HasKey(key))
            return PlayerPrefs.GetInt(key);
        else
            return -1;
    }
    
    private void Save()
    {
        PlayerPrefs.Save();
    }
}