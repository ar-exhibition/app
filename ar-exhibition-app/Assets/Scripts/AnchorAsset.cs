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

    public GameObject PlacementIndicator;
    public GameObject Placeholder;

    [Header("Debug")]
    public bool LoadDummyObject = false;

    private ARAnchor _anchor;
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
        _anchor = GetComponent<ARAnchor>();
        _database = FindObjectOfType<Database>();
        _database.GetData(DatabaseLoaded);
    }

    void DatabaseLoaded(DatabaseData data) {
        AssetData asset;
        Anchor anchor;
        if (_anchor != null) {
            if (_database.TryGetAssetByAnchorId(_anchor.trackableId.ToString(), out asset)) {
                LoadAsset(asset);
            }
            if (_database.TryGetAnchorById(_anchor.trackableId.ToString(), out anchor)) {
                transform.localScale = Vector3.one *  anchor.scale;
            }
        } else {
            Debug.LogWarning("No Anchor attached to " + gameObject.name);
            // Load Dummy Asset
            if (LoadDummyObject) {
                AssetData dummy = new AssetData {link = "https://www.dropbox.com/s/4rz9t48paxuhhoj/Astronaut.glb?dl=1&type=assets&file=Astronaut.glb", assetType = "3d"};
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
                break;
            case "image":
                ImageFetcher.GetComponent<ImageFetcher>().Url = asset.link;
                _gameObject = GameObject.Instantiate(ImageFetcher, Vector3.zero, Quaternion.identity, Placeholder.transform);
                _gameObject.transform.localPosition = Vector3.zero;
                break;
            case "video":
                VideoFetcher.GetComponent<VideoFetcher>().Url = asset.link;
                _gameObject = GameObject.Instantiate(VideoFetcher, Vector3.zero, Quaternion.identity, Placeholder.transform);
                _gameObject.transform.localPosition = Vector3.zero;
                break;
            default:
                Debug.Log("Cannot handle asset of type " + asset.assetType);
                break;
        }
        _asset = asset;
        _animator.SetTrigger("place");
    }

    public void EnterSelection() {
        _animator.SetBool("selected", true);
        PlacementIndicator.SetActive(true);
    }

    public void ExitSelection() {
        _animator.SetBool("selected", false);
        PlacementIndicator.SetActive(false);
    }

    public AssetData GetAsset() {
        return _asset;
    }
    
    public ARAnchor GetAnchor() {
        return _anchor;
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
        GameObject.Destroy(gameObject);
    }



}
