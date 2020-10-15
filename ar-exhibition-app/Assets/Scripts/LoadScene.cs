using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class LoadScene : MonoBehaviour
{

    XRReferenceImage _referenceImage;

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

<<<<<<< HEAD
        _sceneInfo = GameObject.FindObjectOfType<SceneInfo>();

        FindWorldLink(_referenceImage.name);
=======
        Scene scene;
        if(TryGetSceneFromReferenceImage(_referenceImage, out scene)) {
            Debug.Log("Found Name: " + scene.marker.name);
            Debug.Log("Corresponding worldMapLink: " + scene.worldMapLink);
        };
>>>>>>> 405ef7de6b212835dcdd09de04b0a5da8720f8c8
    }

    bool TryGetSceneFromReferenceImage (XRReferenceImage referenceImage, out Scene scene)
    {
        foreach (Scene _scene in _sceneList)
        {
            if (referenceImage.name == _scene.marker.name)
            {
<<<<<<< HEAD
                Debug.Log("Found Name: " + scene.marker.name);
                Debug.Log("Corresponding worldMapLink: " + scene.worldMapLink);
                _sceneInfo.SetScene(scene);
                _sceneInfo.LoadScene(_sceneInfo.GetSceneType());
=======
                scene = _scene;
                return true;
>>>>>>> 405ef7de6b212835dcdd09de04b0a5da8720f8c8
            }
        }
        scene = null;
        return false;
    }
}
