using UnityEngine;
using Vuforia;

public class AutoSpawnOnGround : MonoBehaviour
{
    public ImageTargetBehaviour imageTarget;   // Drag your ImageTarget here
    public GameObject prefabToSpawn;

    private GameObject spawned;
    private bool isSpawned;

    void OnEnable()
    {
        if (imageTarget != null)
            imageTarget.OnTargetStatusChanged += OnTargetStatusChanged;
    }

    void OnDisable()
    {
        if (imageTarget != null)
            imageTarget.OnTargetStatusChanged -= OnTargetStatusChanged;
    }

    void Start()
    {
        // Optional: hide until target is found
        if (spawned != null)
            spawned.SetActive(false);
    }

    void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        bool trackingGood =
            status.Status == Status.TRACKED ||
            status.Status == Status.EXTENDED_TRACKED ||
            status.Status == Status.LIMITED;

        if (trackingGood)
        {
            if (!isSpawned)
            {
                spawned = Instantiate(prefabToSpawn, imageTarget.transform);
                // spawned.transform.localPosition = Vector3.zero;
                // spawned.transform.localRotation = Quaternion.identity;
                // spawned.transform.localScale = Vector3.one * 0.1f; // adjust as needed
                isSpawned = true;
            }

            spawned.SetActive(true);
        }
        else
        {
            if (spawned != null)
                spawned.SetActive(false);
        }
    }
}
