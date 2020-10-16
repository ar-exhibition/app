using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetClickHandler : MonoBehaviour
{

    private int layerMask;

    // Start is called before the first frame update
    void Start()
    {
        layerMask = LayerMask.GetMask("Asset");
    }

    // Update is called once per frame
    void Update()
    {
        //CheckTouch();
        
#if UNITY_EDITOR
        bool up = false;
        if (Input.GetMouseButtonUp(0) && !up)
        {
            up = true;
            Debug.Log("Click");
            // Construct a ray from the current touch coordinates
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RayCheck(ray);
        }
        if (Input.GetMouseButtonDown(0))
            up = false;
#endif

    }

    void RayCheck(Ray ray)
    {
        RaycastHit hit;
        // Create a particle if hit
        if (Physics.Raycast(ray, out hit, 50f, layerMask))
        {
            Debug.Log("Hit");
            AnchorAsset targetAnchor = hit.transform.gameObject.GetComponentInParent<AnchorAsset>();
            if (targetAnchor != null)
            {
                targetAnchor.HandleClick();
            }
        }
    }

    void CheckTouch()
    {
        if (Input.touchCount != 0) {
            if (Input.touchCount == 1)
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    if (Physics.Raycast(ray, out hit, 50f, layerMask))
                    {
                        AnchorAsset targetAnchor = hit.transform.gameObject.GetComponentInParent<AnchorAsset>();
                        if (targetAnchor != null)
                        {
                            targetAnchor.HandleClick();
                        }
                    }
                }
        }
    }
}
