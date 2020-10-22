using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageFetcher : MonoBehaviour
{
    public string Url;
    public GameObject[] ImageHolders;

    public Action OnLoaded;

    [Header("Debug")]
    public bool SkipCache = false;
    public bool LoadOnStart = true;

    private ProgressIndicator _progressIndicator;

    public void Start() {

        _progressIndicator = GetComponentInChildren<ProgressIndicator>();
        _progressIndicator.gameObject.SetActive(false);
        if (LoadOnStart) {
            DownloadFile(Url);
        }
    }


    public void DownloadFile(string url)
    {
        _progressIndicator.gameObject.SetActive(true);

        FileDownloader.DownloadFile(url, SkipCache, (path) => {
            _progressIndicator.gameObject.SetActive(false);
            LoadImage(path);
        }, (progress) => {
            _progressIndicator.progress = progress;
        }, (error) => {
            Debug.LogError(error);
        });
    }

    void LoadImage(string path)
    {
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(fileData);
        
        float widthScaleFactor = tex.width / (float) tex.height; 

        foreach (GameObject imageHolder in ImageHolders)
        {
            imageHolder.transform.localScale = new Vector3(widthScaleFactor, 1.0f, 1.0f);
            imageHolder.GetComponent<Renderer>().material.SetTexture("_MainTex", tex);
            imageHolder.SetActive(true);
        }
        if (OnLoaded != null) {
            OnLoaded.Invoke();
        }
    }
}
