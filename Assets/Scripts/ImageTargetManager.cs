using System;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

[Serializable]
public class ImagePrefabMap
{
    public string targetName;
    public GameObject prefab;
}

public class ImageTargetManager : MonoBehaviour
{
    [Header("Target → Prefab Mapping")]
    public List<ImagePrefabMap> mappings;

    [Header("Single Content Anchor")]
    public Transform contentAnchor;

    Dictionary<string, GameObject> prefabLookup;
    GameObject activeContent;
    [SerializeField] private ObserverBehaviour appleObserver;
    [SerializeField] private ObserverBehaviour antObserver;
    [SerializeField] private ObserverBehaviour aerplaneObserver;

    void Awake()
    {
        Debug.Log("AWAKE");
        prefabLookup = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);
        Debug.LogError(prefabLookup.Count);
        foreach (var m in mappings)
        {
            Debug.LogError("m : " + m);
            if (!string.IsNullOrWhiteSpace(m.targetName) && m.prefab != null)
            {
                Debug.Log(m.targetName);
                prefabLookup[m.targetName] = m.prefab;
            }
        }
    }

    void OnEnable()
    {
        Debug.Log("Enable");
        VuforiaApplication.Instance.OnVuforiaStarted += RegisterTargets;
    }

    void OnDisable()
    {
        Debug.Log("Disable");
        if (VuforiaApplication.Instance != null)
            VuforiaApplication.Instance.OnVuforiaStarted -= RegisterTargets;
    }

    public void OnTargetFound()
    {
        Debug.Log("TARGET FOUND");
        VuforiaApplication.Instance.OnVuforiaStarted += RegisterTargets;
    }

    public void OnTargetLost()
    {
        Debug.Log("TARGET LOST");
        Destroy(activeContent);
        VuforiaApplication.Instance.OnVuforiaStarted -= RegisterTargets;
    }


    void RegisterTargets()
    {
        Debug.LogError("Register Targets");
        var observers = FindObjectsOfType<ImageTargetBehaviour>(true);
        Debug.LogError(observers.Length);
        Debug.LogError(observers[0].name);
        foreach (var obs in observers)
            obs.OnTargetStatusChanged += OnTargetStatusChanged;
    }

    void OnTargetStatusChanged(ObserverBehaviour obs, TargetStatus status)
    {
        Debug.LogError(status);
        Debug.LogError(obs.TargetName);
        bool tracked =
            status.Status == Status.TRACKED ||
            status.Status == Status.EXTENDED_TRACKED;

        if (tracked)
            ActivateForTarget(obs);
    }

    void ActivateForTarget(ObserverBehaviour obs)
    {
        // if (activeObserver == obs)
        //     return;
        Debug.Log(obs.TargetName);
        string targetName = obs.TargetName;

        if (!prefabLookup.TryGetValue(targetName, out var prefab))
            return;

        // Move anchor to detected image
        contentAnchor.SetParent(obs.transform);
        contentAnchor.localPosition = Vector3.zero;
        contentAnchor.localRotation = Quaternion.identity;
        contentAnchor.localScale = Vector3.one;

        // Swap content
        if (activeContent != null)
            Destroy(activeContent);

        activeContent = Instantiate(prefab, contentAnchor);
        // activeObserver = obs;

        Debug.Log($"Showing content for image: {targetName}");
        SoundManager.Instance.PlayByTargetName(targetName);
    }
}
