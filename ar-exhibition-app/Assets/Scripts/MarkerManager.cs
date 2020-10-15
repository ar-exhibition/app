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

        _database.GetData((data) =>
        {
            foreach (Scene scene in data.scenes)
            {
                _scenes.Add(scene);
                Marker newMarker = scene.marker;
                _markers.Add(newMarker);
                
                FileDownloader.DownloadFile(newMarker.link, false, (url) =>
                {
                    StartCoroutine(AddImage(url, newMarker.name));
                });
            }
            _arTrackedImageManager.referenceLibrary = mutableLibrary;
            _arTrackedImageManager.enabled = true;
            Debug.Log(_arTrackedImageManager.referenceLibrary.count);
            Debug.Log(_arTrackedImageManager.referenceLibrary == mutableLibrary);
            Debug.Log(_arTrackedImageManager.descriptor.supportsMutableLibrary);
        });
    }

    private IEnumerator AddImage(string path, string name)
    {
        FileInfo filePath = new FileInfo(path);
        while (isFileLocked(filePath))
        {
            yield return null;
        }
        Texture2D texture = LoadImage(path);
        mutableLibrary.ScheduleAddImageJob(texture, name, 1f);
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

    Texture2D LoadImage(string path)
    {
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(fileData);

        return tex;
    }

    public List<Scene> GetSceneList()
    {
        return _scenes;
    }
}
