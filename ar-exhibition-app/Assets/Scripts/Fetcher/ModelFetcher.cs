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

    private Animation anim;
    private AnimationClip[] animationClips;

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
        GameObject model = Importer.LoadFromFile(path, new ImportSettings(), out AnimationClip[] animClips);
        animationClips = animClips;
        animationClips[0].legacy = true;
        Resize(model, GetMaxBounds(model), 1.0f);
        AddAnimations(model, animClips);
        Vector3 scale = model.transform.localScale;
        model.transform.SetParent(gameObject.transform);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        model.transform.localScale = scale;
    }

    void AddAnimations(GameObject model, AnimationClip[] clips)
    {
        if (clips.Length > 0)
        {
            anim = model.AddComponent<Animation>();
        }
    }

    public void StartAnimation()
    {
        if (anim != null)
        {
            if (anim.isPlaying)
                StopAnimation();
            else
            {
                anim.AddClip(animationClips[0], animationClips[0].name);
                anim.Play(animationClips[0].name);
            }
        }
    }

    private void StopAnimation()
    {
        if (anim != null)
        {
            anim.Stop();
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
