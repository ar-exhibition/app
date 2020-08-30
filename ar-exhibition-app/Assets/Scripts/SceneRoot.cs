using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SceneRoot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // set session origion transform to transform of the instantionated object
        ARSessionOrigin origin = FindObjectOfType<ARSessionOrigin>();
        origin.transform.position = transform.position;
        origin.transform.rotation = transform.rotation;

        // disable image and object manager
        ARTrackedImageManager imageManger = origin.GetComponent<ARTrackedImageManager>();
        imageManger.enabled = false;
        ARTrackedObjectManager objectManger = origin.GetComponent<ARTrackedObjectManager>();
        objectManger.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
