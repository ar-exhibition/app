using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{

    public VisualTreeAsset AssetItem;

    public GameEvent AddButtonClicked;
    public GameEvent CheckButtonClicked;
    public GameEvent CancelButtonClicked;
    public GameEvent DeleteButtonClicked;
    public GameEvent AssetSelected;
    public GameEvent SaveSceneButtonClicked;
    public GameEvent SelectionCheckButtonClicked;
    public GameEvent MenuOpened;
    public GameEvent MenuClosed;

    private Database _database;

    private UIDocument _UIDocument;

    private VisualElement _root;
    private Button _addButton;
    private Button _checkButton;
    private Button _cancelButton;
    private Button _deleteButton;
    private Button _selectCheckButton;
    private VisualElement _sideMenuContainer;
    private ListView _assetListView;
    private ScrollView _assetScrollView;
    private VisualElement _selectedAssetContainer;
    private VisualElement _sceneBar;
    private Button _saveSceneButton;
    private Button _cancelSceneButton;
    private VisualElement _loadingOverlay;
    private Label _loadingLabel;
    private Label _sceneName;

    private SceneInfo _sceneInfo;

    private AssetData[] _assets;
    [HideInInspector]
    public AssetData SelectedAsset;

    private bool isMouseDown = false;
    private bool dragging = false;
    private float dragStart;
    private float scrollPosition;

    // Start is called before the first frame update
    void Awake()
    {   
        _database = GameObject.FindObjectOfType<Database>();
        _UIDocument = GetComponent<UIDocument>();
        _root = _UIDocument.rootVisualElement;
        _addButton = _root.Q<Button>("addButton");
        _checkButton = _root.Q<Button>("checkButton");
        _cancelButton = _root.Q<Button>("cancelButton");
        _deleteButton = _root.Q<Button>("deleteButton");
        _selectCheckButton = _root.Q<Button>("selectCheckButton");
        _sideMenuContainer = _root.Q<VisualElement>("sideMenuContainer");
        _assetListView = _root.Q<ListView>("assetList");
        _assetScrollView = _assetListView.Q<ScrollView>(null, "unity-scroll-view");
        _selectedAssetContainer = _root.Q<VisualElement>("selectedAssetContainer");
        _sceneBar = _root.Q<VisualElement>("sceneBar");
        _saveSceneButton = _sceneBar.Q<Button>("saveSceneButton");
        _cancelSceneButton = _sceneBar.Q<Button>("cancelSceneButton");
        _loadingOverlay = _root.Q<VisualElement>("loadingOverlay");
        _loadingLabel = _loadingOverlay.Q<Label>("loadingLabel");

        _sceneName = _root.Q<Label>("sceneName");

        _assetScrollView.RegisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
        _assetScrollView.RegisterCallback<MouseUpEvent>(OnMouseUp, TrickleDown.TrickleDown);
        _assetScrollView.RegisterCallback<MouseMoveEvent>(OnMouseMove, TrickleDown.TrickleDown);
        _assetScrollView.scrollDecelerationRate = 0;
        _assetScrollView.showVertical = false;
        _assetScrollView.verticalScroller.style.display = DisplayStyle.None;

        _sceneInfo = FindObjectOfType<SceneInfo>();
        _sceneName.text = _sceneInfo.scene.name;

        _database.GetData((data) => {
           _assets = Array.FindAll<AssetData>(data.assets, (e) => e.assetType != "light");
           AssetListSetup();
        });
    }

    void OnEnable() {
        _addButton.clicked += OnAddButtonClicked;
        _checkButton.clicked += OnCheckButtonClicked;
        _cancelButton.clicked += OnCancelButtonClicked;
        _deleteButton.clicked += OnDeleteButtonClicked;
        _saveSceneButton.clicked += OnSaveSceneButtonClicked;
        _cancelSceneButton.clicked += OnCancelSceneButtonClicked;
        _selectCheckButton.clicked += OnSelectionCheckButtonClicked;
    }
    void OnDisable() {
        _addButton.clicked -= OnAddButtonClicked;
        _checkButton.clicked -= OnCheckButtonClicked;
        _cancelButton.clicked -= OnCancelButtonClicked;
        _deleteButton.clicked -= OnDeleteButtonClicked;
        _saveSceneButton.clicked -= OnSaveSceneButtonClicked;
        _cancelSceneButton.clicked -= OnCancelSceneButtonClicked;
        _selectCheckButton.clicked -= OnSelectionCheckButtonClicked;
    }

    void OnAddButtonClicked() {
        if (AddButtonClicked != null) {
            AddButtonClicked.Raise();
        }
    }
    void OnCheckButtonClicked() {
        if (CheckButtonClicked != null) {
            CheckButtonClicked.Raise();
        }
    }
    async void OnSaveSceneButtonClicked() {
        if (SaveSceneButtonClicked != null) {
            SaveSceneButtonClicked.Raise();
        }
        _loadingOverlay.style.display = DisplayStyle.Flex;
        _loadingLabel.text = "Saving Scene...";
        ARWorldMapController worldMap = FindObjectOfType<ARWorldMapController>();
        await worldMap.Save();
        _database.GetData(true, (data) => {
            // update sceneinfo
            _database.TryGetSceneById(_sceneInfo.scene.sceneId, out _sceneInfo.scene);
        });
        _loadingOverlay.style.display = DisplayStyle.None;
        SceneManager.LoadScene("IntroScene", LoadSceneMode.Single);
    }

    void OnCancelSceneButtonClicked() {
        SceneManager.LoadScene("IntroScene", LoadSceneMode.Single);
    }

    void OnCancelButtonClicked() {
        if (CancelButtonClicked != null) {
            CancelButtonClicked.Raise();
        }
    }
    
    void OnSelectionCheckButtonClicked() {
        if (SelectionCheckButtonClicked != null) {
            SelectionCheckButtonClicked.Raise();
        }
    }
    
    void OnDeleteButtonClicked() {
        if (DeleteButtonClicked != null) {
            DeleteButtonClicked.Raise();
        }
    }

    public AnimationCurve easingInOutCurve = new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(1, 1)
    );

    public void ToggleMenu() {
        if (_sideMenuContainer.resolvedStyle.right < 0f) {
            SlideInMenu();
        } else {
            SlideOutMenu();
            if (MenuClosed != null) {
                MenuClosed.Raise();
            }
        }
    }

    public void SlideInMenu() {
        StartCoroutine(SlideMenuAnimation(0f, 0.4f));
        _root.AddToClassList("menuActive");
        if (MenuOpened != null) {
            MenuOpened.Raise();
        }
    }
    public void SlideOutMenu() {
        _root.RemoveFromClassList("menuActive");
        StartCoroutine(SlideMenuAnimation(-550f, 0.4f));
    }

    public void EnterPlacementMode() {
        SlideOutMenu();
        _addButton.style.display = DisplayStyle.None;
        _checkButton.style.display = DisplayStyle.Flex;
        _cancelButton.style.display = DisplayStyle.Flex;
        _root.AddToClassList("placementActive");
        if (SelectedAsset != null) {
            _selectedAssetContainer.style.display = DisplayStyle.Flex;
            _selectedAssetContainer.Q<Label>("selectedCreator").text = SelectedAsset.creator.name;
            _selectedAssetContainer.Q<Label>("selectedName").text = SelectedAsset.name;
        }
    }
    public void ExitPlacementMode() {
        // SlideOutMenu();
        _addButton.style.display = DisplayStyle.Flex;
        _checkButton.style.display = DisplayStyle.None;
        _selectedAssetContainer.style.display = DisplayStyle.None;
        _cancelButton.style.display = DisplayStyle.None;
        _root.RemoveFromClassList("placementActive");
    }

    public void EnterSelectionMode(AssetData asset) {
        _addButton.style.display = DisplayStyle.None;
        _deleteButton.style.display = DisplayStyle.Flex;
        _selectCheckButton.style.display = DisplayStyle.Flex;
        _root.AddToClassList("placementActive");
        if (asset != null) {
            _selectedAssetContainer.style.display = DisplayStyle.Flex;
            _selectedAssetContainer.Q<Label>("selectedCreator").text = asset.creator.name;
            _selectedAssetContainer.Q<Label>("selectedName").text = asset.name;
        }
    }

    public void ExitSelectionMode() {
        _addButton.style.display = DisplayStyle.Flex;
        _deleteButton.style.display = DisplayStyle.None;
        _selectCheckButton.style.display = DisplayStyle.None;
        _selectedAssetContainer.style.display = DisplayStyle.None;
        _root.RemoveFromClassList("placementActive");
    }

    private void AssetListSetup() {

        if (_assetListView.makeItem == null)
            _assetListView.makeItem = MakeAssetItem;
        if (_assetListView.bindItem == null)
            _assetListView.bindItem = BindAssetItem;

        _assetListView.itemsSource = _assets;
        _assetListView.Refresh();
    }

    private VisualElement MakeAssetItem() {
        var element = AssetItem.CloneTree();
        return element;
    }

    private void BindAssetItem(VisualElement element, int index) {
        element.Q<Label>("assetCreator").text = _assets[index].creator.name;
        element.Q<Label>("assetName").text = _assets[index].name;
        element.Q<Label>("assetSubline").text = $"{_assets[index].creator.studies} | {_assets[index].course.name}";
        element.Q<Label>("assetType").text = _assets[index].assetType;
        VisualElement thumbnailElement = element.Q<VisualElement>("assetThumbnail");
        AddThumbnailToElement(_assets[index].thumbnail, thumbnailElement);
        element.Q<Button>("assetItemContainer").clicked += () => {
            if (!dragging) {
                SelectedAsset = _assets[index];
                EnterPlacementMode();
                if (AssetSelected != null) {
                    AssetSelected.Raise();
                }
            }
        };

        element.userData = _assets[index];
    }

    public void AddThumbnailToElement(string link, VisualElement element) {
        FileDownloader.DownloadFile(link, false, (path) =>
        {
            Texture2D tex;
            if(TryLoadImage(path, out tex)) {
                element.style.backgroundImage = tex;
            }
        });
    }

    bool TryLoadImage(string path, out Texture2D texture)
    {
        byte[] fileData = File.ReadAllBytes(path);
        if (fileData.Length > 0) {
            texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            return true;
        }
        texture = null;
        return false;
    }

    private IEnumerator SlideMenuAnimation(float target, float duration) {
        float journey = 0f;
        float origin = _sideMenuContainer.resolvedStyle.right;
        while (journey <= duration) {
            journey = journey + Time.deltaTime;
            
            float percent = Mathf.Clamp01(journey / duration);
            float curvePercent = easingInOutCurve.Evaluate(percent);

            _sideMenuContainer.style.right = Mathf.Lerp(origin, target, curvePercent);
            yield return null;
        }
    }

    private void OnMouseMove(MouseMoveEvent evt)
    {
        if (isMouseDown && !dragging && Mathf.Abs(dragStart - evt.localMousePosition.y - _assetScrollView.scrollOffset.y) > 4)
        {
            dragging = true;
        }
        if (dragging)
        {
            _assetScrollView.scrollOffset = new Vector2(0,dragStart - evt.localMousePosition.y);
        }
    }
    
    private void OnMouseUp(MouseUpEvent evt)
    {
        isMouseDown = false;
        StartCoroutine(StopDragging());
    }
    
    private void OnMouseDown(MouseDownEvent evt)
    {
        isMouseDown = true;
        dragStart = _assetScrollView.scrollOffset.y + evt.localMousePosition.y;
    }

    IEnumerator StopDragging() {
        yield return new WaitForEndOfFrame();
        dragging = false;
    }


}
