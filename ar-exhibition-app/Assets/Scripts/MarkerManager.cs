using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.IO;

public class MarkerManager : MonoBehaviour
{

    private Database _database;
    private List<Marker> _markers = new List<Marker>();
    private List<Scene> _scenes = new List<Scene>();

    private ARTrackedImageManager _arTrackedImageManager;
    private RuntimeReferenceImageLibrary runtimeLibrary;
    private MutableRuntimeReferenceImageLibrary mutableLibrary;

    private void Awake()
    {
        _database = GameObject.FindObjectOfType<Database>();
        _arTrackedImageManager = GameObject.FindObjectOfType<ARTrackedImageManager>();

        runtimeLibrary = _arTrackedImageManager.CreateRuntimeLibrary();
        mutableLibrary = runtimeLibrary as MutableRuntimeReferenceImageLibrary;
        _arTrackedImageManager.referenceLibrary = mutableLibrary;
        _arTrackedImageManager.enabled = true;

        GetMarkerData();
    }

    private void GetMarkerData()
    {
        _database.GetData((data) =>
        {
            foreach (Scene scene in data.scenes)
            {
                _scenes.Add(scene);
                Marker newMarker = scene.marker;
                bool addMarker = true;
                foreach (Marker marker in _markers)
                {
                    if (marker.name.Equals(newMarker.name))
                    {
                        addMarker = false;
                    }
                }
                if (addMarker)
                    _markers.Add(newMarker);
            }
            DownloadMarker();
        });  
    }

    private void DownloadMarker()
    {
        foreach (Marker marker in _markers)
        {
            FileDownloader.DownloadFile(marker.link, false, (path) =>
            {
                Texture2D tex;
                if(TryLoadImage(path, out tex)) {
                    AddMarkerToLibrary(tex, marker.name);
                }
            });
        }
    }

    private void AddMarkerToLibrary(Texture2D texture, string name)
    {
        Debug.Log("Called");
        mutableLibrary.ScheduleAddImageJob(texture, name, 0.1f);
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

    bool TryLoadImage(string path, out Texture2D texture)
    {
        byte[] fileData = File.ReadAllBytes(path);
        if (fileData.Length > 0) {
            texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            return true;
        }
        texture = null;
        return false;
    }

    public List<Scene> GetSceneList()
    {
        return _scenes;
    }
}
