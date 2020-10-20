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
    private Scene[] _scenes;
    private ARSession _arSession;
    private ARTrackedImageManager _arTrackedImageManager;
    private RuntimeReferenceImageLibrary runtimeLibrary;
    private MutableRuntimeReferenceImageLibrary mutableLibrary;

    private SceneInfo _sceneInfo;
    private IntroUIManager _introUI;

    private Scene _lastFoundScene;

    private void Awake()
    {
        _arSession = FindObjectOfType<ARSession>();
        _database = GameObject.FindObjectOfType<Database>();
        _arTrackedImageManager = GameObject.FindObjectOfType<ARTrackedImageManager>();

        _sceneInfo = FindObjectOfType<SceneInfo>();
        _introUI = FindObjectOfType<IntroUIManager>();

        runtimeLibrary = _arTrackedImageManager.CreateRuntimeLibrary();
        mutableLibrary = runtimeLibrary as MutableRuntimeReferenceImageLibrary;
        _arTrackedImageManager.referenceLibrary = mutableLibrary;
        _arTrackedImageManager.enabled = true;

        GetMarkerData();
    }

    void Start() {
        _arSession.Reset();
        _lastFoundScene = null;
    }

    private void GetMarkerData() {
        _database.GetData((data) => {
            _scenes = data.scenes;
            foreach (Scene scene in _scenes) {
                DownloadMarker(scene.marker);
            }
        });  
    }

    private void DownloadMarker(Marker marker) {
        FileDownloader.DownloadFile(marker.link, false, (path) => {
            Texture2D tex;
            if(TryLoadImage(path, out tex)) {
                AddMarkerToLibrary(tex, marker.name);
            }
        });
    }

    private void AddMarkerToLibrary(Texture2D texture, string name) {
        mutableLibrary.ScheduleAddImageJob(texture, name, 0.1f);
    }

    bool TryLoadImage(string path, out Texture2D texture) {
        byte[] fileData = File.ReadAllBytes(path);
        if (fileData.Length > 0) {
            texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            return true;
        }
        texture = null;
        return false;
    }

    bool TryGetSceneFromReferenceImage (XRReferenceImage referenceImage, out Scene scene) {
        foreach (Scene _scene in _scenes) {
            if (referenceImage.name == _scene.marker.name) {
                scene = _scene;
                return true;
            }
        }
        scene = null;
        return false;
    }

    void OnEnable() => _arTrackedImageManager.trackedImagesChanged += OnChanged;

    void OnDisable() => _arTrackedImageManager.trackedImagesChanged -= OnChanged;

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        if (eventArgs.added.Count > 0) {
            XRReferenceImage newImage = eventArgs.added[0].referenceImage;
            Scene scene;
            if(TryGetSceneFromReferenceImage(newImage, out scene)) {
                SceneDetected(scene);
            };
        }
    }

    void SceneDetected(Scene scene) {
        if (_lastFoundScene != scene) {
            Debug.Log("Found Name: " + scene.marker.name);
            Debug.Log("Corresponding worldMapLink: " + scene.worldMapLink);
            _lastFoundScene = scene;
            _introUI.FoundMarker(scene);
            _sceneInfo.scene = scene;
            _arSession.Reset();
        }
    }

}
