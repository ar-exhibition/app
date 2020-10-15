using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

public static class FileDownloader
{

    public static string FilePath = $"{Application.persistentDataPath}/files/";

    public static async void DownloadFile(string url, Action<string> onSuccess, Action<float> onProgress = null, Action<string> onError = null)
    {   
        string result = await FileDownloader.GetFileRequest(url, (progress) => {
            if (onProgress != null) {
                onProgress(progress);
            }
        }, (error) => {
            if (onError != null) {
                onError(error);
            }
        });
        onSuccess(result);
    }

    public static void DownloadFile(string url, bool skipCache, Action<string> onSuccess, Action<float> onProgress = null, Action<string> onError = null)
    {   
        string path = GetFilePath(url);
        if (!skipCache && File.Exists(path))
        {
            Debug.Log("Found file locally, loading...");
            onSuccess(path);
            return;
        }
        FileDownloader.DownloadFile(url, onSuccess, onProgress, onError);
    }

    private static string GetFilePath(string url)
    {
        Uri uri = new Uri(url);
        string fileName = System.IO.Path.GetFileName(uri.LocalPath);
        return $"{FileDownloader.FilePath}{fileName}";
    }


    private static async Task<string> GetFileRequest(string url, Action<float> onProgress = null, Action<string> onError = null)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            int fileSize = 0;
            req.downloadHandler = new DownloadHandlerFile(FileDownloader.GetFilePath(url));
            req.SendWebRequest();            

            if (req.isNetworkError || req.isHttpError)
            {
                if (onError != null) {
                    onError($"{req.error} : {req.downloadHandler.text}");
                }
            }

            while (!req.isDone)
            {
                if (fileSize == 0) {
                    Int32.TryParse(req.GetResponseHeader("Content-Length"), out fileSize);
                }
                if (onProgress != null) {
                    onProgress((fileSize != 0) ? (req.downloadedBytes / (float)fileSize) : req.downloadProgress);
                }
                await Task.Delay(100);
            }
            if (onProgress != null) {
                onProgress(1.0f);
            }
            return FileDownloader.GetFilePath(url);
        }
    }
}
    