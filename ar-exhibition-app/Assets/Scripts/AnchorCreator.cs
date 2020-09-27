using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;



[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(ARRaycastManager))]
public class AnchorCreator : MonoBehaviour
{

    public GameObject PlacementIndicator;

    public void RemoveAllAnchors()
    {
        foreach (var anchor in m_Anchors)
        {
            m_AnchorManager.RemoveAnchor(anchor);
        }
        m_Anchors.Clear();
    }

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_AnchorManager = GetComponent<ARAnchorManager>();
        m_Anchors = new List<ARAnchor>();
    }

    void Update()
    {
        Vector2 screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        if (m_RaycastManager.Raycast(screenCenter, s_Hits, TrackableType.PlaneEstimated)) {
            Pose hitPose = s_Hits[0].pose;
            PlacementIndicator.transform.position = hitPose.position;
            PlacementIndicator.transform.rotation = hitPose.rotation;
            PlacementIndicator.SetActive(true);
            if (Input.touchCount > 0) {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began) {
                    var anchor = m_AnchorManager.AddAnchor(hitPose);
                    if (anchor == null) {
                        Debug.Log("Error creating anchor");
                    } else {
                        m_Anchors.Add(anchor);
                    }
                }
            }
        } else {
            PlacementIndicator.SetActive(false);
        }

    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    List<ARAnchor> m_Anchors;

    ARRaycastManager m_RaycastManager;

    ARAnchorManager m_AnchorManager;
}

