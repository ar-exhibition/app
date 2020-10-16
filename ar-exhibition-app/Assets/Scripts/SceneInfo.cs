using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInfo : MonoBehaviour
{
    public Scene scene;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void EnterExhibition() {
        SceneManager.LoadScene("VisitorScene");
    }
    
    public void EditExhibition() {
        SceneManager.LoadScene("CuratorScene");
    }

    public void GotToStart() {
        SceneManager.LoadScene("IntroScene");
    }
}
