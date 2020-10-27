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

    public Action OnLoaded;

    [Header("Debug")]
    public bool SkipCache = false;
    public bool LoadOnStart = true;

    private ProgressIndicator _progressIndicator;

    private GameObject model;
    private Action<GameObject, AnimationClip[]> onImportFinished;
    private Action<float> onImportProgress;

    public void Start() {

        _progressIndicator = GetComponentInChildren<ProgressIndicator>();
        _progressIndicator.gameObject.SetActive(false);

        if (LoadOnStart) {
            DownloadFile(Url);
        }
    }

    void OnEnable() {
        onImportFinished += importFinished;
        onImportProgress += importProgress;
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
        _progressIndicator.gameObject.SetActive(true);
        try
        {
            Importer.LoadFromFileAsync(path, new ImportSettings(){useLegacyClips = true}, onImportFinished, onImportProgress);
        }
        catch (System.Exception)
        {
            _progressIndicator.gameObject.SetActive(false);
            Debug.LogError("Cannot load model");
        }
        
    }

    void importProgress(float progress) {
        _progressIndicator.progress = progress;
    }

    void importFinished(GameObject importedModel, AnimationClip[] animClips) {
        _progressIndicator.gameObject.SetActive(false);
        model = importedModel;
        Resize(model, GetMaxBounds(model), 1.0f);
        if (animClips.Length > 0) {
            AddAnimations(model, animClips);
        }
        Vector3 scale = model.transform.localScale;
        model.transform.SetParent(gameObject.transform);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        model.transform.localScale = scale;
        if (OnLoaded != null) {
            OnLoaded.Invoke();
        }
    }

    void AddAnimations(GameObject model, AnimationClip[] clips)
    {
        if (clips.Length > 0)
        {
            Animation animation = model.AddComponent<Animation>();
            clips[0].legacy = true;
            animation.AddClip(clips[0], clips[0].name);
            animation.clip = animation.GetClip(clips[0].name);
        }
    }

    public void StartAnimation()
    {
        if (model.TryGetComponent<Animation>(out Animation animation))
        {
            if (animation.isPlaying) {
                StopAnimation();
            }
            else {
                animation.Play();
            }
        }
    }

    private void StopAnimation()
    {
        if (model.TryGetComponent<Animation>(out Animation animation))
        {
            animation.Stop();
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