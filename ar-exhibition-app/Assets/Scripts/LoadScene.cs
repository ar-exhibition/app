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

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Found Marker");
        _referenceImage = GetComponent<ARTrackedImage>().referenceImage;
        _markerManager = GameObject.FindObjectOfType<MarkerManager>();
        _sceneList = _markerManager.GetSceneList();

        Scene scene;
        if(TryGetSceneFromReferenceImage(_referenceImage, out scene)) {
            Debug.Log("Found Name: " + scene.marker.name);
            Debug.Log("Corresponding worldMapLink: " + scene.worldMapLink);
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
