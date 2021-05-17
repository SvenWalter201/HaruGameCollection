using System;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public static class JsonFileManager
{
    public static bool Save<T>(string fileName, T content)
    {
        try
        {
            string jsonString = JsonConvert.SerializeObject(content);
            File.WriteAllText(fileName, jsonString);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
            return false;
        }

    }

    public static bool Load<T>(string path, out T content)
    {
        if (File.Exists(path))
        {
            try
            {
                string jsonString = File.ReadAllText(path);
                content = JsonConvert.DeserializeObject<T>(jsonString);
                return true;
            }
            catch(Exception e)
            {
                Debug.LogWarning(e.Message);
                content = default;
                return false;
            }

        }
        Debug.LogWarning("File not found");
        content = default;
        return false;
    }
}
