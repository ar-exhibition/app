using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Requests : MonoBehaviour
{

    public TextAsset jsonFile;

    private ModelLoader modelLoader;

    [SerializeField] private GameObject assetListPanel;
    [SerializeField] private GameObject assetPanelPrefab;

    /*
     * 1. Get List of stored Assets from DB
     * 2. Request specific Asset from DB
     * 3. Save Scene Layout to DB
     * 4. Request specific Scene Layout from DB
     * 5. Request List of stored Scenes from DB
     */

        private void Start()
        {
             modelLoader = this.gameObject.transform.GetComponent<ModelLoader>();

            Assets assetsInList = JsonUtility.FromJson<Assets>(jsonFile.text);

            foreach (AssetListEntry asset in assetsInList.assets)
            {
                Debug.Log("Found asset: " + asset.fileName + " " + asset.name + " " + asset.fileType + " " + asset.course + " " + asset.semester + " " + asset.link);
                GameObject assetPanel = GameObject.Instantiate(assetPanelPrefab, assetListPanel.transform);
                assetPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = asset.fileName;
                assetPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = asset.name;
                assetPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = asset.course;
                assetPanel.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = asset.semester;
            if (asset.fileType != "GLB")
                assetPanel.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate { modelLoader.DownloadTexture(asset.link); });
            else
                assetPanel.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate { modelLoader.DownloadFile(asset.link); });
        }
        }

    public void GetAssetListFromServer()
    {
        // Some Server Calls to get the Asset List
        // For now reads a local .json file
        Assets assetsInList = JsonUtility.FromJson<Assets>(jsonFile.text);

        foreach (AssetListEntry asset in assetsInList.assets)
        {
            Debug.Log("Found asset: " + asset.fileName + " " + asset.name + " " + asset.fileType + " " + asset.course + " " + asset.semester + " " + asset.link);
            GameObject.Instantiate(assetPanelPrefab, assetListPanel.transform);
        }
    }
}
