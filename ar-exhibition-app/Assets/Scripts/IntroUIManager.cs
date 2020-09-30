using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class IntroUIManager : MonoBehaviour
{

    private UIDocument _UIDocument;
    private VisualElement _root;
    private Button _startExhibitionButton;
    private Button _createExhibitionButton;


    void Awake()
    {   
        _UIDocument = GetComponent<UIDocument>();
        _root = _UIDocument.rootVisualElement;
        _startExhibitionButton = _root.Q<Button>("startExhibitionButton");
        _createExhibitionButton = _root.Q<Button>("createExhibitionButton");
    }

    void OnEnable() {
        _startExhibitionButton.clicked += OnStartExhibition;
        _createExhibitionButton.clicked += OnCreateExhibition;
    }
    void OnDisable() {
        _startExhibitionButton.clicked -= OnStartExhibition;
        _createExhibitionButton.clicked -= OnCreateExhibition;
    }

    void OnStartExhibition() {

    }

    void OnCreateExhibition() {
        SceneManager.LoadScene("CuratorScene", LoadSceneMode.Single);
    }
}
