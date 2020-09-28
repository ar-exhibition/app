using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Siccity.GLTFUtility;

public class ModelFetcher : MonoBehaviour
{
    
    public string Url;

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
            LoadModel(path);
        }, (progress) => {
            _progressIndicator.progress = progress;
        }, (error) => {
            Debug.LogError(error);
        });
    }

    void LoadModel(string path)
    {
        GameObject model = Importer.LoadFromFile(path);
        Resize(model);
        model.transform.SetParent(gameObject.transform);
        model.transform.localPosition = Vector3.zero;

    }

    void Resize(GameObject model, float maxDimension = 0.5f)
    {
        MeshRenderer renderer = model.GetComponentInChildren<MeshRenderer>();
        if (renderer != null) {
            float max = Mathf.Max(Mathf.Max(renderer.bounds.size.x, renderer.bounds.size.y), renderer.bounds.size.z);
            float scalingFactor = max / maxDimension;
            model.transform.localScale /= scalingFactor;    
        }   
    }


}
