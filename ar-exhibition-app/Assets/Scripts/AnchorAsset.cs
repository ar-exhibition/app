﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class AnchorAsset : MonoBehaviour
{
    public GameObject ModelFetcher;
    public GameObject ImageFetcher;
    public GameObject VideoFetcher;

    public GameObject PlacementIndicator;
    public GameObject Placeholder;

    [Header("Debug")]
    public bool LoadDummyObject = false;

    private ARAnchor _anchor;
    private ARAnchorManager _anchorManager;
    private Database _database;

    private DatabaseData _databaseData;
    private AssetData _asset;

    private GameObject _gameObject;
    private Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {   
        Debug.Log("Anchor Asset started");
        if (_anchor == null) {
            _anchor = GetComponent<ARAnchor>();
        }
        _anchorManager = FindObjectOfType<ARAnchorManager>();
        _database = FindObjectOfType<Database>();
        _database.GetData(DatabaseLoaded);
    }

    void DatabaseLoaded(DatabaseData data) {
        AssetData asset;
        Anchor anchor;
        if (_anchor != null) {
            if (_database.TryGetAssetByAnchorId(_anchor.trackableId.ToString(), out asset)) {
                LoadAsset(asset);
            } else {
                Debug.LogWarning("Anchor Id " +_anchor.trackableId.ToString() + " not found in database!");
                if (LoadDummyObject) {
                    AssetData dummy = new AssetData {link = "http://luziffer.ddnss.de:8080/content/assets/1442c92f-c04c-5491-b3df-8bdb166e4057.mp4", assetType = "video"};
                    LoadAsset(dummy);
                }
            }
            if (_database.TryGetAnchorById(_anchor.trackableId.ToString(), out anchor)) {
                transform.localScale = Vector3.one *  anchor.scale;
            }
        } else {
            Debug.LogWarning("No Anchor attached to " + gameObject.name);
            // Load Dummy Asset
            if (LoadDummyObject) {
                AssetData dummy = new AssetData {link = "http://luziffer.ddnss.de:8080/content/assets/243f344e-e828-59c9-8a8c-d88f427273f8.png", assetType = "image"};
                LoadAsset(dummy);
            }
        }
    }

    public void LoadAsset(AssetData asset) {
        switch (asset.assetType)
        {
            case "3d":
                ModelFetcher.GetComponent<ModelFetcher>().Url = asset.link;
                _gameObject = GameObject.Instantiate(ModelFetcher, Vector3.zero, Quaternion.identity, Placeholder.transform);
                _gameObject.transform.localPosition = Vector3.zero;
                _gameObject.transform.localRotation = Quaternion.identity;
                _gameObject.GetComponent<ModelFetcher>().OnLoaded += () => {
                    _animator.SetTrigger("place");
                };
                break;
            case "image":
                ImageFetcher.GetComponent<ImageFetcher>().Url = asset.link;
                _gameObject = GameObject.Instantiate(ImageFetcher, Vector3.zero, Quaternion.identity, Placeholder.transform);
                _gameObject.transform.localPosition = Vector3.zero;
                _gameObject.transform.localRotation = Quaternion.identity;
                _gameObject.GetComponent<ImageFetcher>().OnLoaded += () => {
                    _animator.SetTrigger("place");
                };
                break;
            case "video":
                VideoFetcher.GetComponent<VideoFetcher>().Url = asset.link;
                _gameObject = GameObject.Instantiate(VideoFetcher, Vector3.zero, Quaternion.identity, Placeholder.transform);
                _gameObject.transform.localPosition = Vector3.zero;
                _gameObject.transform.localRotation = Quaternion.identity;
                _gameObject.GetComponent<VideoFetcher>().OnLoaded += () => {
                    _animator.SetTrigger("place");
                };
                break;
            default:
                Debug.Log("Cannot handle asset of type " + asset.assetType);
                break;
        }
        _asset = asset;
        _animator.SetTrigger("place");
    }

    public void HandleClick()
    {
        switch (_asset.assetType)
        {
            case "3d":
                this.GetComponentInChildren<ModelFetcher>().StartAnimation();
                break;
            case "image":
                // Do nothing for now
                break;
            case "video":
                this.GetComponentInChildren<VideoFetcher>().StartVideo();
                break;
            default:
                Debug.Log("Cannot handle asset of type " + _asset.assetType);
                break;
        }
    }

    public void EnterSelection() {
        _animator.SetBool("selected", true);
        PlacementIndicator.SetActive(true);
    }

    public void ExitSelection() {
        _animator.SetBool("selected", false);
        PlacementIndicator.SetActive(false);
        if (_anchor != null) {
            _anchor.destroyOnRemoval = false;
            _anchorManager.RemoveAnchor(_anchor);
            _anchorManager.anchorPrefab = null;
            _anchor = _anchorManager.AddAnchor(new Pose(transform.position, transform.rotation));
        }
    }

    public AssetData GetAsset() {
        return _asset;
    }
    
    public ARAnchor GetAnchor() {
        return _anchor;
    }
    public void SetAnchor(ARAnchor anchor) {
        _anchor = anchor;
    }

    public void CreateCollider() {
        Bounds bounds = GetMaxBounds(_gameObject);
        if (bounds.size.magnitude > 0) {
            Placeholder.GetComponent<BoxCollider>().size = bounds.size * (1 / transform.localScale.x);
            Placeholder.GetComponent<BoxCollider>().center = new Vector3(0, bounds.size.y / 2.0f, 0);
        } else {
            Debug.Log("The bounds returned 0, keeping the default collider size");
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


    public void Delete() {
        _animator.SetTrigger("delete");
    }

    public void Destroy() {
        if (_anchor != null) {
            _anchor.destroyOnRemoval = true;
            _anchorManager.RemoveAnchor(_anchor);
        } else {
            GameObject.Destroy(gameObject);
        }
    }



}
