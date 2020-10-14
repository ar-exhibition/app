using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SelectAssetGesture : MonoBehaviour
{
    private int layerMask;
    private AnchorAsset lastSelected;

    private AnchorAsset dragStartSelected;

    private Vector2 startTouch, swipeDelta;
    private bool isDraging = false, isMultiTouching;

    private UIManager _uiManager;

    ARRaycastManager _raycastManager;
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    void Start() {
        layerMask = LayerMask.GetMask("Asset");
        _raycastManager = FindObjectOfType<ARRaycastManager>();
        _uiManager = FindObjectOfType<UIManager>();
    }

    void Update() {
        CheckTouch();
        #if UNITY_EDITOR
        if (Input.GetMouseButtonUp(0)) {
            Debug.Log("Click");
            // Construct a ray from the current touch coordinates
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RayCheck(ray);
        }
        #endif
    }
 
    void CheckTouch() {    
        if (Input.touchCount != 0) {
            if (Input.touchCount == 1) {
                if (Input.GetTouch(0).phase == TouchPhase.Began) {
                    isDraging = true;
                    startTouch = Input.GetTouch(0).position;

                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    if (Physics.Raycast(ray, out hit, 50f, layerMask)) {
                        AnchorAsset targetAnchor = hit.transform.gameObject.GetComponentInParent<AnchorAsset>();
                        if (targetAnchor != null) {
                            dragStartSelected = targetAnchor;
                        }
                    }
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Moved) {
                    if (!isMultiTouching && isDraging && lastSelected != null) {
                        if (_raycastManager.Raycast(Input.GetTouch(0).position, s_Hits, TrackableType.PlaneEstimated)) {
                            Pose hitPose = s_Hits[0].pose;
                            Vector3 hitPosition = hitPose.position;

                            if (dragStartSelected == lastSelected) {
                                dragStartSelected.transform.position = hitPosition;
                            }
                        }
                    }
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled) {
                    swipeDelta = Input.GetTouch(0).position - startTouch;
                    if (swipeDelta.magnitude > 25) {
                        // swipe
                    } else {
                        if (!isMultiTouching) {
                            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                            RayCheck(ray);
                        }

                    }

                    Reset();
                }
            } else if (Input.touchCount == 2) {
                isMultiTouching = true;
                // Store both touches.
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                // Find the position in the previous frame of each touch.
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // Find the magnitude of the vector (the distance) between the touches in each frame.
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                // Find the difference in the distances between each frame.
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                if (lastSelected != null) {
                    float min = 0.5f;
                    float max = 2.5f;

                    Vector3 newScale = lastSelected.transform.localScale + Vector3.one * -deltaMagnitudeDiff * 0.001f;
                    if (newScale.x > min && newScale.x < max) {
                        lastSelected.transform.localScale = newScale;
                    }
                }

                Vector2 prevDir = (touchZeroPrevPos - touchOnePrevPos).normalized;
                Vector2 latestDir = (touchZero.position - touchOne.position).normalized;

                float angle = Vector2.SignedAngle(prevDir, latestDir);

                if (lastSelected != null) {
                    lastSelected.transform.Rotate(Vector3.up, -angle * 0.5f);
                }

            }
        }
        swipeDelta = Vector2.zero;
    }
 
    void Reset() {
        startTouch = swipeDelta = Vector2.zero;
        dragStartSelected = null;
        isDraging = false;
        isMultiTouching = false;
    }

    void RayCheck(Ray ray) {
        RaycastHit hit;
        // Create a particle if hit
        if (Physics.Raycast(ray, out hit, 50f, layerMask)) {
            Debug.Log("Hit");
            AnchorAsset targetAnchor = hit.transform.gameObject.GetComponentInParent<AnchorAsset>();
            if (targetAnchor != null) {
                Debug.Log("Touched anchor asset");
                if (lastSelected != null) {
                    lastSelected.ExitSelection();
                    _uiManager.ExitSelectionMode();
                }
                if (lastSelected != targetAnchor) {
                    targetAnchor.EnterSelection();
                    _uiManager.EnterSelectionMode(targetAnchor.GetAsset());
                    lastSelected = targetAnchor;
                } else {
                    lastSelected = null;
                }
                
            }
        } 
    }

    public void DeselectCurrentAsset() {
        if (lastSelected != null) {
            lastSelected.ExitSelection();
            _uiManager.ExitSelectionMode();
        }
    }
    
    public void DeleteCurrentAsset() {
        if (lastSelected != null) {
            lastSelected.Delete();
            lastSelected = null;
            Reset();
        }
    }
}
