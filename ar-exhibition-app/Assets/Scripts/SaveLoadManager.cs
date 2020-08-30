using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveLoadSystem {
    
    public static readonly string SAVE_FOLDER = Application.persistentDataPath + "/data/";

    public static void Init() {
        if (!Directory.Exists(SAVE_FOLDER)) {
            Directory.CreateDirectory(SAVE_FOLDER);
        }
        Debug.Log(SAVE_FOLDER);
    }

    public static void Save(ExhibitionData data) {
        string jsonString = JsonUtility.ToJson(data);
        File.WriteAllText(SAVE_FOLDER + "data.json", jsonString);
    }

    public static ExhibitionData Load() {
        if (File.Exists(SAVE_FOLDER + "data.json")) {
            string jsonString = File.ReadAllText(SAVE_FOLDER + "data.json");
            return JsonUtility.FromJson<ExhibitionData>(jsonString);
        } else {
            return null;
        }
    }

}

[System.Serializable]
public class ExhibitionData {
    public List<Asset> assets;
}

[System.Serializable]
public class Asset {

    public string modelName;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

}