using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Siccity.GLTFUtility;
using UnityEngine.UI;

public class ModelLoader : MonoBehaviour
{

    GameObject wrapper;
    GameObject spriteWrapper;
    Sprite sprite;
    Texture2D tex;
    string filePath;

    [SerializeField] float modelDimension = 3f;
    [SerializeField] float imageDimension = 3f;

    [SerializeField] GameObject progressSlider;

    // Start is called before the first frame update
    void Start()
    {
        filePath = $"{Application.persistentDataPath}/Files/";
        tex = new Texture2D(600, 600);
    }

    public void DownloadFile(string url)
    {
        string path = GetFilePath(url);
        if (File.Exists(path))
        {
            Debug.Log("Found file locally, loading...");
            LoadModel(path);
            return;
        }

        StartCoroutine(GetFileRequest(url, (req) =>
        {
            LoadModel(path);
        }));
    }

    public void DownloadTexture(string url)
    {
        string path = GetFilePath(url);
        CreateSpriteWrapper();

        StartCoroutine(GetImageRequest(url, (response) =>
        {
            sprite = response;
            spriteWrapper.GetComponent<SpriteRenderer>().sprite = sprite;
            ResizeImage(spriteWrapper);
        }));
        
    }


    string GetFilePath(string url)
    {
        string[] pieces = url.Split('/');
        string fileName = pieces[pieces.Length - 1];

        if (fileName.Contains("?raw=true"))
        {
            string[] fileNamePieces = fileName.Split('?');
            fileName = fileNamePieces[0];
        }
            

        return $"{filePath}{fileName}";
    }

    void LoadImage(string path)
    {
        
        CreateSpriteWrapper();
    }

    void LoadModel(string path)
    {
        CreateWrapper();
        GameObject model = Importer.LoadFromFile(path);
        model.transform.SetParent(wrapper.transform);
        
        Resize(wrapper);
    }

    void CreateSpriteWrapper()
    {
        spriteWrapper = new GameObject
        {
            name = "Image"
        };
        spriteWrapper.AddComponent<SpriteRenderer>();
    }

    void CreateWrapper()
    {
        wrapper = new GameObject
        {
            name = "Model"
        };
    }

    void ResizeImage(GameObject image)
    {
        SpriteRenderer sr = image.GetComponent<SpriteRenderer>();

        float max = Mathf.Max(Mathf.Max(sr.bounds.size.x, sr.bounds.size.y), sr.bounds.size.z);
        float scalingFactor = imageDimension / max;
        image.transform.localScale *= scalingFactor;
    }

    void Resize(GameObject model)
    {
        MeshRenderer renderer = model.GetComponentInChildren<MeshRenderer>();

        float max = Mathf.Max(Mathf.Max(renderer.bounds.size.x, renderer.bounds.size.y), renderer.bounds.size.z);
        float scalingFactor = max / modelDimension;
        model.transform.localScale /= scalingFactor;       
    }

    IEnumerator GetImageRequest(string url, System.Action<Sprite> callback)
    {
        using (var req = UnityWebRequestTexture.GetTexture(url))
        {
            yield return req.SendWebRequest();
            
            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log(req.error);
            }
            else
            {
                if (req.isDone)
                {
                    var texture = DownloadHandlerTexture.GetContent(req);
                    var rect = new Rect(0, 0, texture.width, texture.height);
                    var sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
                    callback(sprite);
                }
            }
        }
    }

    IEnumerator GetFileRequest(string url, Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.downloadHandler = new DownloadHandlerFile(GetFilePath(url));
            req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log($"{req.error} : {req.downloadHandler.text}");
            }

            progressSlider.SetActive(true);
            while (!req.isDone)
            {
                progressSlider.GetComponent<Slider>().value = req.downloadProgress;
                yield return null;
            }
            progressSlider.SetActive(false);
            callback(req);
        }
    }
}
