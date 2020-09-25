using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBillboard : MonoBehaviour
{
    private Camera _camera;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
    }

     private void LateUpdate()
     {
         transform.LookAt(_camera.transform.position, -Vector3.up);
     }
}
