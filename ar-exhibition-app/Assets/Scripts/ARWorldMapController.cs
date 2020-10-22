using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Threading.Tasks;
#if UNITY_IOS
using UnityEngine.XR.ARKit;
#endif

/// <summary>
/// Demonstrates the saving and loading of an
/// <a href="https://developer.apple.com/documentation/arkit/arworldmap">ARWorldMap</a>
/// </summary>
/// <remarks>
/// ARWorldMaps are only supported by ARKit, so this API is in the
/// <c>UntyEngine.XR.ARKit</c> namespace.
/// </remarks>
public class ARWorldMapController : MonoBehaviour
{
    [Tooltip("The ARSession component controlling the session from which to generate ARWorldMaps.")]
    [SerializeField]
    ARSession m_ARSession;

    /// <summary>
    /// The ARSession component controlling the session from which to generate ARWorldMaps.
    /// </summary>
    public ARSession arSession
    {
        get { return m_ARSession; }
        set { m_ARSession = value; }
    }

    [SerializeField]
    ARAnchorManager m_ARAnchorManager;

    SceneInfo _sceneInfo;

    private ARWorldMappingStatus mappingStatus;
    public GameEvent MappingIsSufficient;
    public GameEvent MappingIsNotSufficient;

    void Start() {
        _sceneInfo = FindObjectOfType<SceneInfo>();
    }

    void Update() {
        #if UNITY_IOS
            var sessionSubsystem = (ARKitSessionSubsystem) m_ARSession.subsystem;
            if (mappingStatus != sessionSubsystem.worldMappingStatus) {
                mappingStatus = sessionSubsystem.worldMappingStatus;
                if (mappingStatus == ARWorldMappingStatus.Mapped) {
                    if (MappingIsSufficient != null) {
                        MappingIsSufficient.Raise();
                    }
                } else {
                    if (MappingIsNotSufficient != null) {
                        MappingIsNotSufficient.Raise();
                    }
                }
            }
        #endif
        
    }

    public void ResetSession()
    {
        m_ARSession.Reset();
    }

#if UNITY_IOS
    public async Task<bool> Save()
    {
        var sessionSubsystem = (ARKitSessionSubsystem)m_ARSession.subsystem;
        if (sessionSubsystem == null)
        {
            Log("No session subsystem available. Could not save.");
            return false;
        }

        var request = sessionSubsystem.GetARWorldMapAsync();

        while (!request.status.IsDone())
            await Task.Delay(100);

        if (request.status.IsError())
        {
            Log(string.Format("Session serialization failed with status {0}", request.status));
            return false;
        }

        await SaveAnchors();
        var worldMap = request.GetWorldMap();
        request.Dispose();

        SaveAndDisposeWorldMap(worldMap);
        return true;
    }
#endif

    public async Task<bool> SaveAnchors() {
        AnchorAsset[] anchorAssets = FindObjectsOfType<AnchorAsset>();
        Debug.Log("Found " + anchorAssets.Length + " anchors");
        Anchor[] anchors = new Anchor[anchorAssets.Length];
        for (int i = 0; i < anchorAssets.Length; i++)
        {
            AnchorAsset anchorAsset = anchorAssets[i];
            anchors[i] = new Anchor {assetId = anchorAsset.GetAsset().assetId, anchorId = anchorAsset.GetAnchor().trackableId.ToString(), scale = anchorAsset.transform.localScale.x};
        }
        AnchorPost anchorPost = new AnchorPost {anchors = anchors};
        string jsonPost = JsonUtility.ToJson(anchorPost);
        Debug.Log(jsonPost);
        using (UnityWebRequest req = new UnityWebRequest("http://luziffer.ddnss.de:8080/api/anchors", "POST"))
        {   
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPost);
            req.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
            req.SetRequestHeader("Content-Type", "application/json");
            req.SendWebRequest();
            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log($"{req.error} : {req.downloadHandler.text}");
                return false;
            }

