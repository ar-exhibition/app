using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoFetcher : MonoBehaviour
{
    public string Url;

    [Header("Debug")]
    public bool SkipCache = false;
    public bool LoadOnStart = true;

    private ProgressIndicator _progressIndicator;
    private VideoPlayer _videoPlayer;

    public void Start() {

        _progressIndicator = GetComponentInChildren<ProgressIndicator>();
        _progressIndicator.gameObject.SetActive(false);
        _videoPlayer = GetComponentInChildren<VideoPlayer>(true);
        Debug.Log(_videoPlayer);
        if (LoadOnStart) {
            DownloadFile(Url);
        }
    }


    public void DownloadFile(string url)
    {
        _progressIndicator.gameObject.SetActive(true);

        FileDownloader.DownloadFile(url, true, (path) => {
            _progressIndicator.gameObject.SetActive(false);
            LoadVideo(path);
        }, (progress) => {
            _progressIndicator.progress = progress;
        }, (error) => {
            Debug.LogError(error);
        });
    }

    void LoadVideo(string path)
    {
        _videoPlayer.url = path;
        _videoPlayer.gameObject.SetActive(true);
        _videoPlayer.prepareCompleted += (videoPlayer) => {

            float widthScaleFactor = videoPlayer.texture.width / (float) videoPlayer.texture.height; 
            videoPlayer.transform.localScale = new Vector3(widthScaleFactor, 1.0f, 0.01f);
            Resize(videoPlayer.gameObject);
            videoPlayer.Play();
        };
        _videoPlayer.Prepare();

    }

    public void StartVideo()
    {
        if (_videoPlayer.isPrepared)
        {
            if (_videoPlayer.isPlaying)
                PauseVideo();
            else
                _videoPlayer.Play();
        }
    }

    private void PauseVideo()
    {
        _videoPlayer.Pause();
    }

    void Resize(GameObject model, float maxDimension = 0.5f)
    {
        MeshRenderer renderer = model.GetComponentInChildren<MeshRenderer>();
        if (renderer != null) {
            float max = Mathf.Max(Mathf.Max(renderer.bounds.size.x, renderer.bounds.size.y), renderer.bounds.size.z);
            float scalingFactor = max / maxDimension;
            model.transform.parent.localScale /= scalingFactor;    
        }   
    }
}
