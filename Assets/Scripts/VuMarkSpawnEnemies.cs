// using System.Diagnostics;
using UnityEngine;
using Vuforia;

[RequireComponent(typeof(VuMarkBehaviour))]
public class VuMarkSpawnEnemies : MonoBehaviour
{
    public GameObject enemyPrefab;

    // positions relative to the VuMark (meters)
    public Vector3[] localSpawnPoints =
    {
        new Vector3(-0.05f, 0.01f,  0.03f),
        new Vector3( 0.05f, 0.01f,  0.03f),
        new Vector3(-0.05f, 0.01f, -0.03f),
        new Vector3( 0.05f, 0.01f, -0.03f),
    };

    VuMarkBehaviour vb;
    GameObject[] spawned;

    void Awake()
    {
        vb = GetComponent<VuMarkBehaviour>();
    }

    void OnEnable()  => vb.OnTargetStatusChanged += OnStatus;
    void OnDisable() => vb.OnTargetStatusChanged -= OnStatus;

    void OnStatus(ObserverBehaviour b, TargetStatus s)
    {
        bool tracked = s.Status == Status.TRACKED || s.Status == Status.EXTENDED_TRACKED;

        if (tracked)
        {
            Debug.Log("if");
            if (spawned == null) Spawn();
            Debug.Log("true");
            SetActive(true);
        }
        else
        {
            Debug.Log("else");
            SetActive(false); // smoother than Destroy() when tracking flickers
            Spawn();
        }
    }

    void Spawn()
    {
        spawned = new GameObject[localSpawnPoints.Length];

        for (int i = 0; i < localSpawnPoints.Length; i++)
        {
            var e = Instantiate(enemyPrefab, transform); // parent to VuMark
            e.transform.localPosition = localSpawnPoints[i];
            e.transform.localRotation = Quaternion.identity;
            spawned[i] = e;
        }
    }

    void SetActive(bool on)
    {
        if (spawned == null) return;
        for (int i = 0; i < spawned.Length; i++)
            if (spawned[i]) spawned[i].SetActive(on);
    }
}
