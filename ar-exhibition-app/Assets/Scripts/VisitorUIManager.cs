using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VisitorUIManager : MonoBehaviour
{

    private UIDocument _UIDocument;
    private VisualElement _root;

    private ARWorldMapController _arWorldMapController;

    // Start is called before the first frame update
    void Awake()
    {
        _UIDocument = GetComponent<UIDocument>();
        _root = _UIDocument.rootVisualElement;
    }

    async void Start() {
        _arWorldMapController = FindObjectOfType<ARWorldMapController>();
        _root.style.display = DisplayStyle.Flex;

        await _arWorldMapController.Load();
        _root.style.display = DisplayStyle.None;
    }
}
