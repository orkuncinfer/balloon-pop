using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    // Save directory for persistent data
    private static readonly string SaveDirectory = Path.Combine(Application.persistentDataPath, "Saves");

    /// <summary>
    /// Saves an object to a file with the specified key.
    /// Only serializes public fields and fields marked with [SerializeField].
    /// </summary>
    /// <param name="key">The key to identify the saved data.</param>
    /// <param name="data">The object to save.</param>
    public static void Save(string key, object data)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Key cannot be null or empty.");

        if (data == null)
            throw new ArgumentNullException(nameof(data), "Data to save cannot be null.");

        // Ensure the save directory exists
        if (!Directory.Exists(SaveDirectory))
            Directory.CreateDirectory(SaveDirectory);

        // Serialize the object to JSON
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        string filePath = GetFilePath(key, data.GetType());

        // Write the JSON to the file
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// Loads data from a file with the specified key into the provided object.
    /// </summary>
    /// <param name="key">The key to identify the saved data.</param>
    /// <param name="data">The object to load data into.</param>
    public static void LoadInto(string key, object data)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Key cannot be null or empty.");

        if (data == null)
            throw new ArgumentNullException(nameof(data), "Target object cannot be null.");

        string filePath = GetFilePath(key, data.GetType());

        // Check if the file exists
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"No save data found for key: {key} and type: {data.GetType()}");
            return;
        }

        // Read the JSON from the file
        string json = File.ReadAllText(filePath);

        // Deserialize the JSON into the provided object
        JsonUtility.FromJsonOverwrite(json, data);
    }

    /// <summary>
    /// Generates a file path based on the key and object type.
    /// </summary>
    /// <param name="key">The key to identify the saved data.</param>
    /// <param name="type">The type of the object being saved.</param>
    /// <returns>The full file path.</returns>
    private static string GetFilePath(string key, Type type)
    {
        // Include the type name in the file name to allow multiple objects with the same key but different types
        string fileName = $"{key}_{type.Name}.json";
        return Path.Combine(SaveDirectory, fileName);
    }
}