using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class IntroUIManager : MonoBehaviour
{

    private UIDocument _UIDocument;
    private VisualElement _root;
    private VisualElement _finderImage;
    private VisualElement _foundSceneContainer;
    private Label _foundScene;

    private Button _enterExhibition;
    private Button _editExhibition;

    private SceneInfo _sceneInfo;


    void Awake()
    {   
        _UIDocument = GetComponent<UIDocument>();
        _root = _UIDocument.rootVisualElement;
        _finderImage = _root.Q<VisualElement>("finderImage");
        _foundSceneContainer = _root.Q<VisualElement>("foundSceneContainer");
        _foundScene = _foundSceneContainer.Q<Label>("foundScene");

        _enterExhibition = _root.Q<Button>("enterExhibition");
        _editExhibition = _root.Q<Button>("editExhibition");

        _sceneInfo = FindObjectOfType<SceneInfo>();
    }

    void OnEnable() {
        _enterExhibition.clicked += () => {
            _sceneInfo.EnterExhibition();
        };

        _editExhibition.clicked += () => {
            _sceneInfo.EditExhibition();
        };
    }
    void OnDisable() {
        _enterExhibition.clicked -= () => {
            _sceneInfo.EnterExhibition();
        };

        _editExhibition.clicked -= () => {
            _sceneInfo.EditExhibition();
        };
    }

    public void FoundMarker(Scene scene) {
        _finderImage.style.display = DisplayStyle.None;
        _foundSceneContainer.style.display = DisplayStyle.Flex;
        _foundScene.text = scene.name;
    }

}
