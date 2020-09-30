using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{

    public VisualTreeAsset AssetItem;

    public GameEvent AddButtonClicked;
    public GameEvent CheckButtonClicked;
    public GameEvent AssetSelected;
    public GameEvent SaveSceneButtonClicked;

    private Database _database;

    private UIDocument _UIDocument;

    private VisualElement _root;
    private Button _addButton;
    private Button _checkButton;
    private VisualElement _sideMenuContainer;
    private ListView _assetListView;
    private VisualElement _selectedAssetContainer;
    private VisualElement _sceneBar;
    private Button _saveSceneButton;

    private AssetData[] _assets;
    private AssetData _selectedAsset;

    // Start is called before the first frame update
    void Awake()
    {   
        _database = GameObject.FindObjectOfType<Database>();
        _UIDocument = GetComponent<UIDocument>();
        _root = _UIDocument.rootVisualElement;
        _addButton = _root.Q<Button>("addButton");
        _checkButton = _root.Q<Button>("checkButton");
        _sideMenuContainer = _root.Q<VisualElement>("sideMenuContainer");
        _assetListView = _root.Q<ListView>("assetList");
        _selectedAssetContainer = _root.Q<VisualElement>("selectedAssetContainer");
        _sceneBar = _root.Q<VisualElement>("sceneBar");
        _saveSceneButton = _sceneBar.Q<Button>("saveSceneButton");

        _database.GetData((data) => {
           _assets = Array.FindAll<AssetData>(data.assets, (e) => e.assetType != "light");
           AssetListSetup();
        });
    }

    void OnEnable() {
        _addButton.clicked += OnAddButtonClicked;
        _checkButton.clicked += OnCheckButtonClicked;
        _saveSceneButton.clicked += OnSaveSceneButtonClicked;
    }
    void OnDisable() {
        _addButton.clicked -= OnAddButtonClicked;
        _checkButton.clicked -= OnCheckButtonClicked;
        _saveSceneButton.clicked -= OnSaveSceneButtonClicked;
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
    void OnSaveSceneButtonClicked() {
        if (SaveSceneButtonClicked != null) {
            SaveSceneButtonClicked.Raise();
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
        }
    }

    public void SlideInMenu() {
        StartCoroutine(SlideMenuAnimation(0f, 0.4f));
        _root.AddToClassList("menuActive");
    }
    public void SlideOutMenu() {
        _root.RemoveFromClassList("menuActive");
        StartCoroutine(SlideMenuAnimation(-550f, 0.4f));
    }

    public void EnterPlacementMode() {
        SlideOutMenu();
        _addButton.style.display = DisplayStyle.None;
        _checkButton.style.display = DisplayStyle.Flex;
        _sceneBar.style.display = DisplayStyle.None;
        if (_selectedAsset != null) {
            _selectedAssetContainer.style.display = DisplayStyle.Flex;
            _selectedAssetContainer.Q<Label>("selectedCreator").text = _selectedAsset.creator.name;
            _selectedAssetContainer.Q<Label>("selectedName").text = _selectedAsset.name;
        }
    }
    public void ExitPlacementMode() {
        // SlideOutMenu();
        _addButton.style.display = DisplayStyle.Flex;
        _checkButton.style.display = DisplayStyle.None;
        _selectedAssetContainer.style.display = DisplayStyle.None;
        _sceneBar.style.display = DisplayStyle.Flex;
    }

    private void AssetListSetup() {

        if (_assetListView.makeItem == null)
            _assetListView.makeItem = MakeAssetItem;
        if (_assetListView.bindItem == null)
            _assetListView.bindItem = BindAssetItem;
        
        _assetListView.onSelectionChange += (e) => {
            object[] arr = e.ToArray<object>();
            if (arr.Length > 0) {
                AssetData asset = arr[0] as AssetData;
                _selectedAsset = asset;
                EnterPlacementMode();
                if (AssetSelected != null) {
                    AssetSelected.Raise();
                }
            }
        };

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

        element.userData = _assets[index];
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


}
