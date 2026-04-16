using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracker : MonoBehaviour
{
    [SerializeField]private ARTrackedImageManager trackedImages;
    [SerializeField]private GameObject[] prefabs;
    
    [Header("Debug")]
    [SerializeField] private bool debugLogs = false;

    private readonly Dictionary<string, GameObject> prefabMap = new();
    private readonly Dictionary<string, GameObject> activeObjects = new();
    private readonly Dictionary<string, GameObject> pooledObjects = new();

    // -------------------- UNITY --------------------

    void Awake()
    {
        // if (!trackedImages)
        //     trackedImages = FindObjectOfType<ARTrackedImageManager>();

        // if (!trackedImages)
        // {
        //     Debug.LogError("ARTrackedImageManager missing");
        //     enabled = false;
        //     return;
        // }

        BuildPrefabMap();
    }

    void OnEnable()
    {
        trackedImages.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImages.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    // -------------------- PREFAB MAP --------------------

    void BuildPrefabMap()
    {
        prefabMap.Clear();

        foreach (var prefab in prefabs)
        {
            if (!prefab) continue;

            string key = Normalize(prefab.name);

            if (!prefabMap.ContainsKey(key))
                prefabMap.Add(key, prefab);
        }

        Log($"PrefabMap initialized ({prefabMap.Count})");
    }

    // -------------------- EVENTS --------------------

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var img in args.added)
            HandleAdded(img);

        foreach (var img in args.updated)
            HandleUpdated(img);

        foreach (var img in args.removed)
            HandleRemoved(img);
    }

    // -------------------- HANDLERS --------------------

    void HandleAdded(ARTrackedImage img)
    {
        if (!IsValid(img)) return;

        string key = Normalize(img.referenceImage.name);

        if (activeObjects.ContainsKey(key))
            return;

        GameObject obj = GetFromPool(key, img.transform);

        if (!obj)
        {
            Log($"No prefab for image: {key}");
            return;
        }

        activeObjects[key] = obj;
        obj.SetActive(true);
        Log($"Activated: {key}");
    }

    void HandleUpdated(ARTrackedImage img)
    {
        if (!IsValid(img)) return;

        string key = Normalize(img.referenceImage.name);

        if (!activeObjects.TryGetValue(key, out var obj))
        {
            HandleAdded(img);
            return;
        }

        obj.SetActive(img.trackingState == TrackingState.Tracking);
    }

    void HandleRemoved(ARTrackedImage img)
    {
        if (!IsValid(img)) return;

        string key = Normalize(img.referenceImage.name);

        if (!activeObjects.TryGetValue(key, out var obj))
            return;

        ReturnToPool(key, obj);
        activeObjects.Remove(key);
        Log($"Returned to pool: {key}");
    }

    // -------------------- POOLING --------------------

    GameObject GetFromPool(string key, Transform parent)
    {
        if (pooledObjects.TryGetValue(key, out var pooled))
        {
            pooledObjects.Remove(key);
            pooled.transform.SetParent(parent);
            ResetTransform(pooled.transform);
            return pooled;
        }

        if (!prefabMap.TryGetValue(key, out var prefab))
            return null;

        var instance = Instantiate(prefab, parent);
        ResetTransform(instance.transform);
        return instance;
    }

    void ReturnToPool(string key, GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        pooledObjects[key] = obj;
    }

    // -------------------- UTIL --------------------

    static bool IsValid(ARTrackedImage img)
    {
        return img && img.referenceImage != null && !string.IsNullOrEmpty(img.referenceImage.name);
    }

    static void ResetTransform(Transform t)
    {
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        // t.localScale = Vector3.one;
    }

    static string Normalize(string value)
    {
        return value.Trim().ToLowerInvariant();
    }

    void Log(string msg)
    {
        if (debugLogs)
            Debug.Log("[ImageTracker] " + msg);
    }
}
