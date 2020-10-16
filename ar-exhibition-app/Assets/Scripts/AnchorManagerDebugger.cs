using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class AnchorManagerDebugger : MonoBehaviour
{
    ARAnchorManager _anchorManager;
    // Start is called before the first frame update
    void Start()
    {
        _anchorManager = GetComponent<ARAnchorManager>();
        _anchorManager.anchorsChanged += (args) => {
            List<ARAnchor> added = args.added;
            foreach (ARAnchor anchor in added)
            {
                Debug.Log("Added anchor: " + anchor.trackableId.ToString());
            }
        };
    }

    public void LogTrackables() {
        Debug.Log("Found " + _anchorManager.trackables.count + " trackables");
        foreach (ARAnchor anchor in _anchorManager.trackables)
        {
            Debug.Log("Found anchor: " + anchor.trackableId.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
