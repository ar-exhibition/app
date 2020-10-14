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
        try
        {
            GameObject model = Importer.LoadFromFile(path, new ImportSettings(), out AnimationClip[] animClips);
            Resize(model, GetMaxBounds(model), 1.0f);
            AddAnimations(model, animClips);
            Vector3 scale = model.transform.localScale;
            model.transform.SetParent(gameObject.transform);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = scale;
        }
        catch
        {
            Debug.Log("Something went wrong");
            try
            {
                StartCoroutine(DeleteFile(path));
            }
            catch
            {
                Debug.Log("Can't delete file");
            }
        }

    }

    public static bool isFileLocked(FileInfo file)
    {
        FileStream stream = null;

        try
        {
            stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }
        catch (IOException)
        {
            return true;
        }
        finally
        {
            if (stream != null)
                stream.Close();
        }

        return false;
    }

    public IEnumerator DeleteFile(string path)
    {
        FileInfo filePath = new FileInfo(path);

        while (isFileLocked(filePath))
        {
            yield return null;
        }
        File.Delete(path);
    }

    void AddAnimations(GameObject model, AnimationClip[] clips)
    {
        if (clips.Length > 0)
        {
            Animation anim = model.AddComponent<Animation>();
            clips[0].legacy = true;
            anim.AddClip(clips[0], clips[0].name);
            anim.Play(clips[0].name);
        }
    }

    Bounds GetMaxBounds(GameObject g)
    {
        var b = new Bounds(g.transform.position, Vector3.zero);
        foreach (Renderer r in g.GetComponentsInChildren<Renderer>())
        {
            b.Encapsulate(r.bounds);
        }
        return b;
    }

    void Resize(GameObject model, Bounds maxBounds, float maxDimension = 0.5f)
    {

        float max = Mathf.Max(Mathf.Max(maxBounds.size.x, maxBounds.size.y), maxBounds.size.z);
        float scalingFactor = max / maxDimension;
        model.transform.localScale /= scalingFactor;
    }


}
