using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInfo : MonoBehaviour
{
    private Scene _scene;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetScene (Scene scene)
    {
        _scene = scene;
    }

    public Scene GetScene()
    {
        return _scene;
    }

    public void EnterExhibition() {
        SceneManager.LoadScene("VisitorScene");
    }
    
    public void EditExhibition() {
        SceneManager.LoadScene("CuratorScene");
    }
}
