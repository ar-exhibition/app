using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectAssetGesture : MonoBehaviour
{
    private int layerMask;
    private AnchorAsset lastSelected;

    void Start() {
        layerMask = LayerMask.GetMask("Asset");
    }

    void Update() {
        for (var i = 0; i < Input.touchCount; ++i) {
            if (Input.GetTouch(i).phase == TouchPhase.Began) {
                Debug.Log("Touch");
                // Construct a ray from the current touch coordinates
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                RayCheck(ray);        
            }
        }
        #if UNITY_EDITOR
        if (Input.GetMouseButtonUp(0)) {
            Debug.Log("Click");
            // Construct a ray from the current touch coordinates
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RayCheck(ray);
        }
        #endif
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
                }
                if (lastSelected != targetAnchor) {
                    targetAnchor.EnterSelection();
                    lastSelected = targetAnchor;
                } else {
                    lastSelected = null;
                }
                
            }
        } 
    }
}
