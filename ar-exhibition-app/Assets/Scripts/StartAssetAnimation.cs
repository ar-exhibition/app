using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class StartAssetAnimation : MonoBehaviour
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
        CheckTouch();
#if UNITY_EDITOR
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Click");
            // Construct a ray from the current touch coordinates
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RayCheck(ray);
        }
#endif
    }

    void CheckTouch()
    {
        if (Input.touchCount != 0)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            if (Physics.Raycast(ray, out hit, 50f, layerMask))
            {
                Animation animation = hit.transform.gameObject.GetComponentInParent<Animation>();
                if (animation != null)
                {
                    animation.Play();
                }
            }
                
        }
    }

    void RayCheck(Ray ray)
    {
        RaycastHit hit;
        // Create a particle if hit
        if (Physics.Raycast(ray, out hit, 50f, layerMask))
        {
            Debug.Log("Hit");
            Animation animation = hit.transform.gameObject.GetComponentInParent<Animation>();
            if (animation != null)
            {
                animation.Play();
            }
        }
    }
}
