using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PositionDebug : MonoBehaviour
{
    Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<Text>();
        text.text = $"x: {transform.position.x} y: {transform.position.y} z: {transform.position.z}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
