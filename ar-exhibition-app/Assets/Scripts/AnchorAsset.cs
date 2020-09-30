using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class AnchorAsset : MonoBehaviour
{
    public GameObject ModelFetcher;
    public GameObject ImageFetcher;
    public GameObject VideoFetcher;

    private ARAnchor _anchor;
    private Database _database;

    private DatabaseData _databaseData;

    // Start is called before the first frame update
    void Start()
    {
        _anchor = GetComponent<ARAnchor>();
        _database = FindObjectOfType<Database>();

        _database.GetData(DatabaseLoaded);
    }

    void DatabaseLoaded(DatabaseData data) {
        AssetData asset;
        if (_database.TryGetAssetByAnchorId(_anchor.sessionId.ToString(), out asset)) {
            LoadAsset(asset);
        }

    }

    public void LoadAsset(AssetData asset) {
        GameObject go;
        switch (asset.assetType)
        {
            case "3d":
                ModelFetcher.GetComponent<ModelFetcher>().Url = asset.link;
                go = GameObject.Instantiate(ModelFetcher, Vector3.zero, Quaternion.identity, transform);
                go.transform.localPosition = Vector3.zero;
                break;
            case "image":
                ImageFetcher.GetComponent<ImageFetcher>().Url = asset.link;
                go = GameObject.Instantiate(ImageFetcher, Vector3.zero, Quaternion.identity, transform);
                go.transform.localPosition = Vector3.zero;
                break;
            case "video":
                VideoFetcher.GetComponent<VideoFetcher>().Url = asset.link;
                go = GameObject.Instantiate(VideoFetcher, Vector3.zero, Quaternion.identity, transform);
                go.transform.localPosition = Vector3.zero;
                break;
            default:
                Debug.Log("Cannot handle asset of type " + asset.assetType);
                break;
        }
    }

}
