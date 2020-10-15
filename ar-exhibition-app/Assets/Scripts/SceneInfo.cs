using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInfo : MonoBehaviour
{

    private Scene _scene;
    private bool _curator = true;

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

    public void SetCurator(bool state)
    {
        _curator = state;
    }

    public bool GetCurator()
    {
        return _curator;
    }

    public string GetSceneType()
    {
        if (_curator)
            return "CuratorScene";
        else
            return "VisitorScene";
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
