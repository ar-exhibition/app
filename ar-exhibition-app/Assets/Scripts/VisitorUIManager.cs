using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VisitorUIManager : MonoBehaviour
{

    private UIDocument _UIDocument;
    private VisualElement _root;
    private VisualElement _loadingOverlay;
    private Button _backButton;

    private ARWorldMapController _arWorldMapController;

    private SceneInfo _sceneInfo;

    // Start is called before the first frame update
    void Awake()
    {
        _UIDocument = GetComponent<UIDocument>();
        _root = _UIDocument.rootVisualElement;
        _loadingOverlay = _root.Q<VisualElement>("visitorRoot");
        _backButton = _root.Q<Button>("backButtonVisitor");
    }

    async void Start() {
        _arWorldMapController = FindObjectOfType<ARWorldMapController>();

        _sceneInfo = FindObjectOfType<SceneInfo>();
        if (_sceneInfo != null) {
            if (_sceneInfo.scene.worldMapLink != null && _sceneInfo.scene.worldMapLink != "") {
                _loadingOverlay.style.display = DisplayStyle.Flex;
                _arWorldMapController.ResetSession();
                FileDownloader.DownloadFile(_sceneInfo.scene.worldMapLink, false, async (path) => {
                    await _arWorldMapController.Load(path);
                    _loadingOverlay.style.display = DisplayStyle.None;
                });
            }
        }
        
    }

    void OnEnable() {
        _backButton.clicked += () => {
            _sceneInfo.GotToStart();
        };
    }

    void OnDisable() {
        _backButton.clicked -= () => {
            _sceneInfo.GotToStart();
        };
    }
}
