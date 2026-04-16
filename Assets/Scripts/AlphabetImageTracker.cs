  using UnityEngine;
using Vuforia;
// using UnityEngine.AddressableAssets;
// using UnityEngine.ResourceManagement.AsyncOperations;

public class AlphabetImageTracker : MonoBehaviour
{
    [SerializeField] private ObserverBehaviour observer;
    private GameObject spawnedObject;
    // private AsyncOperationHandle<GameObject>? handle;

    async void Awake()
    {
        // await Addressables.InitializeAsync().Task;
        Debug.Log("Addressables initialized");
    }

    void Start()
    {
        observer.OnTargetStatusChanged += OnStatusChanged;
    }

    void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED)
        {
            LoadAndShow();
        }
        else
        {
            Hide();
        }
    }

    async void LoadAndShow()
    {
        if (spawnedObject != null)
        {
            spawnedObject.SetActive(true);
            return;
        }

        string targetName = observer.TargetName;   // A_Ant
        string prefabKey = targetName.Split('_')[1]; // Ant
        Debug.Log($"Loading prefab for {targetName}");
        Debug.Log($"Loading prefab for {prefabKey}");
        // handle = Addressables.LoadAssetAsync<GameObject>(prefabKey);
        
        // await handle.Value.Task;

        // if (handle.Value.Status == AsyncOperationStatus.Succeeded)
        // {
        //     spawnedObject = Instantiate(handle.Value.Result, transform);
        // }
    }

    void Hide()
    {
        if (spawnedObject)
            spawnedObject.SetActive(false);
    }

    void OnDestroy()
    {
        // if (handle.HasValue)
            // Addressables.Release(handle.Value);
    }
}
