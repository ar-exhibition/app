using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{

    public GameEvent AddButtonClicked;

    private UIDocument _UIDocument;

    private VisualElement _root;
    private Button _addButton;

    // Start is called before the first frame update
    void Awake()
    {   
        _UIDocument = GetComponent<UIDocument>();
        _root = _UIDocument.rootVisualElement;
        _addButton = _root.Q<Button>("addButton");
    }

    void OnEnable() {
        _addButton.clicked += OnAddButtonClicked;
    }
    void OnDisable() {
        _addButton.clicked -= OnAddButtonClicked;
    }

    void OnAddButtonClicked() {
        if (AddButtonClicked != null) {
            AddButtonClicked.Raise();
        }
    }
}
