using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class LoadScene : MonoBehaviour
{

    XRReferenceImage _referenceImage;
    ARTrackedImageManager _imageManager;

    MarkerManager _markerManager;
    List<Scene> _sceneList;

    SceneInfo _sceneInfo;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Found Marker");
        _referenceImage = GetComponent<ARTrackedImage>().referenceImage;
        _markerManager = GameObject.FindObjectOfType<MarkerManager>();
        _sceneList = _markerManager.GetSceneList();

        _sceneInfo = GameObject.FindObjectOfType<SceneInfo>();

        Scene scene;
        if(TryGetSceneFromReferenceImage(_referenceImage, out scene)) {
            Debug.Log("Found Name: " + scene.marker.name);
            Debug.Log("Corresponding worldMapLink: " + scene.worldMapLink);
            if (scene.worldMapLink != "")
            {
                _imageManager.enabled = false;
                _sceneInfo.SetScene(scene);
                _sceneInfo.LoadScene(_sceneInfo.GetSceneType());
            }
            else
            {
                Debug.Log("No WorldMapLink found");
            }
        };
    }

    bool TryGetSceneFromReferenceImage (XRReferenceImage referenceImage, out Scene scene)
    {
        foreach (Scene _scene in _sceneList)
        {
            if (referenceImage.name == _scene.marker.name)
            {
                scene = _scene;
                return true;
            }
        }
        scene = null;
        return false;
    }
}
