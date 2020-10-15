using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class Database : MonoBehaviour
{
    public string ContentRequestUrl;

    private DatabaseData _databaseData;
    private Dictionary<int, AssetData> _assetDict;
    private Dictionary<string, Anchor> _anchorDict;

    void Start()
    {
        
    }


    public async void GetData(bool forceReload, Action<DatabaseData> onDone = null) {
        if (_databaseData == null || forceReload) {
            string result = await Database.GetRequest(ContentRequestUrl);
            _databaseData = JsonUtility.FromJson<DatabaseData>(result);
            if (_databaseData != null) {
                _assetDict = CreateAssetDict(_databaseData.assets);
                _anchorDict = CreateAnchorDict(_databaseData.anchors);
            } else {
                Debug.LogWarning("Cannot load Database");
            }
        }
        if (onDone != null) {
            onDone(_databaseData);
        }
    }

    public void GetData(Action<DatabaseData> onDone = null) {
        GetData(false, (data) => {
            if (onDone != null) {
                onDone(data);
            }
        });
    }

    public bool TryGetAssetByAnchorId(string anchorId, out AssetData asset) {
        Anchor anchor;
        if (_anchorDict.TryGetValue(anchorId, out anchor)) {
            if (_assetDict.TryGetValue(anchor.assetId, out asset)) {
                return true;
            }
        }
        asset = null;
        return false;
    }
    
    public bool TryGetAnchorById(string anchorId, out Anchor anchor) {
        if (_anchorDict.TryGetValue(anchorId, out anchor)) {
            return true;
        }
        anchor = null;
        return false;
    }

    private Dictionary<int, AssetData> CreateAssetDict(AssetData[] assets) {

        Dictionary<int, AssetData> assetDict = new Dictionary<int, AssetData>();

        foreach (AssetData asset in assets) {
            assetDict.Add(asset.assetId, asset);
        } 
        return assetDict;
    }

    private Dictionary<string, Anchor> CreateAnchorDict(Anchor[] anchors) {

        Dictionary<string, Anchor> anchorDict = new Dictionary<string, Anchor>();

        foreach (Anchor anchor in anchors) {
            anchorDict.Add(anchor.anchorId, anchor);
        } 
        return anchorDict;
    }

    private static async Task<string> GetRequest(string url, Action<float> onProgress = null, Action<string> onError = null)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.SendWebRequest();            

            if (req.isNetworkError || req.isHttpError)
            {
                if (onError != null) {
                    onError($"{req.error} : {req.downloadHandler.text}");
                }
            }

            while (!req.isDone) {
                await Task.Delay(100);
            }
            if (onProgress != null) {
                onProgress(1.0f);
            }
            return req.downloadHandler.text;
        }
    }

}

[System.Serializable]
public class DatabaseData {
    public AssetData[] assets;
    public Anchor[] anchors;
    public Scene[] scenes;
}

[System.Serializable]
public class AssetData {
    public int assetId;
    public Creator creator;
    public Course course;
    public string name;
    public string link;
    public string thumbnail;
    public string assetType;

    public string type;
    public string power;
    public string color;

}

[System.Serializable]
public class Creator {
    public string name;
    public string studies;
}

[System.Serializable]
public class Course {
    public string name;
    public string term;
}

[System.Serializable]
public class Anchor {
    public string anchorId;
    public int assetId;
    public float scale;
}

[System.Serializable]
public class AnchorPost {
    public Anchor[] anchors;
}

[System.Serializable]
public class Scene {
    public int sceneId;
    public string name;
    public string worldMapLink;
    public string worldMapUUID;
    public Marker marker;
}

[System.Serializable]
public class Marker {
    public string name;
    public string link;
}


