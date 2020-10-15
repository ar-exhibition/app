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

        _sceneInfo = GameObject.FindObjectOfType<SceneInfo>();

        FindWorldLink(_referenceImage.name);
    }

    void FindWorldLink (string imageName)
    {
        foreach (Scene scene in _sceneList)
        {
            if (_referenceImage.name == scene.marker.name)
            {
                Debug.Log("Found Name: " + scene.marker.name);
                Debug.Log("Corresponding worldMapLink: " + scene.worldMapLink);
                _sceneInfo.SetScene(scene);
                _sceneInfo.LoadScene(_sceneInfo.GetSceneType());
            }
        }
    }
}
