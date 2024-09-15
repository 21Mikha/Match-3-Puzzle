using UnityEngine;
public class LevelDataLoader
{
    public Level LoadLevelData(string jsonPath)
    {
        TextAsset jsonTextFile = Resources.Load<TextAsset>(jsonPath);
        if (jsonTextFile == null)
        {
            Debug.LogError("JSON file not found at: " + jsonPath);
            return null;
        }

        return JsonUtility.FromJson<Level>(jsonTextFile.text);
    }
}
