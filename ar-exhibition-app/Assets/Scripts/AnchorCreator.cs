using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;



[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(ARRaycastManager))]
public class AnchorCreator : MonoBehaviour
{

    public GameObject PlacementIndicator;
    public GameObject ObjectToPlace;

    private Vector3 _hitPosition;
    private UIManager _uiManager;

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
        PlacementIndicator.SetActive(false);
    }

    void Start() {
        _uiManager = FindObjectOfType<UIManager>();
    }

    void Update()
    {
        Vector2 screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        if (m_RaycastManager.Raycast(screenCenter, s_Hits, TrackableType.PlaneEstimated)) {
            Pose hitPose = s_Hits[0].pose;
            _hitPosition = hitPose.position;
            PlacementIndicator.transform.rotation = hitPose.rotation;
            PlacementIndicator.SetActive(true);
        } else {
            PlacementIndicator.SetActive(false);
        }

        if (PlacementIndicator.activeInHierarchy) {
            PlacementIndicator.transform.position = Vector3.Lerp(PlacementIndicator.transform.position, _hitPosition, Time.deltaTime * 5f);
        }

    }

    public void PlaceAnchor() {
        if (_uiManager.SelectedAsset != null) {
            Vector2 screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
            if (m_RaycastManager.Raycast(screenCenter, s_Hits, TrackableType.PlaneEstimated)) {
                Pose hitPose = s_Hits[0].pose;
                ARAnchor anchor = m_AnchorManager.AddAnchor(hitPose);
                if (anchor == null) {
                     Debug.Log("Error creating anchor");
                } else {
                    GameObject anchorPrefab = GameObject.Instantiate(ObjectToPlace, hitPose.position, hitPose.rotation);
                    anchorPrefab.GetComponent<AnchorAsset>().LoadAsset(_uiManager.SelectedAsset);
                    anchorPrefab.GetComponent<AnchorAsset>().SetAnchor(anchor);
                    m_Anchors.Add(anchor);
                }
                
            }
        }
    }

    void OnDisable() {
        PlacementIndicator.SetActive(false);
    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    List<ARAnchor> m_Anchors;

    ARRaycastManager m_RaycastManager;

    ARAnchorManager m_AnchorManager;
}

