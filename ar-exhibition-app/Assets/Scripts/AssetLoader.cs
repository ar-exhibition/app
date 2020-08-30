using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetLoader : MonoBehaviour
{

    public GameObject prefab;
    private ExhibitionData data;

    // Start is called before the first frame update
    void Start()
    {
        SaveLoadSystem.Init();
    }

    public void LoadData() {
        Reset();
        SaveLoadSystem.Init();
        data = SaveLoadSystem.Load();
        Debug.Log(data);
        if (data != null) {
            foreach (Asset asset in data.assets)
            {
                Instantiate(prefab, asset.position, Quaternion.Euler(asset.rotation));
            }
        }
    }

    public void SaveData() {

        List<Asset> assets = new List<Asset>();

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Asset");
        foreach (GameObject gameObject in gameObjects)
        {
            Asset asset = new Asset();
            asset.modelName = "cube";
            asset.position = gameObject.transform.position;
            asset.rotation = gameObject.transform.rotation.eulerAngles;
            asset.scale = gameObject.transform.localScale;

            assets.Add(asset);
        }

        data = new ExhibitionData();
        data.assets = assets;

        Debug.Log(data);

        SaveLoadSystem.Save(data);

    }

    public void Reset() {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Asset");
        foreach (var gameObject in gameObjects)
        {
            Destroy(gameObject);
        }
    }
}
