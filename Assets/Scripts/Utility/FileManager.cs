using System;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public static class FileManager
{
    public static bool SaveJSON<T>(string fileName, T content)
    {
        try
        {
            string jsonString = JsonConvert.SerializeObject(content, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            File.WriteAllText(fileName, jsonString);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
            return false;
        }

    }

    public static string[] GetDirectoryContentsFromEditableResources(string subDirectory)
    {
#if UNITY_EDITOR
        string path = Application.dataPath;
        path = path.Substring(0, path.Length - 6) + "EditableResources/" + subDirectory;
        return GetDirectoryContents(path);
#else
        string path = Application.dataPath;
        path = path.Substring(0, path.Length - 6) + "EditableResources/" + subDirectory;
        return GetDirectoryContents(path);
#endif 
    }
    public static string[] GetDirectoryContents(string directory) =>
        Directory.GetFiles(directory);

    public static bool LoadFromEditableResources<T>(string fileName, out T content)
    {
        string path = Application.dataPath;

#if UNITY_EDITOR
        path = path.Substring(0, path.Length - 6) + "EditableResources/" + fileName + ".json";
        return LoadJSON(path, out content);


#else
        path += "/EditableResources/" + fileName + ".json";
        return LoadJSON(path, out content);
#endif 
    }

    public static bool SaveToEditableResources<T>(string fileName, T content)
    {
        string path = Application.dataPath;


#if UNITY_EDITOR
        path = path.Substring(0, path.Length - 6) + "EditableResources";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        path += "/" + fileName + ".json";
        return SaveJSON(path, content);
#else
        path += "/EditableResources";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        path += "/" + fileName + ".json";
        return SaveJSON(path, content);
#endif
    }

    public static bool SaveJSONToResources<T>(string fileName, out T content)
    {
        TextAsset a = Resources.Load<TextAsset>(fileName);
        if (!a)
        {
            Debug.LogError("Missing asset: " + fileName + " could not be found.");
            content = default;
            return false;
        }

        string jsonString = a.text;
        content = JsonConvert.DeserializeObject<T>(jsonString);

        return true;
    }
    public static bool LoadJSONFromResources<T>(string fileName, out T content)
    {
        TextAsset a = Resources.Load<TextAsset>(fileName);
        if (!a)
        {
            Debug.LogError("Missing asset: " + fileName + " could not be found.");
            content = default;
            return false;
        }

        string jsonString = a.text;
        content = JsonConvert.DeserializeObject<T>(jsonString);

        return true;
    }

    public static bool LoadJSON<T>(string path, out T content)
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

    public static bool LoadPNG(string filePath, out Texture2D tex)
    {
        tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            return true;
        }
        Debug.LogWarning("File doesn't exist");
        return false;
    }


    public static bool LoadObject(string filePrefix, string fileName, out UnityEngine.Object o)
    {
        o = Resources.Load(filePrefix + "/" + fileName);
        if (!o)
        {
            Debug.LogError("Missing asset: " + filePrefix + "/" + fileName + " could not be found.");
            return false;
        }
        return true;
    }


}
