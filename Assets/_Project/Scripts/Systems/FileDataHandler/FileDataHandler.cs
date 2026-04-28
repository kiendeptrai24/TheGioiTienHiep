using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public class FileDataHandler<T> where T : class
{
    private readonly string dataDirPath;
    private readonly string dataFileName;
    private readonly bool encryptData;
    private readonly string codeWord = "kiendev";

    public FileDataHandler(string dataDirPath, string dataFileName, bool encryptData)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.encryptData = encryptData;
    }

    public void Save(T data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonConvert.SerializeObject(data, Formatting.Indented);

            if (encryptData)
                dataToStore = EncryptDecrypt(dataToStore);

            File.WriteAllText(fullPath, dataToStore);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving data to file: {fullPath}\n{e}");
        }
    }

    public T Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        if (!File.Exists(fullPath))
            return null;

        try
        {
            string dataToLoad = File.ReadAllText(fullPath);

            if (encryptData)
                dataToLoad = EncryptDecrypt(dataToLoad);

            return JsonConvert.DeserializeObject<T>(dataToLoad);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error reading data from file: {fullPath}\n{e}");
            return null;
        }
    }

    public bool Exists()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        return File.Exists(fullPath);
    }

    public void Delete()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

    private string EncryptDecrypt(string data)
    {
        if (string.IsNullOrEmpty(data))
            return data;

        StringBuilder result = new StringBuilder(data.Length);

        for (int i = 0; i < data.Length; i++)
        {
            result.Append((char)(data[i] ^ codeWord[i % codeWord.Length]));
        }

        return result.ToString();
    }
}