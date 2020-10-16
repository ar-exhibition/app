using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VisitorUIManager : MonoBehaviour
{

    private UIDocument _UIDocument;
    private VisualElement _root;

    private ARWorldMapController _arWorldMapController;

    private SceneInfo _sceneInfo;

    // Start is called before the first frame update
    void Awake()
    {
        _UIDocument = GetComponent<UIDocument>();
        _root = _UIDocument.rootVisualElement;
    }

    async void Start() {
        _arWorldMapController = FindObjectOfType<ARWorldMapController>();
        _root.style.display = DisplayStyle.Flex;

        _sceneInfo = FindObjectOfType<SceneInfo>();
        if (_sceneInfo != null) {
            if (_sceneInfo.scene.worldMapLink != null && _sceneInfo.scene.worldMapLink != "") {
                FileDownloader.DownloadFile(_sceneInfo.scene.worldMapLink, false, async (path) => {
                    await _arWorldMapController.Load(path);
                    _root.style.display = DisplayStyle.None;
                });
            }
        }
        
    }
}
