using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{

    public AnchorCreator anchorCreator;

    public void EnablePlacing() {
        anchorCreator.enabled = true;
    }
    

}
