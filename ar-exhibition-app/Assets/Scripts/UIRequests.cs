using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIRequests : MonoBehaviour
{
    [SerializeField] private TextAsset courseJSON;
    [SerializeField] private TextAsset semesterJSON;

    private ModelLoader modelLoader;

    [SerializeField] private GameObject topPanel;
    [SerializeField] private GameObject assetListPanel;
    [SerializeField] private GameObject assetPanelPrefab;

    [SerializeField] private string url = "cms.domain/api?";

    /*
     * 1. Get List of stored Assets from DB
     * 2. Request specific Asset from DB
     * 3. Save Scene Layout to DB
     * 4. Request specific Scene Layout from DB
     * 5. Request List of stored Scenes from DB
     */
     
    private void Start()
    {
        GetCourseList(courseJSON);
        GetSemesterList(semesterJSON);
    }
    

    public void GetCourseList(TextAsset courseList)
    {
        // Server query to get list with all Course names (e.g. VFX, P1, VN, etc)
        Courses coursesInList = JsonUtility.FromJson<Courses>(courseList.text);
        

        foreach (CourseListEntry course in coursesInList.courses)
        {
            topPanel.transform.GetChild(2).GetComponent<TMP_Dropdown>().options.Add(new TMP_Dropdown.OptionData() { text = course.name });
        }
    }

    public void GetSemesterList(TextAsset semesterList)
    {
        // Server query to get list with all Semester names (e.g. SS19, WS1920, etc)
        Semester semestersInList = JsonUtility.FromJson<Semester>(semesterList.text);


        foreach (SemesterListEntry semester in semestersInList.semesters)
        {
            topPanel.transform.GetChild(3).GetComponent<TMP_Dropdown>().options.Add(new TMP_Dropdown.OptionData() { text = semester.name });
        }
    }

    public void GetAssetList()
    {
        // Server query to get list with assets that comply the given criteria

        string query = BuildServerQuery();
        Debug.Log(query);

        //Send Query to Server
    }



    public void GetAssetListFromServer(TextAsset file)
    {
        // Some Server Calls to get the Asset List
        // For now reads a local .json file
        modelLoader = this.gameObject.transform.GetComponent<ModelLoader>();

        Assets assetsInList = JsonUtility.FromJson<Assets>(file.text);

        PlaceAssetsInUI(assetsInList);
    }

    private string BuildServerQuery()
    {
        string assetName, creatorName, courseName, semesterName;
        string assetString, creatorString, courseString, semesterString;

        string webrequest = url;
        webrequest += "ressource=asset";

        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        Transform buttonParent = clickedButton.transform.parent;

        assetName = buttonParent.Find("AssetNameTextfield").GetComponentInChildren<TMP_InputField>().text;
        creatorName = buttonParent.Find("CreatorNameTextfield").GetComponentInChildren<TMP_InputField>().text;
        TMP_Dropdown courseDropdown = buttonParent.Find("CourseDropdown").GetComponent<TMP_Dropdown>();
        TMP_Dropdown semesterDropdown = buttonParent.Find("SemesterDropdown").GetComponent<TMP_Dropdown>();

        if (assetName == "Asset Name" || assetName == "" || assetName == " ")
            assetString = "";
        else
            assetString = "&name=" + assetName;
        if (creatorName == "Creator Name" || creatorName == "" || creatorName == " ")
            creatorString = "";
        else
            creatorString = "&creator=" + creatorName;
        if (courseDropdown.options.Count == 0 || courseDropdown.value == 0)
            courseString = "";
        else
            courseString = "&course=" + courseDropdown.options[courseDropdown.value].text;
        if (semesterDropdown.options.Count == 0 || semesterDropdown.value == 0)
            semesterString = "";
        else
            semesterString = "&semester=" + semesterDropdown.options[semesterDropdown.value].text;

        webrequest += assetString + creatorString + courseString + semesterString;
        webrequest = webrequest.Replace(' ', '+');

        return webrequest;
    }

    private void PlaceAssetsInUI(Assets assetsInList)
    {
        if (assetListPanel.transform.childCount != 0)
        {
            foreach (Transform child in assetListPanel.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        foreach (AssetListEntry asset in assetsInList.assets)
        {
            GameObject assetPanel = GameObject.Instantiate(assetPanelPrefab, assetListPanel.transform);
            assetPanel.transform.Find("AssetFileNameText").GetComponent<TextMeshProUGUI>().text = asset.fileName;
            assetPanel.transform.Find("AssetCreatorNameText").GetComponent<TextMeshProUGUI>().text = asset.name;
            assetPanel.transform.Find("AssetCourseNameText").GetComponent<TextMeshProUGUI>().text = asset.course;
            assetPanel.transform.Find("AssetSemesterNameText").GetComponent<TextMeshProUGUI>().text = asset.semester;
            if (asset.fileType != "GLB")
                assetPanel.transform.Find("DownloadButton").GetComponent<Button>().onClick.AddListener(delegate { modelLoader.DownloadTexture(asset.link); });
            else
                assetPanel.transform.Find("DownloadButton").GetComponent<Button>().onClick.AddListener(delegate { modelLoader.DownloadFile(asset.link); });
        }
    }
}
