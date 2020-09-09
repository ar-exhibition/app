﻿using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Siccity.GLTFUtility;

public class ModelLoader : MonoBehaviour
{

    GameObject wrapper;
    string filePath;

    [SerializeField] float modelDimension = 3f;

    // Start is called before the first frame update
    void Start()
    {
        filePath = $"{Application.persistentDataPath}/Files/";
    }

    public void DownloadFile(string url)
    {
        string path = GetFilePath(url);
        if (File.Exists(path))
        {
            Debug.Log("Found file locally, loading...");
            LoadModel(path);
            return;
        }

        StartCoroutine(GetFileRequest(url, (UnityWebRequest req) =>
        {
            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log($"{req.error} : {req.downloadHandler.text}");
            }
            else
            {
                LoadModel(path);
            }
        }));
    }

    string GetFilePath(string url)
    {
        string[] pieces = url.Split('/');
        string fileName = pieces[pieces.Length - 1];

        return $"{filePath}{fileName}";
    }

    void LoadModel(string path)
    {
        CreateWrapper();
        GameObject model = Importer.LoadFromFile(path);
        model.transform.SetParent(wrapper.transform);
        
        Resize(wrapper);
    }

    void CreateWrapper()
    {
        wrapper = new GameObject
        {
            name = "Model"
        };
    }

    void Resize(GameObject model)
    {
        MeshRenderer renderer = model.GetComponentInChildren<MeshRenderer>();

        float max = Mathf.Max(Mathf.Max(renderer.bounds.size.x, renderer.bounds.size.y), renderer.bounds.size.z);
        float scalingFactor = max * modelDimension;
        model.transform.localScale *= scalingFactor;
    }

    IEnumerator GetFileRequest(string url, Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.downloadHandler = new DownloadHandlerFile(GetFilePath(url));
            yield return req.SendWebRequest();
            callback(req);
        }
    }

    // Not needed right now
    void ResetWrapper()
    {
        if (wrapper != null)
        {
            foreach (Transform trans in wrapper.transform)
            {
                Destroy(trans.gameObject);
            }
        }
    }
}
