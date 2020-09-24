using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
     /*
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
    */

    public void GetCourseList()
    {
        // Server query to get list with all Course names (e.g. VFX, P1, VN, etc)
    }

    public void GetSemesterList()
    {
        // Server query to get list with all Semester names (e.g. SS19, WS1920, etc)
    }

    public void GetAssetList()
    {
        string assetName, creatorName, courseName, semesterName;

        // Server query to get list with assets that comply the given criteria
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        Transform buttonParent = clickedButton.transform.parent;

        assetName = buttonParent.GetChild(0).GetComponentInChildren<TMP_InputField>().text;
        Debug.Log(assetName == " ");
        if (assetName == "Asset Name" || assetName == "" || assetName == " ")
            assetName = "*";
        creatorName = buttonParent.GetChild(1).GetComponentInChildren<TMP_InputField>().text;
        if (creatorName == "Creator Name" || creatorName == "" || creatorName == " ")
            creatorName = "*";
        TMP_Dropdown courseDropdown = buttonParent.GetChild(2).GetComponent<TMP_Dropdown>();
        if (courseDropdown.options.Count == 0)
            courseName = "*";
        else
            courseName = courseDropdown.options[courseDropdown.value].ToString();
        TMP_Dropdown semesterDropdown = buttonParent.GetChild(3).GetComponent<TMP_Dropdown>();
        if (semesterDropdown.options.Count == 0)
            semesterName = "*";
        else
            semesterName = semesterDropdown.options[semesterDropdown.value].ToString();

        Debug.Log(assetName + "  " + creatorName + "   " + courseName + "   " + semesterName);
    }

    public void GetAssetListFromServer(TextAsset file)
    {
        // Some Server Calls to get the Asset List
        // For now reads a local .json file
        modelLoader = this.gameObject.transform.GetComponent<ModelLoader>();

        Assets assetsInList = JsonUtility.FromJson<Assets>(jsonFile.text);

        if (assetListPanel.transform.childCount != 0)
        {
            foreach (Transform child in assetListPanel.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

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
}