            while (!req.isDone)
            {
                await Task.Delay(100);
            }
            return true;
        }
    }
#if UNITY_IOS
    public async Task<bool> Load(string path)
    {
        var sessionSubsystem = (ARKitSessionSubsystem)m_ARSession.subsystem;
        if (sessionSubsystem == null)
        {
            Log("No session subsystem available. Could not load.");
            return false;
        }

        var file = File.Open(path, FileMode.Open);
        if (file == null)
        {
            Log(string.Format("File {0} does not exist.", path));
            return false;
        }

        Log(string.Format("Reading {0}...", path));

        int bytesPerFrame = 1024 * 10;
        var bytesRemaining = file.Length;
        var binaryReader = new BinaryReader(file);
        var allBytes = new List<byte>();
        while (bytesRemaining > 0)
        {
            var bytes = binaryReader.ReadBytes(bytesPerFrame);
            allBytes.AddRange(bytes);
            bytesRemaining -= bytesPerFrame;
            await Task.Delay(1);
        }

        var data = new NativeArray<byte>(allBytes.Count, Allocator.Temp);
        data.CopyFrom(allBytes.ToArray());

        Log(string.Format("Deserializing to ARWorldMap...", path));
        ARWorldMap worldMap;
        if (ARWorldMap.TryDeserialize(data, out worldMap))
        data.Dispose();

        if (worldMap.valid)
        {
            Log("Deserialized successfully.");
        }
        else
        {
            Debug.LogError("Data is not a valid ARWorldMap.");
            return false;
        }

        Log("Apply ARWorldMap to current session.");
        sessionSubsystem.ApplyWorldMap(worldMap);
        return true;
    }

    async Task<bool> SaveAndDisposeWorldMap(ARWorldMap worldMap)
    {
        string worldMapUUID;

        if (_sceneInfo.scene.worldMapUUID != null && _sceneInfo.scene.worldMapUUID != "") {
            worldMapUUID = _sceneInfo.scene.worldMapUUID;
        } else {
            worldMapUUID = System.Guid.NewGuid().ToString() + ".worldmap";
        }

        Debug.Log("Using this worldmap uuid" + worldMapUUID);

        string path = FileDownloader.FilePath + worldMapUUID;
        Debug.Log("Using this path: " + path);

        Log("Serializing ARWorldMap to byte array...");
        var data = worldMap.Serialize(Allocator.Temp);
        Log(string.Format("ARWorldMap has {0} bytes.", data.Length));

        FileStream file = File.Open(path, FileMode.Create);
        var writer = new BinaryWriter(file);
        writer.Write(data.ToArray());
        writer.Close();
        worldMap.Dispose();
        Log(string.Format("ARWorldMap written to {0}", path));

        WWWForm form = new WWWForm();
        form.AddField("SceneID", _sceneInfo.scene.sceneId);
        form.AddField("FileUUID", worldMapUUID);
        form.AddBinaryData("files[]", data.ToArray(), worldMapUUID);

        Debug.Log("SceneID: " + _sceneInfo.scene.sceneId);
        using (UnityWebRequest req = UnityWebRequest.Post("http://luziffer.ddnss.de:8080/api/scenes", form))
        {   
            req.SendWebRequest();
            if (req.isNetworkError || req.isHttpError)
            {
                Debug.Log(req.error);
                data.Dispose();
                return false;
            }

            while (!req.isDone)
            {
                await Task.Delay(100);
            }
            string result = req.downloadHandler.text;
            Debug.Log("result");
            Debug.Log(result);
        }
        data.Dispose();

        return true;

    }
#endif


    bool supported
    {
        get
        {
#if UNITY_IOS
            return m_ARSession.subsystem is ARKitSessionSubsystem && ARKitSessionSubsystem.worldMapSupported;
#else
            return false;
#endif
        }
    }

    void Log(string logMessage)
    {
        Debug.Log(logMessage);
    }

}